using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TopVisor.Core.Services;

namespace TopVisor.Core.Tests
{
    [TestClass]
    public class APITests
    {
        [TestMethod]
        public async Task DeleteAllProjects()
        {
            await RemoveAll();
            var projects = await APIService.GetProjects();
            Assert.AreEqual(projects.Count, 0);
        }

        private static async Task RemoveAll()
        {
            var projects = await APIService.GetProjects();
            foreach (var project in projects)
            {
                await APIService.DeleteProject(project.id);
            }
        }

        [TestMethod]
        public async Task AddProject()
        {
            await RemoveAll();

            var projectName = "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var addedProject = await APIService.GetProject(newProjectId);
            Assert.AreEqual(addedProject.id, newProjectId);
            Assert.AreEqual(addedProject.name, projectName);
            Assert.AreEqual(addedProject.site, projectSite);
        }

        [TestMethod]
        public async Task DeleteProject()
        {
            await RemoveAll();

            var projectName = "test project";
            var projectSite = "test.ru";

            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;
            var projects = await APIService.GetProjects();
            Assert.AreEqual(projects.Count, 1);

            await APIService.DeleteProject(newProjectId);
            projects = await APIService.GetProjects();
            Assert.AreEqual(projects.Count, 0);
        }
        [TestMethod]
        public async Task EditProject()
        {
            await RemoveAll();

            var projectName = "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var newName = "test project 1";
            await APIService.EditProject(newProjectId, newName, projectSite);
            var afterEdit = await APIService.GetProject(newProjectId);
            Assert.AreEqual(afterEdit.name, newName);
        }

        [TestMethod]
        public async Task DeleteProjectWithBadId()
        {
            var error = false;
            try
            {
                await RemoveAll();

                var projectName = "test project";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                await APIService.DeleteProject(newProjectId+1);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error); // должны ошибку словить, иначе фейл
        }
        [TestMethod]
        public async Task EditProjectWithBadId()
        {
            var error = false;
            try
            {
                await RemoveAll();

                var projectName = "test project";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                await APIService.EditProject(newProjectId+1, projectName, projectSite);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error); // должны ошибку словить, иначе фейл
        }
        [TestMethod]
        public async Task AddProjectWithBadSite()
        {
            var error = false;
            try
            {
                await RemoveAll();

                var projectName = "test project";
                var projectSite = "test.baddomain";
                await APIService.AddProject(projectName, projectSite);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error); // должны ошибку словить, иначе фейл
        }
        [TestMethod]
        public async Task AddProjectWithEmptyName()
        {
            await RemoveAll();

            var projectName = "";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result??0;

            var addedProject = await APIService.GetProject(newProjectId);
            Assert.AreEqual(addedProject.id, newProjectId);
            Assert.AreEqual(addedProject.name, projectSite);
            Assert.AreEqual(addedProject.site, projectSite);
        }

        [TestMethod]
        public async Task AddPhrase()
        {
            await RemoveAll();

            var projectName = "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var phraseText = "test phrase";
            await APIService.AddPhrase(newProjectId, phraseText);

            var phrases = await APIService.GetPhrases(newProjectId);
            Assert.AreEqual(phrases.Count,1);
            Assert.AreEqual(phrases[0].phrase, phraseText);
        }
        [TestMethod]
        public async Task DeletePhrase()
        {
            await RemoveAll();

            var projectName = "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var phraseText = "test phrase";
            var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;

            var phrases = await APIService.GetPhrases(newProjectId);
            Assert.AreEqual(phrases.Count, 1);

            await APIService.DeletePhrase(newProjectId, newPhraseId);
            phrases = await APIService.GetPhrases(newProjectId);
            Assert.AreEqual(phrases.Count,0);
        }
        [TestMethod]
        public async Task EditPhrase()
        {
            await RemoveAll();

            var projectName = "test project";
            var projectSite = "test.ru";
            var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

            var phraseText = "test phrase";
            var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;

            var newText = "new test phrase";
            await APIService.EditPhrase(newProjectId, newPhraseId, newText);

            var phrases = await APIService.GetPhrases(newProjectId);
            Assert.AreEqual(phrases.Count, 1);
            Assert.AreEqual(phrases[0].phrase, newText);
        }
        [TestMethod]
        public async Task AddPhraseToBadProject()
        {
            var error = false;
            try
            {
                await RemoveAll();

                var projectName = "test project";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                await APIService.AddPhrase(newProjectId+1, phraseText);

            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error); // должны ошибку словить, иначе фейл
        }
        [TestMethod]
        public async Task DeletePhraseFromBadProject()
        {
            var error = false;
            try
            {
                await RemoveAll();

                var projectName = "test";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;
                await APIService.DeletePhrase(newProjectId+1, newPhraseId);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error); // должны ошибку словить, иначе фейл
        }

        [TestMethod]
        public async Task EditPhraseInBadProject()
        {
            var error = false;
            try
            {
                await RemoveAll();

                var projectName = "test";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;

                var newText = "new test phrase";
                await APIService.EditPhrase(newProjectId+1, newPhraseId, newText);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error); // должны ошибку словить, иначе фейл
        }
        [TestMethod]
        public async Task DeletePhraseWithBadId()
        {
            var error = false;
            try
            {
                await RemoveAll();

                var projectName = "test";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;
                await APIService.DeletePhrase(newProjectId, newPhraseId+1);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error); // должны ошибку словить, иначе фейл
        }

        [TestMethod]
        public async Task EditPhraseWithBadId()
        {
            var error = false;
            try
            {
                await RemoveAll();

                var projectName = "test";
                var projectSite = "test.ru";
                var newProjectId = (await APIService.AddProject(projectName, projectSite)).result ?? 0;

                var phraseText = "test phrase";
                var newPhraseId = (await APIService.AddPhrase(newProjectId, phraseText)).result ?? 0;

                var newText = "new test phrase";
                await APIService.EditPhrase(newProjectId, newPhraseId+1, newText);
            }
            catch (Exception)
            {
                error = true;
            }
            Assert.IsTrue(error); // должны ошибку словить, иначе фейл
        }
        [TestMethod]
        public async Task AddDuplicatePhrase()
        {
            var error = false;
            try
            {
                await RemoveAll();

                var projectName = "test";
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
            Assert.IsTrue(error); // должны ошибку словить, иначе фейл
        }

        [TestMethod]
        public async Task EditDuplicatePhrase()
        {
            var error = false;
            try
            {
                await RemoveAll();

                var projectName = "test";
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
            Assert.IsTrue(error); // должны ошибку словить, иначе фейл
        }
    }
}
