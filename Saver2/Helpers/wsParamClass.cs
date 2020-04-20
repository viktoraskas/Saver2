namespace Saver2.Helpers
{
    static class wsParam
    {

        private static string _wsMethodName;
        public static string wsMethodName
        {
            get => _wsMethodName;
            set => _wsMethodName = "c_" + value.ToString().ToLower();
        }

        public const string wsMethodDefault = "0a";
        public static bool online { get; set; }
        public static int volume { get; set; }
        public static string timeout { get; set; }
        public static System.DateTime logout_time { get; set; }
        public static string ws2Url { get; set; }
        public static string service_key { get; set; }
        public static string aparato_id { get; set; }
        public static string session_id { get; set; }
        public static int stage { get; set; }
        public static string lang { get; set; }
        public static string scaned { get; set; }
        public static void Init ()
        {
            //default vaules
            _wsMethodName = string.Empty;
            wsMethodName = string.Empty;
            ws2Url = string.Empty;
            service_key = string.Empty;
            aparato_id = string.Empty;
            session_id = string.Empty;
            stage = 0;
            timeout = 30.ToString();
            scaned = string.Empty;
            lang = string.Empty;
            logout_time = new System.DateTime(2999, 12, 31, 23, 59, 59);
        }
        
    }
}