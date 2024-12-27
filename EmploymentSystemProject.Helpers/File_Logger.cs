using EmploymentSystemProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Helpers
{
    public static class File_Logger
    {
        public static readonly Func<string> LogPathSetter;

        private static readonly List<string> logFilePathList;

        private static readonly object lockObj;

        private static string ConfigurationLogPath { get; set; }

        static File_Logger()
        {
            logFilePathList = new List<string>();
            lockObj = new object();
            SetConfigurationPath();
        }

        public static void WriteToLogFile(Exception exception, string exceptionDetail, string subFolderLogName = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            WriteToLogFile(ActionType.Exception, GetStackTraceLine(memberName, sourceFilePath, sourceLineNumber), exceptionDetail + Environment.NewLine + exception.ToString(), subFolderLogName);
        }

        public static void WriteToLogFile(ActionType logAction, string message, string subFolderLogName = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            WriteToLogFile(logAction, GetStackTraceLine(memberName, sourceFilePath, sourceLineNumber), message, subFolderLogName);
        }

        public static void WriteToLogFile(ActionType logAction, string methodName, string message, string subFolderLogName = null)
        {
            Task.Run(delegate
            {
                try
                {
                    string text = Path.Combine((!string.IsNullOrEmpty(ConfigurationLogPath)) ? ConfigurationLogPath : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"), DateTime.Now.ToString("yyyy-MM-dd"));
                    if (!string.IsNullOrWhiteSpace(subFolderLogName))
                    {
                        text = Path.Combine(text, subFolderLogName);
                    }

                    if (!Directory.Exists(text))
                    {
                        Directory.CreateDirectory(text);
                    }

                    string text2 = Path.Combine(text, DateTime.Now.ToString("yyyy-MM-dd HH") + ((logAction == ActionType.Exception) ? "_Exception" : string.Empty) + ".txt");
                    AddFilePathIfNotExist(text2);
                    lock (lockObj)
                    {
                        BackupLogFileWhenSizeExceeded(text2);
                        using StreamWriter streamWriter = new StreamWriter(text2, append: true);
                        streamWriter.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " || " + methodName + " || " + logAction.ToString().PadRight(11) + " || " + message);
                        if (logAction == ActionType.Exception)
                        {
                            streamWriter.WriteLine(Environment.NewLine + "==============================================" + Environment.NewLine);
                        }
                    }
                }
                catch
                {
                }
            });
        }

        private static void AddFilePathIfNotExist(string filePath)
        {
            if (logFilePathList.Contains(filePath))
            {
                return;
            }

            lock (logFilePathList)
            {
                if (logFilePathList.Count > 50)
                {
                    logFilePathList.RemoveAt(logFilePathList.Count - 1);
                }

                logFilePathList.Insert(0, filePath);
            }
        }

        private static void BackupLogFileWhenSizeExceeded(string filePath)
        {
            try
            {
                if (File.Exists(filePath) && File.ReadAllBytes(filePath).Length > 5242880)
                {
                    File.Move(filePath, filePath.Replace(".txt", "_" + DateTime.Now.ToString("HHmmssfff") + ".txt"));
                }
            }
            catch
            {
            }
        }

        private static void SetConfigurationPath()
        {
            ConfigurationLogPath = LogPathSetter?.Invoke();
        }

        private static string GetStackTraceLine(string memberName, string sourceFilePath, int sourceLineNumber)
        {
            return ((!string.IsNullOrWhiteSpace(sourceFilePath)) ? (Path.GetFileName(sourceFilePath) + ", ") : "") + memberName + ":" + sourceLineNumber;
        }
    }
}
