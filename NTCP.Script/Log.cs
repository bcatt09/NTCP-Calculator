using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace NTCP
{
    public static class Log
    {
        private static string LogName = "ESAPIScripts";
        private static string DefaultLogFileName = LogName + ".log";

        public static void Initialize(ScriptContext context)
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = GetDefaultLogPath(),
                Layout = "${date:format=HH\\:mm\\:ss:padding=-10:fixedlength=true} ${gdc:item=Script:padding=-20:fixedlength=true} ${level:uppercase=true:padding=-10:fixedlength=true} ${gdc:item=User:padding=-35:fixedlength=true} ${gdc:item=Patient:padding=-35:fixedlength=true} ${message}${onexception:${newline}  ${exception:format=Message,StackTrace:separator=\r\n}}"
            };

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;

            GlobalDiagnosticsContext.Set("Script", "NTCP-Calculator");
            GlobalDiagnosticsContext.Set("User", $"{context.CurrentUser.Name} ({context.CurrentUser.Id})");
            GlobalDiagnosticsContext.Set("Patient", $"{context.Patient.LastName}, {context.Patient.FirstName} ({context.Patient.Id})");

            // Clear the log every day and save yesterday's log in case there were errors that need to be looked into
            if (File.Exists(GetDefaultLogPath()) && DateTime.Now.Day != File.GetLastWriteTime(GetDefaultLogPath()).Day)
            {
                File.Delete(GetOldLogPath());
                File.Copy(GetDefaultLogPath(), GetOldLogPath());
                File.Delete(GetDefaultLogPath());
            }
        }

        private static string GetDefaultLogPath()
        {
            return Path.Combine(GetAssemblyDirectory(), DefaultLogFileName);
        }

        private static string GetOldLogPath()
        {
            return Path.Combine(GetAssemblyDirectory(), LogName + ".old.log");
        }

        private static string GetAssemblyDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}