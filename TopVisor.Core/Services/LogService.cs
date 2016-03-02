using System;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace TopVisor.Core.Services
{
    public static class LogService
    {
        private static readonly LogWriter DefaultLogWriter = new LogWriterFactory().Create();

        public static void Log(String message, IEnumerable<String> categories)
        {
            DefaultLogWriter.Write(message,categories);
        }
    }

    public static class LogCategories
    {
        public static readonly string[] APICategories = { "API" };
        public static readonly string[] APIErrorCategories = { "API", "Errors" };
        public static readonly string[] ErrorCategories = { "Errors" };
        public static readonly string[] GeneralCategories = { "General" };
        public static readonly string[] SynchronizationCategories = { "Synchronization" };
    }
}