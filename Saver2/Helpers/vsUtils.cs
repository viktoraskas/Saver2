using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saver2.Activity;
using static Saver2.helpers.ConstantsClass;

namespace Saver2.Helpers
{
    public class vsUtils
    {
        public static bool CheckURLValid(string strURL)
        {
            Uri uriResult;
            return Uri.TryCreate(strURL, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == Uri.UriSchemeHttp);
        }
        public static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static void SaveConfig(Context context,Dictionary<string, dynamic> _cnfg)
        {
            using (ISharedPreferences sharedprefs = context.GetSharedPreferences(AppConfigJson, FileCreationMode.Private))
            {
                if (_cnfg != null && _cnfg.Count > 0)
                {
                    var str = JsonConvert.SerializeObject(_cnfg);
                    sharedprefs.Edit().PutString(AppConfigJson, str).Commit();
                    Signalize.Good(context);
                }
            }
        }
        public static Dictionary<string,dynamic> WsParamToDictionary()
        {  
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>()
            {
                {"aparato_id",wsParam.aparato_id },
                {"service_key", wsParam.service_key},
                {"ws2Url",wsParam.ws2Url},
                {"timeout",wsParam.timeout}
            };
            return result;
        }
        public static bool GetAppConfig(Context context)
        {
            bool result = true;
            using (ISharedPreferences sharedprefs = context.GetSharedPreferences(AppConfigJson, FileCreationMode.Private))
            {
                var jsonStr = sharedprefs.GetString(AppConfigJson, string.Empty);
                if (string.IsNullOrEmpty(jsonStr) || !IsValidJson(jsonStr))
                {
                    result = false;
                    return result;
                }
                wsParam.aparato_id = string.Empty;
                wsParam.service_key = string.Empty;
                wsParam.ws2Url = string.Empty;
                Dictionary<string, string> cnfg = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
                if (cnfg.ContainsKey("aparato_id"))
                {
                    wsParam.aparato_id = cnfg["aparato_id"];
                }
                if (cnfg.ContainsKey("service_key"))
                {
                    wsParam.service_key = cnfg["service_key"];
                }
                if (cnfg.ContainsKey("ws2Url"))
                {
                    wsParam.ws2Url = cnfg["ws2Url"];
                }
                wsParam.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
                wsParam.timeout = 30.ToString();
                if (cnfg.ContainsKey("timeout"))
                {
                    wsParam.timeout = cnfg["timeout"];
                }
            }

            if (string.IsNullOrWhiteSpace(wsParam.aparato_id) || string.IsNullOrWhiteSpace(wsParam.service_key) || string.IsNullOrWhiteSpace(wsParam.ws2Url))
            {
                result = false;
            }
            return result;
        }
        public static void logOut(Context context)
        {
            wsParam.aparato_id = string.Empty;
            wsParam.service_key = string.Empty;
            vsUtils.SaveConfig(context, vsUtils.WsParamToDictionary());
            Intent intent = new Intent(Application.Context, typeof(FirstLogin));
            context.StartActivity(intent);
            //this.Finish(); //nežinau kaip užbaigti activity iš čia
        }

     }
}