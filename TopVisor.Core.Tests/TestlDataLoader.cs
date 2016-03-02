using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopVisor.Core.Model;
using TopVisor.Core.Services.DataLoader;

namespace TopVisor.Core.Tests
{
    public class TestlDataLoader : DataLoaderBase
    {
        private string _projectNamePrefix;
        private uint _projectsCount;
        private uint _phrasesPerProject;

        public TestlDataLoader(String projectNamePrefix, UInt32 projectsCount, UInt32 phrasesPerProject)
        {
            _phrasesPerProject = phrasesPerProject;
            _projectsCount = projectsCount;
            _projectNamePrefix = projectNamePrefix;
        }

        public override async Task<List<Project>> LoadProjects()
        {
            var result = new List<Project>();
            for (UInt32 i = 1; i <= _projectsCount; i++)
            {
                result.Add(new Project(i, _projectNamePrefix + "test" + i, "test" + i + ".ru"));
            }
            return result;
        }

        public override async Task<List<Phrase>> LoadPhrases(uint projectId)
        {
            var result = new List<Phrase>();
            var project = Data.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project == null) throw new ArgumentNullException(nameof(project));
            for (UInt32 i = 1; i <= _phrasesPerProject; i++)
            {
                result.Add(new Phrase (i,"phrase " + i + " in project " + projectId));
            }
            return result;
        }

        public override async Task<List<Phrase>> LoadPhrases(string projectName)
        {
            var project = Data.Projects.FirstOrDefault(p => p.Name == projectName);
            if (project == null) throw new ArgumentException("Незвестный проект \"" + projectName + "\"", projectName);
            return await LoadPhrases(project.Id);
        }
    }
}