using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Intranet.Core.DTOs.Attachments
{
    public class AttachmentForCreation
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string FolderName { get; set; }
        public string? Visibility { get; set; }
        public int? JobId { get; set; }
        public int? JobTaskId { get; set; }
    }
}
