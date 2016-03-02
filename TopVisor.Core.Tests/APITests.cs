using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TopVisor.Core.Model.DTO;
using TopVisor.Core.Services;

namespace TopVisor.Core.Tests
{
    [TestClass]
    public class APITests
    {
        private const String APITestProjectNamePrefix = "_test_API_";
//        private static readonly Dictionary<String, Object> TestFilter = new Dictionary<string, object> { {"name", APITestProjectNamePrefix }};

        private static async Task RemoveAll()
        {
            var projects = await GetTestProjects();
            foreach (var project in projects)
            {
                await APIService.DeleteProject(project.id);
            }
        }

        private static async Task<List<ProjectDTO>> GetTestProjects()
        {
//            return await APIService.GetProjects(TestFilter);
            return (await APIService.GetProjects()).Where(p => p.name.StartsWith(APITestProjectNamePrefix)).ToList();
        }

        [TestMethod]
        public async Task AddProject()
        {
            await RemoveAll();

            var projectName = APITestProjectNamePrefix + "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var addedProject = await APIService.GetProject(newProjectId);
            Assert.AreEqual(addedProject.id, newProjectId, "ID");
            Assert.AreEqual(addedProject.name, projectName, "Name");
            Assert.AreEqual(addedProject.site, projectSite, "Site");

            await RemoveAll();
        }

        [TestMethod]
        public async Task DeleteProject()
        {
            await RemoveAll();

            var projectName = APITestProjectNamePrefix + "test project";
            var projectSite = "test.ru";

            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;
            var projects = await GetTestProjects();
            Assert.AreEqual(projects.Count, 1, "Count!=1");

            await APIService.DeleteProject(newProjectId);
            projects = await GetTestProjects();
            Assert.AreEqual(projects.Count, 0, "Count!=0");

            await RemoveAll();
        }

        [TestMethod]
        public async Task EditProject()
        {
            await RemoveAll();

            var projectName = APITestProjectNamePrefix + "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var newName = APITestProjectNamePrefix + "test project 1";
            await APIService.EditProject(newProjectId, newName, projectSite);
            var afterEdit = await APIService.GetProject(newProjectId);
            Assert.AreEqual(afterEdit.name, newName, "Name");

            await RemoveAll();
        }

        [TestMethod]
        public async Task DeleteProjectWithBadId()
        {
            await RemoveAll();

            var error = false;
            try
            {
                var projectName = APITestProjectNamePrefix + "test project";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                await APIService.DeleteProject(newProjectId + 1);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error, "Exception expected"); // должны ошибку словить, иначе фейл

            await RemoveAll();
        }

        [TestMethod]
        public async Task EditProjectWithBadId()
        {
            await RemoveAll();

            var error = false;
            try
            {
                var projectName = APITestProjectNamePrefix + "test project";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                await APIService.EditProject(newProjectId + 1, projectName, projectSite);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error, "Exception expected"); // должны ошибку словить, иначе фейл

            await RemoveAll();
        }

        [TestMethod]
        public async Task AddProjectWithBadSite()
        {
            await RemoveAll();

            var error = false;
            try
            {
                var projectName = APITestProjectNamePrefix + "test project";
                var projectSite = "test.baddomain";
                await APIService.AddProject(projectName, projectSite);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error, "Exception expected"); // должны ошибку словить, иначе фейл

            await RemoveAll();
        }

        [TestMethod]
        public async Task AddProjectWithEmptyName()
        {
            await RemoveAll();

            var projectName = "";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var addedProject = await APIService.GetProject(newProjectId);
            Assert.AreEqual(addedProject.id, newProjectId, "ID");
            Assert.AreEqual(addedProject.name, projectSite, "Name");
            Assert.AreEqual(addedProject.site, projectSite, "Site");

            await APIService.DeleteProject(newProjectId);

            await RemoveAll();
        }

        [TestMethod]
        public async Task AddPhrase()
        {
            await RemoveAll();

            var projectName = APITestProjectNamePrefix + "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var phraseText = "test phrase";
            await APIService.AddPhrase(newProjectId, phraseText);

            var phrases = await APIService.GetPhrases(newProjectId);
            Assert.AreEqual(phrases.Count, 1, "Count!=1");
            Assert.AreEqual(phrases[0].phrase, phraseText, "Phrase");

            await RemoveAll();
        }

        [TestMethod]
        public async Task DeletePhrase()
        {
            await RemoveAll();

            var projectName = APITestProjectNamePrefix + "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var phraseText = "test phrase";
            var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;

            var phrases = await APIService.GetPhrases(newProjectId);
            Assert.AreEqual(phrases.Count, 1);

            await APIService.DeletePhrase(newProjectId, newPhraseId);
            phrases = await APIService.GetPhrases(newProjectId);
            Assert.AreEqual(phrases.Count, 0, "Count!=0");

            await RemoveAll();
        }

