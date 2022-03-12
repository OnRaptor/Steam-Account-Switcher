using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamSwitcher.Model
{
    [Serializable]
    public class Account
    {
        public string ViewName { get; set; }
        public string password { get; set; }
        public string login { get; set; }
        public string tag { get; set; }
        public string ImageUrl { get; set; }
        public string SteamUrl { get; set; }
        public int ID { get; set; }
        public string SteamID64 { get; set; }

        public Account(string login, string password)
        {
            this.login = login;
            this.password = password;
        }

    }
}
