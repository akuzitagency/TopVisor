using System;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using TopVisor.Core.Services;
using TopVisor.Core.Services.DataLoader;

namespace TopVisor.Synchronization
{
    class Program
    {
        static void Main(string[] args)
        {
            var logWriter = new LogWriterFactory().Create();
            try
            {
                var localDataPath = (args?.Length >= 1) ? args[0] : "LocalData.xml";

                var sourceTask = new XMLDataLoader(localDataPath).LoadAll();
                var destTask = new APIDataLoader().LoadAll();
                Task.WaitAll(sourceTask, destTask);

                var sync = new SynchronizationService
                {
                    SourceData = sourceTask.Result,
                    DestData = destTask.Result
                };
                var syncTask = sync.Synchronize();
                Task.WaitAll(syncTask);

#if DEBUG
                Console.WriteLine("Синхронизация завершена");
                Console.ReadKey();
#endif
            }
            catch (Exception e)
            {
                logWriter.Write(e, new[] {"Errors"});
#if DEBUG
                Console.WriteLine("Ошибка:");
                Console.WriteLine(e);
                Console.ReadKey();
#endif
            }
        }
    }
}
