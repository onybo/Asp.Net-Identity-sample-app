using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCustomUser.Models
{
    public class RegistrationViewModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
    }
}