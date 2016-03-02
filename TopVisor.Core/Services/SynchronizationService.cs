using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopVisor.Core.Model;

namespace TopVisor.Core.Services
{
    public class SynchronizationService
    {
        public TopVisorData SourceData { get; set; }
        public TopVisorData DestData { get; set; }

        public async Task Synchronize()
        {
            LogService.Log("Синхронизация началась", LogCategories.SynchronizationCategories);

            if (SourceData == null) throw new Exception("Нет источника");
            if (DestData == null) throw new Exception("Нет получателя");

            await DeleteExcessProjects();
            await UpdateExistingProjects();
            await AddNewProjects();

            LogService.Log("Синхронизация закончилась", LogCategories.SynchronizationCategories);
        }

        private async Task DeleteExcessProjects()
        {
            var projectsToDelete = DestData.Projects.Where(project => SourceData.Projects.All(pr => pr.Name != project.Name)).ToList();
            foreach (var project in projectsToDelete)
            {
                LogService.Log("Удаление проекта \"" + project.Name + "\"", LogCategories.SynchronizationCategories);
                await APIService.DeleteProject(project.Id);
            }
        }

        private async Task AddNewProjects()
        {
            var projectsToAdd = SourceData.Projects.Where(project => DestData.Projects.All(pr => pr.Name != project.Name)).ToList();
            foreach (var project in projectsToAdd)
            {
                LogService.Log("Добавление проекта \"" + project.Name + "\"", LogCategories.SynchronizationCategories);
                var apiCallResult = await APIService.AddProject(project.Name, project.Site);
                // ReSharper disable once PossibleInvalidOperationException
                await SynchronizePhrases(apiCallResult.result.Value, project.Name, project.Phrases, new List<Phrase>());
            }
        }

        private async Task UpdateExistingProjects()
        {
            var projectPairs =
                from sourceProject in SourceData.Projects
                join destProject in DestData.Projects
                    on sourceProject.Name equals destProject.Name
                select new {SourceProject = sourceProject, DestProject = destProject};

            foreach (var pair in projectPairs)
            {
                // Топвизор не даёт сайт менять, а больше пока ничего не трогаю
//                if(pair.SourceProject.Site!=pair.DestProject.Site)
//                    await APIService.EditProject(pair.DestProject.Id, pair.SourceProject.Name, pair.SourceProject.Site);

                await SynchronizePhrases(pair.DestProject.Id, pair.DestProject.Name, pair.SourceProject.Phrases, pair.DestProject.Phrases);
            }
        }

        private async Task SynchronizePhrases(UInt32 projectId, String projectName, List<Phrase> sourcePhrases, List<Phrase> destPhrases)
        {
            await DeleteExcessPhrases(projectId, projectName, sourcePhrases, destPhrases);
            await AddNewPhrases(projectId, projectName, sourcePhrases, destPhrases);
            await UpdateExistingPhrases(projectId, sourcePhrases, destPhrases);
        }

        private async Task UpdateExistingPhrases(uint projectId, List<Phrase> sourcePhrases, List<Phrase> destPhrases)
        {
            // do nothing
        }

        private async Task AddNewPhrases(UInt32 projectId, String projectName, List<Phrase> sourcePhrases, List<Phrase> destPhrases)
        {
            var phrasesToAdd = sourcePhrases.Where(phrase => destPhrases.All(ph => ph.Text != phrase.Text)).ToList();
            if(phrasesToAdd.Count>0)
                LogService.Log("Добавление фраз в проект \"" + projectName + "\"", LogCategories.SynchronizationCategories);
            foreach (var phrase in phrasesToAdd)
            {
                LogService.Log("Добавление фразы \"" + phrase.Text + "\"", LogCategories.SynchronizationCategories);
                await APIService.AddPhrase(projectId, phrase.Text);
            }
        }

        private async Task DeleteExcessPhrases(UInt32 projectId, String projectName, List<Phrase> sourcePhrases, List<Phrase> destPhrases)
        {
            var phrasesToDelete = destPhrases.Where(phrase => sourcePhrases.All(ph => ph.Text != phrase.Text)).ToList();
            if(phrasesToDelete.Count>0)
                LogService.Log("Удаление фраз из проекта \"" + projectName + "\"", LogCategories.SynchronizationCategories);
            foreach (var phrase in phrasesToDelete)
            {
                LogService.Log("Удаление фразы \"" + phrase.Text + "\"", LogCategories.SynchronizationCategories);
                await APIService.DeletePhrase(projectId, phrase.Id);
            }
        }
    }
}