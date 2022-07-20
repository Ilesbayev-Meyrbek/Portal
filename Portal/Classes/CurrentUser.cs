using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Classes
{
    public class CurrentUser
    {
        public string Login { get; set; }
        public string MarketID { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsUser { get; set; }
    }
}
