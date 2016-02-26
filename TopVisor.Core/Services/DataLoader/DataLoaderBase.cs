using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TopVisor.Core.Model;

namespace TopVisor.Core.Services.DataLoader
{
    public abstract class DataLoaderBase : IDataLoader
    {
        protected DataLoaderBase()
        {
            IsLoaded = false;
            Data = new TopVisorData();
        }

        public TopVisorData Data { get; private set; }
        public bool IsLoaded { get; private set; }

        public async Task<TopVisorData> LoadAll()
        {
            IsLoaded = false;
            Data = new TopVisorData();

            var projects = await LoadProjects();
            Data.Projects.AddRange(projects);
            foreach (var project in projects)
            {
                var phrases = String.IsNullOrWhiteSpace(project.Name)
                    ? await LoadPhrases(project.Id)
                    : await LoadPhrases(project.Name);
                project.Phrases.AddRange(phrases);
            }

            IsLoaded = true;
            return Data;
        }

        public abstract Task<List<Project>> LoadProjects();

        public abstract Task<List<Phrase>> LoadPhrases(uint projectId);

        public abstract Task<List<Phrase>> LoadPhrases(string projectName);
    }
}