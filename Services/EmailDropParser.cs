using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AdviceFirstApi.Domain.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using MimeKit;
using MsgReader.Outlook.Storage;

namespace AdviceFirstApi.Services
{
    public class EmailDropParser : IEmailDropParser
    {
        private static readonly string[] SupportedExtensions = { ".msg", ".eml" };
        private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new FileExtensionContentTypeProvider();

        public async Task<EmailDropModel> ParseAsync(IFormFile emailFile, CancellationToken cancellationToken = default)
        {
            if (emailFile == null)
            {
                throw new ArgumentNullException(nameof(emailFile));
            }

            if (emailFile.Length <= 0)
            {
                throw new ArgumentException("Email file is empty.", nameof(emailFile));
            }

            var extension = Path.GetExtension(emailFile.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new NotSupportedException("Only .msg and .eml files are supported.");
            }

            using var stream = new MemoryStream();
            await emailFile.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
            stream.Position = 0;

            if (extension.Equals(".msg", StringComparison.OrdinalIgnoreCase))
            {
                return ParseMsg(stream, emailFile.FileName);
            }

            return await ParseEmlAsync(stream, emailFile.FileName, cancellationToken).ConfigureAwait(false);
        }

        private static EmailDropModel ParseMsg(Stream stream, string sourceFileName)
        {
            using var message = new Message(stream);
            var parsedEmail = new EmailDropModel
            {
                SourceFileName = sourceFileName,
                Subject = GetStringValue(message, "Subject"),
                BodyText = GetStringValue(message, "BodyText"),
                BodyHtml = GetStringValue(message, "BodyHtml"),
                From = BuildSenderValue(GetPropertyValue(message, "Sender")),
                SentOn = ToDateTimeOffset(GetPropertyValue(message, "SentOn", "DeliveryTime"))
            };

            parsedEmail.To.AddRange(ReadMsgToRecipients(message));

            var attachments = GetPropertyValue(message, "Attachments") as IEnumerable;
            if (attachments == null)
            {
                return parsedEmail;
            }

            foreach (var attachment in attachments)
            {
                var data = GetByteArrayValue(attachment, "Data");
                if (data == null || data.Length == 0)
                {
                    continue;
                }

                var fileName = GetStringValue(attachment, "FileName", "LongFileName", "DisplayName");
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = "attachment.bin";
                }

                parsedEmail.Attachments.Add(new EmailDropAttachmentModel
                {
                    FileName = fileName,
                    ContentType = ResolveContentType(fileName),
                    SizeBytes = data.LongLength,
                    ContentBase64 = Convert.ToBase64String(data)
                });
            }

            return parsedEmail;
        }

        private static async Task<EmailDropModel> ParseEmlAsync(Stream stream, string sourceFileName, CancellationToken cancellationToken)
        {
            var message = await MimeMessage.LoadAsync(stream, cancellationToken).ConfigureAwait(false);
            var parsedEmail = new EmailDropModel
            {
                SourceFileName = sourceFileName,
                Subject = message.Subject,
                From = message.From?.ToString(),
                BodyText = message.TextBody,
                BodyHtml = message.HtmlBody,
                SentOn = message.Date == DateTimeOffset.MinValue ? (DateTimeOffset?)null : message.Date
            };

            parsedEmail.To = message.To
                .Select(address => address.ToString())
                .Where(address => !string.IsNullOrWhiteSpace(address))
                .ToList();

            foreach (var attachment in message.Attachments)
            {
                var parsedAttachment = ConvertMimeAttachment(attachment);
                if (parsedAttachment != null)
                {
                    parsedEmail.Attachments.Add(parsedAttachment);
                }
            }

            return parsedEmail;
        }

        private static EmailDropAttachmentModel ConvertMimeAttachment(MimeEntity attachment)
        {
            if (attachment == null)
            {
                return null;
            }

            var fileName = attachment.ContentDisposition?.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = attachment.ContentType?.Name;
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "attachment.bin";
            }

            var contentType = attachment.ContentType?.MimeType ?? ResolveContentType(fileName);

            using var output = new MemoryStream();
            if (attachment is MessagePart messagePart)
            {
                messagePart.Message.WriteTo(output);
                contentType = "message/rfc822";
                if (string.Equals(fileName, "attachment.bin", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = "attached-email.eml";
                }
            }
            else if (attachment is MimePart mimePart)
            {
                mimePart.Content?.DecodeTo(output);
            }
            else
            {
                return null;
            }

            var data = output.ToArray();
            return new EmailDropAttachmentModel
            {
                FileName = fileName,
                ContentType = contentType,
                SizeBytes = data.LongLength,
                ContentBase64 = Convert.ToBase64String(data)
            };
        }

        private static IEnumerable<string> ReadMsgToRecipients(Message message)
        {
            var recipientsMethod = typeof(Message)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(method => method.Name == "GetEmailRecipients" && method.GetParameters().Length == 3);

            if (recipientsMethod == null)
            {
                return Enumerable.Empty<string>();
            }

            var enumType = recipientsMethod.GetParameters()[0].ParameterType;
            if (!enumType.IsEnum)
            {
                return Enumerable.Empty<string>();
            }

            object toRecipientType;
            try
            {
                toRecipientType = Enum.Parse(enumType, "To", true);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }

            // Reflection keeps this resilient across MsgReader signature changes.
            string recipientsRaw;
            try
            {
                recipientsRaw = recipientsMethod.Invoke(message, new object[] { toRecipientType, false, false }) as string;
            }
            catch
            {
                return Enumerable.Empty<string>();
            }

            if (string.IsNullOrWhiteSpace(recipientsRaw))
            {
                return Enumerable.Empty<string>();
            }

            return recipientsRaw
                .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string BuildSenderValue(object sender)
        {
            if (sender == null)
            {
                return null;
            }

            var displayName = GetStringValue(sender, "DisplayName", "Name");
            var email = GetStringValue(sender, "Email", "EmailAddress", "Address", "SmtpAddress");

            if (string.IsNullOrWhiteSpace(displayName))
            {
                return string.IsNullOrWhiteSpace(email) ? sender.ToString() : email;
            }

            return string.IsNullOrWhiteSpace(email) ? displayName : $"{displayName} <{email}>";
        }

        private static DateTimeOffset? ToDateTimeOffset(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is DateTimeOffset dateTimeOffset && dateTimeOffset != DateTimeOffset.MinValue)
            {
                return dateTimeOffset;
            }

            if (value is DateTime dateTime && dateTime != DateTime.MinValue)
            {
                return new DateTimeOffset(dateTime);
            }

            return DateTimeOffset.TryParse(value.ToString(), out var parsedDate) ? parsedDate : (DateTimeOffset?)null;
        }

        private static string ResolveContentType(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName) && ContentTypeProvider.TryGetContentType(fileName, out var contentType))
            {
                return contentType;
            }

            return "application/octet-stream";
        }

        private static object GetPropertyValue(object source, params string[] propertyNames)
        {
            if (source == null || propertyNames == null || propertyNames.Length == 0)
            {
                return null;
            }

            var sourceType = source.GetType();
            foreach (var propertyName in propertyNames)
            {
                var property = sourceType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    return property.GetValue(source);
                }
            }

            return null;
        }

        private static string GetStringValue(object source, params string[] propertyNames)
        {
            var value = GetPropertyValue(source, propertyNames);
            return value?.ToString();
        }

        private static byte[] GetByteArrayValue(object source, params string[] propertyNames)
        {
            var value = GetPropertyValue(source, propertyNames);
            return value as byte[];
        }
    }
}
