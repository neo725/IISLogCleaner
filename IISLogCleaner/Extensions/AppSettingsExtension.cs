using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogCleaner.Extensions
{
    public static class AppSettingsExtension
    {
        public static string TryGet(this NameValueCollection settings, string key, bool required = true)
        {
            var valueNotAvailable = false;

            if (ConfigurationManager.AppSettings.AllKeys.Count(k => k == key) == 0)
            {
                // AppSetting not found
                valueNotAvailable = true;
            }

            var value = ConfigurationManager.AppSettings[key];

            if (required && String.IsNullOrEmpty(value))
            {
                // value is null or empty
                valueNotAvailable = true;
            }

            if (valueNotAvailable)
            {
                throw new Exception($"必要的參數 : ${key} 尚未設定或設定不正確");
            }

            return value;
        }
    }
}
