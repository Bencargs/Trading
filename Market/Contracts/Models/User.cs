using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contracts.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
    }
}