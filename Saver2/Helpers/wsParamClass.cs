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

        public static bool online { get; set; }
        public static int volume { get; set; }
        public static string ws_url { get; set; }
        public static string user_id { get; set; }
        public static string aparatoid { get; set; }
        public static string session_id { get; set; }
        public static int stage { get; set; }
        public static string lang { get; set; }
        public static string scaned { get; set; }
        public static void Init ()
        {
            _wsMethodName = string.Empty;
            wsMethodName = string.Empty;
            ws_url = string.Empty;
            user_id = string.Empty;
            aparatoid = string.Empty;
            session_id = string.Empty;
            stage = 0;
            scaned = string.Empty;
            lang = string.Empty;
        }
        
    }
}