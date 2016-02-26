using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopVisor.Core.Model;

namespace TopVisor.Core.Services.DataLoader
{
    public class APIDataLoader : DataLoaderBase
    {
        public override async Task<List<Project>> LoadProjects()
        {
            var result = new List<Project>();

            var projects = await APIService.GetProjects();
            projects.ForEach(project=>result.Add(new Project (project.id, project.name, project.site)));

            return result;
        }

        public override async Task<List<Phrase>> LoadPhrases(uint projectId)
        {
            var result = new List<Phrase>();

            var phrases = await APIService.GetPhrases(projectId);
            phrases.ForEach(phrase => result.Add(new Phrase(phrase.phrase_id, phrase.phrase)));

            return result;
        }

        public override async Task<List<Phrase>> LoadPhrases(string projectName)
        {
            var project = Data.Projects.FirstOrDefault(p => p.Name == projectName);
            if(project==null) throw new ArgumentException("Незвестный проект \"" + projectName + "\"", projectName);
            return await LoadPhrases(project.Id);
        }
    }
}