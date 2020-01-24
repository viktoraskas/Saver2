
using System.Collections.Generic;

namespace Saver2.Helpers
{
    public class MenuClass1
    {
        public string MenuName { get; set; }
        public string MenuIcon { get; set; }
    }
    public class MenuClass
    {
        public string status { get; set; }
        public string description { get; set; }
        public List<MenuOperation> operation { get; set; }
    }

    public class MenuOperation
    {
        public string module { get; set; }
        public string message_lt { get; set; }
        public string message_en { get; set; }
        public string message(string lang)
        {
            if (lang == "LT")
            {
                return message_lt;
            }
            return message_en;
        }
    }

}