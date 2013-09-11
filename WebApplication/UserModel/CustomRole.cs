using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserModel
{
    public class CustomRole : IRole
    {
        [Key]
        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
