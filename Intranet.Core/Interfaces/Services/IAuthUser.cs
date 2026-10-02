using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Interfaces.Services
{
    public interface IAuthUser
    {
        int UserId { get; }
        string Name { get; }
        string Email { get; }
        string Role { get; }
    }
}
