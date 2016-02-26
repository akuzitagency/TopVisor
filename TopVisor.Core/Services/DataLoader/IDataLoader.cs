using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TopVisor.Core.Model;

namespace TopVisor.Core.Services.DataLoader
{
    public interface IDataLoader
    {
        TopVisorData Data { get; }
        Boolean IsLoaded { get; }
        Task<TopVisorData> LoadAll();
        Task<List<Project>>  LoadProjects();
        Task<List<Phrase>>  LoadPhrases(UInt32 projectId);
        Task<List<Phrase>> LoadPhrases(String projectName);
    }
}