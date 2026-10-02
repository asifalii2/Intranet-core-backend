using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Hub
{
    public class HubUserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string ConnectionId { get; set; }
    }
}
