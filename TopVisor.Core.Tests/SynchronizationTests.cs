using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TopVisor.Core.Model;
using TopVisor.Core.Services;
using TopVisor.Core.Services.DataLoader;

namespace TopVisor.Core.Tests
{
    [TestClass]
    public class SynchronizationTests
    {
        private const String SynchronizationTestProjectNamePrefix = "_test_Synchronization_";
        private IDataLoader _checkResultsDataLoader;
        private IDataLoader _destDataLoader;
        private IDataLoader _sourceDataLoader;
        private readonly SynchronizationService _syncService = new SynchronizationService();

        private async Task PrepareSynchronizationTest()
        {
            if (_sourceDataLoader == null || !_sourceDataLoader.IsLoaded)
            {
                _sourceDataLoader = new TestlDataLoader(SynchronizationTestProjectNamePrefix, 5, 10);
                await _sourceDataLoader.LoadAll();
            }

            _destDataLoader = new APIDataLoader();
            await _destDataLoader.LoadAll();
            RemoveNonTestProjects(_destDataLoader.Data);

            _syncService.SourceData = _sourceDataLoader.Data;
            _syncService.DestData = _destDataLoader.Data;
        }

        private void RemoveNonTestProjects(TopVisorData data)
        {
            var nonTestProjects = data.Projects.Where(p => !p.Name.StartsWith(SynchronizationTestProjectNamePrefix)).ToList();
            foreach (var project in nonTestProjects)
            {
                data.Projects.Remove(project);
            }
        }

        private async Task CheckSyncronizationResults()
        {
            _checkResultsDataLoader = new APIDataLoader();
            await _checkResultsDataLoader.LoadAll();
            RemoveNonTestProjects(_checkResultsDataLoader.Data);

            Assert.AreEqual(_sourceDataLoader.Data.Projects.Count, _checkResultsDataLoader.Data.Projects.Count, "Разное количество проектов");

            foreach (var sourceProject in _sourceDataLoader.Data.Projects)
            {
                var destProjects = _checkResultsDataLoader.Data.Projects.Where(p => p.Name == sourceProject.Name).ToList();

                Assert.IsFalse(destProjects.Count == 0, "Отсутствует проект " + sourceProject.Name);
                Assert.IsFalse(destProjects.Count > 1, "Задублировался проект " + sourceProject.Name);

                var destProject = destProjects[0];
                Assert.AreEqual(sourceProject.Name, destProject.Name, "Отличается Name у проекта " + sourceProject.Name);
                Assert.AreEqual(sourceProject.Site, destProject.Site, "Отличается Site у проекта " + sourceProject.Name);

                var sourcePhrases = sourceProject.Phrases;
                var destPhrases = destProject.Phrases;
                foreach (var sourcePhrase in sourcePhrases)
                {
                    var destPhrasesCount = destPhrases.Count(phr => phr.Text == sourcePhrase.Text);
                    Assert.IsFalse(destPhrasesCount == 0, "Отсутствует фраза \"" + sourcePhrase.Text + "\"" + " в проекте " + sourceProject.Name);
                    Assert.IsFalse(destPhrasesCount > 1, "Задублировалась фраза \"" + sourcePhrase.Text + "\"" + " в проекте " + sourceProject.Name);
                }
                Assert.IsTrue(sourcePhrases.Count == destPhrases.Count, "Разное количество фраз в проекте " + sourceProject.Name);
            }
        }

        [TestMethod]
        public async Task ClearAll()
        {
            await PrepareSynchronizationTest();

            _syncService.SourceData.Projects.Clear();
            await _syncService.Synchronize();
            await CheckSyncronizationResults();

            _sourceDataLoader = null;
        }

        [TestMethod]
        public async Task SimpleSynchronization()
        {
            await PrepareSynchronizationTest();

            // Первоначальная синхронизация
            await _syncService.Synchronize();
            await CheckSyncronizationResults();
        }

