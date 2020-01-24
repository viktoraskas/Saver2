using System.Collections.Generic;

namespace Saver2.helpers
{
    public class AppConfigRoot
    {
        public List<AppConfigItemsClass> list { get; set; }
    }
    public class AppConfigItemsClass
    {
        public string param_name { get; set; }
        public string param_value { get; set; }
    }

    public class AppConfigClass
    {
        public static string AppVersion { get; set; }
    }
}