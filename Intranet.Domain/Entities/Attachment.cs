using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Intranet.Domain.Entities
{
    public class Attachment : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; } // PNG, JPG, PDF, XML, DOCX
        public string FileExtension { get; set; } // .pdf, .jpg, .png, .docx, etc.
        public long FileSize { get; set; } // in bytes
        public string Visibility { get; set; } // "SuperAdmin", "InternalUser"
    }
}