        [TestMethod]
        public async Task DeletePhrasesFromProject()
        {
            await PrepareSynchronizationTest();

            // Удалили фразы из существующего проекта
            _syncService.SourceData.Projects[0].Phrases.RemoveAt(0);
            _syncService.SourceData.Projects[0].Phrases.RemoveAt(0);
            await _syncService.Synchronize();
            await CheckSyncronizationResults();
        }

        [TestMethod]
        public async Task AddPhrasesToProject()
        {
            await PrepareSynchronizationTest();

            // Добавили фразы в существующий проект
            _syncService.SourceData.Projects[0].Phrases.Add(new Phrase(1, "asdfasdf"));
            _syncService.SourceData.Projects[0].Phrases.Add(new Phrase(2, "qwerqwer"));
            await _syncService.Synchronize();
            await CheckSyncronizationResults();
        }

        [TestMethod]
        public async Task DeleteProjects()
        {
            await PrepareSynchronizationTest();

            // Удалили проекты
            _syncService.SourceData.Projects.RemoveAt(0);
            _syncService.SourceData.Projects.RemoveAt(0);
            await _syncService.Synchronize();
            await CheckSyncronizationResults();
        }

        [TestMethod]
        public async Task AddEmptyProjects()
        {
            await PrepareSynchronizationTest();

            // Добавили пустые проекты
            _syncService.SourceData.Projects.Add(new Project(1, SynchronizationTestProjectNamePrefix + "test_add_1", "test.ru"));
            _syncService.SourceData.Projects.Add(new Project(1, SynchronizationTestProjectNamePrefix + "test_add_2", "test.ru"));
            await _syncService.Synchronize();
            await CheckSyncronizationResults();
        }

        [TestMethod]
        public async Task AddProjectsWithPhrases()
        {
            await PrepareSynchronizationTest();

            // Добавили проект с фразами
            var newProject = new Project(1, SynchronizationTestProjectNamePrefix + "test_add_3", "test.ru");
            newProject.Phrases.Add(new Phrase(1, "asdfasdf"));
            newProject.Phrases.Add(new Phrase(2, "qwerqwer"));
            _syncService.SourceData.Projects.Add(newProject);
            await _syncService.Synchronize();
            await CheckSyncronizationResults();
        }

        [TestMethod]
        public async Task FullSynchronizationTest()
        {
            await ClearAll();
                       
            await PrepareSynchronizationTest();

            // Первоначальная синхронизация
            await _syncService.Synchronize();
            await CheckSyncronizationResults();

            await PrepareSynchronizationTest();
            // Удалили фразы из существующего проекта
            var sourceData = _syncService.SourceData;
            sourceData.Projects[0].Phrases.RemoveAt(0);
            sourceData.Projects[0].Phrases.RemoveAt(0);
            // Добавили фразы в существующий проект
            sourceData.Projects[0].Phrases.Add(new Phrase(1, "asdfasdf"));
            sourceData.Projects[0].Phrases.Add(new Phrase(2, "qwerqwer"));
            // Удалили проекты
            sourceData.Projects.RemoveAt(1);
            sourceData.Projects.RemoveAt(1);
            // Добавили пустые проекты
            sourceData.Projects.Add(new Project(1, SynchronizationTestProjectNamePrefix + "test_add_1", "test.ru"));
            sourceData.Projects.Add(new Project(1, SynchronizationTestProjectNamePrefix + "test_add_2", "test.ru"));
            // Добавили проект с фразами
            var newProject = new Project(1, SynchronizationTestProjectNamePrefix + "test_add_3", "test.ru");
            newProject.Phrases.Add(new Phrase(1, "asdfasdf"));
            newProject.Phrases.Add(new Phrase(2, "qwerqwer"));
            sourceData.Projects.Add(newProject);

            await _syncService.Synchronize();
            await CheckSyncronizationResults();

            await ClearAll();
        }


    }
}