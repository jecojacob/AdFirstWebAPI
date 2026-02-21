using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AdviceFirstApi.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AdviceFirstApi.Test
{
    public class EmailDropParserTests
    {
        [Fact]
        public async Task ParseAsync_WithEmlFile_ReturnsEmailAndAttachment()
        {
            var parser = new EmailDropParser();
            var eml = BuildSampleEml();
            var file = BuildFormFile("emailFile", "dragged-email.eml", eml, "message/rfc822");

            var result = await parser.ParseAsync(file);

            Assert.NotNull(result);
            Assert.Equal("dragged-email.eml", result.SourceFileName);
            Assert.Equal("Outlook drag drop test", result.Subject);
            Assert.Contains("sender@example.com", result.From);
            Assert.Single(result.To);
            Assert.Contains("receiver@example.com", result.To[0]);
            Assert.Equal("Body from Outlook drag/drop.", result.BodyText.Trim());
            Assert.Single(result.Attachments);
            Assert.Equal("note.txt", result.Attachments[0].FileName);
            Assert.Equal("text/plain", result.Attachments[0].ContentType);
            Assert.Equal(
                "Hello Blazor attachment",
                Encoding.UTF8.GetString(Convert.FromBase64String(result.Attachments[0].ContentBase64)));
        }

        [Fact]
        public async Task ParseAsync_WithUnsupportedFileType_ThrowsNotSupportedException()
        {
            var parser = new EmailDropParser();
            var file = BuildFormFile("emailFile", "dragged-email.txt", "not supported", "text/plain");

            var exception = await Assert.ThrowsAsync<NotSupportedException>(() => parser.ParseAsync(file));

            Assert.Equal("Only .msg and .eml files are supported.", exception.Message);
        }

        private static IFormFile BuildFormFile(string fieldName, string fileName, string content, string contentType)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, fieldName, fileName)
            {
                ContentType = contentType
            };
        }

        private static string BuildSampleEml()
        {
            var attachmentBody = Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello Blazor attachment"));
            return string.Join(
                "\r\n",
                "From: Sender <sender@example.com>",
                "To: Receiver <receiver@example.com>",
                "Subject: Outlook drag drop test",
                "Date: Fri, 21 Feb 2025 11:20:00 +0000",
                "MIME-Version: 1.0",
                "Content-Type: multipart/mixed; boundary=\"drag-drop-boundary\"",
                string.Empty,
                "--drag-drop-boundary",
                "Content-Type: text/plain; charset=utf-8",
                string.Empty,
                "Body from Outlook drag/drop.",
                "--drag-drop-boundary",
                "Content-Type: text/plain; name=\"note.txt\"",
                "Content-Disposition: attachment; filename=\"note.txt\"",
                "Content-Transfer-Encoding: base64",
                string.Empty,
                attachmentBody,
                "--drag-drop-boundary--",
                string.Empty);
        }
    }
}