        [TestMethod]
        public async Task EditPhrase()
        {
            await RemoveAll();

            var projectName = APITestProjectNamePrefix + "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var phraseText = "test phrase";
            var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;

            var newText = "new test phrase";
            await APIService.EditPhrase(newProjectId, newPhraseId, newText);

            var phrases = await APIService.GetPhrases(newProjectId);
            Assert.AreEqual(phrases.Count, 1, "Count!=1");
            Assert.AreEqual(phrases[0].phrase, newText, "Phrase");

            await RemoveAll();
        }

        [TestMethod]
        public async Task AddPhraseToBadProject()
        {
            await RemoveAll();

            var error = false;
            try
            {
                var projectName = APITestProjectNamePrefix + "test project";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                await APIService.AddPhrase(newProjectId + 1, phraseText);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error, "Exception expected"); // должны ошибку словить, иначе фейл

            await RemoveAll();
        }

        [TestMethod]
        public async Task DeletePhraseFromBadProject()
        {
            await RemoveAll();

            var error = false;
            try
            {
                var projectName = APITestProjectNamePrefix + "test";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;
                await APIService.DeletePhrase(newProjectId + 1, newPhraseId);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error, "Exception expected"); // должны ошибку словить, иначе фейл

            await RemoveAll();
        }

        [TestMethod]
        public async Task EditPhraseInBadProject()
        {
            await RemoveAll();

            var error = false;
            try
            {
                var projectName = APITestProjectNamePrefix + "test";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;

                var newText = "new test phrase";
                await APIService.EditPhrase(newProjectId + 1, newPhraseId, newText);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error, "Exception expected"); // должны ошибку словить, иначе фейл

            await RemoveAll();
        }

        [TestMethod]
        public async Task DeletePhraseWithBadId()
        {
            await RemoveAll();

            var error = false;
            try
            {
                var projectName = APITestProjectNamePrefix + "test";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;
                await APIService.DeletePhrase(newProjectId, newPhraseId + 1);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error, "Exception expected"); // должны ошибку словить, иначе фейл

            await RemoveAll();
        }

        [TestMethod]
        public async Task EditPhraseWithBadId()
        {
            await RemoveAll();

            var error = false;
            try
            {
                var projectName = APITestProjectNamePrefix + "test";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;

                var newText = "new test phrase";
                await APIService.EditPhrase(newProjectId, newPhraseId + 1, newText);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error, "Exception expected"); // должны ошибку словить, иначе фейл

            await RemoveAll();
        }

        [TestMethod]
        public async Task AddDuplicatePhrase()
        {
            await RemoveAll();

            var error = false;
            try
            {
                var projectName = APITestProjectNamePrefix + "test";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                await APIService.AddPhrase(newProjectId, phraseText);
                await APIService.AddPhrase(newProjectId, phraseText);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error, "Exception expected"); // должны ошибку словить, иначе фейл

            await RemoveAll();
        }

        [TestMethod]
        public async Task EditDuplicatePhrase()
        {
            await RemoveAll();

            var error = false;
            try
            {
                var projectName = APITestProjectNamePrefix + "test";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                await APIService.AddPhrase(newProjectId, phraseText);

                var anotherPhraseText = "another phrase";
                var anotherPhraseId = (await APIService.AddPhrase(newProjectId, anotherPhraseText)).result ?? 0;

                await APIService.EditPhrase(newProjectId, anotherPhraseId, phraseText);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error, "Exception expected"); // должны ошибку словить, иначе фейл

            await RemoveAll();
        }

        [TestMethod]
        public async Task SimultaneousCalls()
        {
            await RemoveAll();

            var tasks = new Task[50];
            for (var i = 0; i < 50; i++)
            {
                tasks[i] = APIService.AddProject(APITestProjectNamePrefix + "test" + i, "test.ru");
            }
            Task.WaitAll(tasks);

            await RemoveAll();
        }

        [TestMethod]
        public async Task GetProjectWithPaging()
        {
            await RemoveAll();

            const int projectsCount = 250;
            for (var i = 0; i < projectsCount; i++)
            {
                await APIService.AddProject(APITestProjectNamePrefix + "test" + i, "test.ru");
            }
            var projects = await GetTestProjects();
            Assert.AreEqual(projectsCount, projects.Count, "Не совпадает количество проектов");

            await RemoveAll();
        }

        [TestMethod]
        public async Task GetPhrasesWithPaging()
        {
            await RemoveAll();

            var projectName = APITestProjectNamePrefix + "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var phraseText = "test phrase {0}";

            const int phrasesCount = 150;
            for (var i = 0; i < phrasesCount; i++)
            {
                await APIService.AddPhrase(newProjectId, String.Format(phraseText, i));
            }
            var phrases = await APIService.GetPhrases(newProjectId);
            Assert.AreEqual(phrasesCount, phrases.Count, "Не совпадает количество фраз");

            await RemoveAll();
        }

        [TestMethod]
        public async Task DeleteAllProjects()
        {
            await RemoveAll();
            var projects = await GetTestProjects();
            Assert.AreEqual(projects.Count, 0, "Count!=0");
        }
    }
}