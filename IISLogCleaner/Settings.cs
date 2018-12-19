using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IISLogCleaner.Extensions;

namespace IISLogCleaner
{
    public static class Settings
    {

        public static bool Load()
        {
            // 測試從 Config 取得相關設定值
            // 如果有錯誤發生，由這裡回傳 false

            ProcessLog.Instance.Debug(String.Empty.PadLeft(10, '=')); // 我是分隔線

            try
            {
                ProcessLog.Instance.Debug("目前組態設定值 :");
                ProcessLog.Instance.Debug();
                ProcessLog.Instance.Debug($" Mode : {Settings.Mode}");
                ProcessLog.Instance.Debug($" IIS Log Path : {Settings.IISLogPath}");
                ProcessLog.Instance.Debug($" PreserveDays : {Settings.PreserveDays}");
                ProcessLog.Instance.Debug($" BackupStagePath : {Settings.BackupStagePath}");

                return true;
            }
            catch (Exception ex)
            {
                ProcessLog.Instance.Debug("讀取組態設定發生以下錯誤 :");
                ProcessLog.Instance.Debug();
                ProcessLog.Instance.Debug(ex.Message);

                return false;
            }

            finally
            {
                ProcessLog.Instance.Debug(String.Empty.PadLeft(10, '=')); // 我是分隔線
            }
        }

        public static string Mode
        {
            get
            {
                var value = ConfigurationManager.AppSettings.TryGet("Mode", false);
                value = value ?? String.Empty;

                switch (value.ToLower().Trim())
                {
                    case "debug":
                        return value;
                    default:
                        return String.Empty;
                }
            }
        }
        /// <summary>
        /// 取得 Config 所設定的 IIS Logs 存放路徑
        /// </summary>
        public static string IISLogPath
        {
            get
            {
                var path = ConfigurationManager.AppSettings.TryGet("IIS_Log_Path");

                if (System.IO.Directory.Exists(path) == false)
                {
                    throw new Exception($"IIS_Log_Path 所設定的路徑 ({path}) 不存在");
                }

                return path;
            }
        }

        /// <summary>
        /// 取得 Config 所設定的 Logs 保留天數
        /// </summary>
        public static int? PreserveDays
        {
            get
            {
                var value = ConfigurationManager.AppSettings.TryGet("PreserveDays", false);

                var days = ConstDefaultSetting.DEFAULT_LOGS_PRESERVE_DAYS;

                Int32.TryParse(value, out days);

                return days;
            }
        }

        /// <summary>
        /// 取得 Config 所設定的刪除前保留區路徑
        /// </summary>
        /// <remarks>
        /// 在 log 已經超過保留天數後，將會先移動到此設定值所只定的路徑
        /// 保留區只存放 60 天，在保留區放置超過 60 天後，一律會進行實體刪除
        /// 如果不想進行保留，則此設定只保留空白即可
        /// </remarks>
        public static string BackupStagePath
        {
            get
            {
                var path = ConfigurationManager.AppSettings.TryGet("BackupStagePath", false);

                if (String.IsNullOrEmpty(path) == false &&
                    System.IO.Directory.Exists(path) == false)
                {
                    throw new Exception($"BackupStagePath 所設定的路徑 ({path}) 不存在");
                }
                
                return path ?? String.Empty;
            }
        }
    }
}
