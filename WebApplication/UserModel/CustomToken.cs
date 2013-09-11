using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserModel
{
    public class CustomToken : IToken
    {
        [Key]
        public string Id
        {
            get;
            set;
        }

        public DateTime ValidUntilUtc
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
        public CustomToken()
        {
            Id = Guid.NewGuid().ToString();
			ValidUntilUtc = DateTime.MaxValue;
        }
    }
}
