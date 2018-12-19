#define DEBUG_MODE_ALL_EXPIRED_ON

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogCleaner
{
    class Program
    {

        static void Main(string[] args)
        {
            var log4netPath = Path.Combine(Directory.GetCurrentDirectory(), "log4net.config");

            ProcessLog.Instance.Init(log4netPath);

            // read setting
            if (Settings.Load() == false)
            {
                // 組態設定值讀取有錯誤
                return;
            }

            var findPath = Settings.IISLogPath;
            var days = Settings.PreserveDays;
            var backupStagePath = Settings.BackupStagePath;

            var files = Directory.GetFiles(findPath, "*.log", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                // 排除萬一保留區設定在 iis_log_path 裡面的檔案
                if (String.IsNullOrEmpty(Settings.BackupStagePath) == false &&
                    file.ToLower().StartsWith(Settings.BackupStagePath.ToLower()))
                {
                    continue;
                }

#if (DEBUG && DEBUG_MODE_ALL_EXPIRED_ON)
                // 強制讓檔案逾期，測試使用
                File.SetLastAccessTime(
                    file,
                    DateTime.Now.AddDays(0 - (Settings.PreserveDays.Value + 1)));
#endif
                var lastAccess = File.GetLastAccessTime(file);

                var periodFromLastAccess = DateTime.Now.Subtract(lastAccess);

                if (periodFromLastAccess.TotalDays > days)
                {
                    try
                    {
                        // file is expired
                        if (String.IsNullOrEmpty(Settings.BackupStagePath))
                        {
                            // 沒有設定保留區，直接刪除
                            File.Delete(file);
                            continue;
                        }

                        // 如果沒有設定保留區的話，前面的流程就會做完實體刪除，然後跳過後面流程繼續處理下一個項目
                        // 因此流程到這裡的話，就是要搬 log 到保留區的處理
                        var filename = Path.GetFileName(file);
                        var destfile = Path.Combine(Settings.BackupStagePath, filename);
                        
                        File.Move(file, destfile);
                        
                        File.SetLastWriteTime(destfile, DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        ProcessLog.Instance.Debug("處理發生錯誤，以下是相關訊息 :");
                        ProcessLog.Instance.Debug($"檔案 : {file}");
                        ProcessLog.Instance.Debug(ex.Message);
                    }
                }
            }

            // 保留區檔案的處理
            if (String.IsNullOrEmpty(Settings.BackupStagePath) == false)
            {
                files = Directory.GetFiles(Settings.BackupStagePath, "*.log", SearchOption.AllDirectories);

                foreach (var file in files)
                {
#if (DEBUG && DEBUG_MODE_ALL_EXPIRED_ON)
                    // 強制讓檔案逾期，測試使用
                    File.SetLastWriteTime(
                        file,
                        DateTime.Now.AddDays(0 - (Settings.PreserveDays.Value + 1)));
#endif
                    var lastWriteTime = File.GetLastWriteTime(file);

                    var periodFromLastWrite = DateTime.Now.Subtract(lastWriteTime);

                    try
                    {
                        if (periodFromLastWrite.TotalDays > 30)
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        ProcessLog.Instance.Debug("處理發生錯誤，以下是相關訊息 :");
                        ProcessLog.Instance.Debug($"檔案 : {file}");
                        ProcessLog.Instance.Debug(ex.Message);
                    }
                }
            }
            
        }
    }
}
