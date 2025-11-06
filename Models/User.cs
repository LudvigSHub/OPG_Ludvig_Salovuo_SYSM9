using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public virtual bool IsAdmin { get; protected set; } = false;


    }

    public class Admin : User
    {
        public Admin()
        {
            IsAdmin = true;
        }
    }
}
