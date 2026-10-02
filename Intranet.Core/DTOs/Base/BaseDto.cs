using Intranet.Core.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Base
{
    public class BaseDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public UserOverviewDto? DeletedBy { get; set; }
        public UserOverviewDto CreatedBy { get; set; }
        public UserOverviewDto? UpdatedBy { get; set; }
    }
}
