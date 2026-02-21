using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AdviceFirstApi.Domain.Models.Dto
{
    public class EmailDropRequestModel
    {
        [Required]
        public IFormFile EmailFile { get; set; }
    }

    public class EmailDropModel
    {
        public string SourceFileName { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public List<string> To { get; set; } = new List<string>();
        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
        public DateTimeOffset? SentOn { get; set; }
        public List<EmailDropAttachmentModel> Attachments { get; set; } = new List<EmailDropAttachmentModel>();
    }

    public class EmailDropAttachmentModel
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long SizeBytes { get; set; }
        public string ContentBase64 { get; set; }
    }
}
