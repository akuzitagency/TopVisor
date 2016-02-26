using System;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using TopVisor.Core.Model.DTO;
using TopVisor.Core.Services;

namespace TopVisor.Shell.ViewModels
{
    public class TestAPIViewModel :BindableBase
    {
        private string _errorMessage;
        private string _newProjectSite;
        private string _newProjectName;
        private ProjectDTO _selectedProject;
        private string _newPhrase;
        private PhraseDTO _selectedPhrase;

        public TestAPIViewModel()
        {
            ErrorMessage = null;

            Projects = new ObservableCollection<ProjectDTO>();
            Phrases=new ObservableCollection<PhraseDTO>();

            GetProjects = new DelegateCommand(DoGetProjects);
            AddProject = new DelegateCommand(DoAddProject);
            EditProject = new DelegateCommand(DoEditProject);
            DeleteProject = new DelegateCommand(DoDeleteProject);

            GetPhrases = new DelegateCommand(DoGetPhrases);
            AddPhrase = new DelegateCommand(DoAddPhrase);
            EditPhrase = new DelegateCommand(DoEditPhrase);
            DeletePhrase = new DelegateCommand(DoDeletePhrase);
        }

        private async void DoDeletePhrase()
        {
            if (SelectedPhrase==null) return;

            ErrorMessage = null;
            try
            {
                var serverResponce = APIService.DeletePhrase(SelectedPhrase.project_id, SelectedPhrase.phrase_id);
                await serverResponce;

                await GetPhrases.Execute();
            }
            catch (Exception exception)
            {
                ErrorMessage = "Ошибк случилсо:" + Environment.NewLine + exception.Message;
            }
        }

        private async void DoEditPhrase()
        {
            if (SelectedPhrase == null) return;

            ErrorMessage = null;
            try
            {
                var serverResponce = APIService.EditPhrase(SelectedPhrase.project_id, SelectedPhrase.phrase_id, SelectedPhrase.phrase);
                await serverResponce;

                await GetPhrases.Execute();
            }
            catch (Exception exception)
            {
                ErrorMessage = "Ошибк случилсо:" + Environment.NewLine + exception.Message;
            }
        }

        private async void DoGetPhrases()
        {
            if (SelectedProject == null) return;

            ErrorMessage = null;
            Phrases.Clear();
            try
            {
                var serverResponce = APIService.GetPhrases(SelectedProject.id);
                await serverResponce;
                var result = serverResponce.Result;
                result.ForEach(k => Phrases.Add(k));
            }
            catch (Exception exception)
            {
                ErrorMessage = "Ошибк случилсо:" + Environment.NewLine + exception.Message;
            }
        }

        private async void DoAddPhrase()
        {
            if (SelectedProject == null || String.IsNullOrWhiteSpace(NewPhrase)) return;

            ErrorMessage = null;
            try
            {
                var serverResponce = APIService.AddPhrase(SelectedProject.id, NewPhrase);
                await serverResponce;

                await GetPhrases.Execute();
            }
            catch (Exception exception)
            {
                ErrorMessage = "Ошибк случилсо:" + Environment.NewLine + exception.Message;
            }
        }

        private async void DoDeleteProject()
        {
            if (SelectedProject == null) return;

            ErrorMessage = null;
            try
            {
                var serverResponce = APIService.DeleteProject(SelectedProject.id);
                await serverResponce;

                await GetProjects.Execute();
            }
            catch (Exception exception)
            {
                ErrorMessage = "Ошибк случилсо:" + Environment.NewLine + exception.Message;
            }
        }

        private async void DoEditProject()
        {
            if (SelectedProject == null) return;

            ErrorMessage = null;
            try
            {
                var serverResponce = APIService.EditProject(SelectedProject.id, SelectedProject.name, SelectedProject.site);
                await serverResponce;

                await GetProjects.Execute();
            }
            catch (Exception exception)
            {
                ErrorMessage = "Ошибк случилсо:" + Environment.NewLine + exception.Message;
            }
        }

        private async void DoAddProject()
        {
            ErrorMessage = null;
            try
            {
                var serverResponce = APIService.AddProject(NewProjectName, NewProjectSite);
                await serverResponce;

                await GetProjects.Execute();
            }
            catch (Exception exception)
            {
                ErrorMessage = "Ошибк случилсо:" + Environment.NewLine + exception.Message;
            }
        }

        private async void DoGetProjects()
        {
            ErrorMessage = null;
            Projects.Clear();
            try
            {
                var serverResponce = APIService.GetProjects();
                await serverResponce;
                var result = serverResponce.Result;
                result.ForEach(p=>Projects.Add(p));
            }
            catch (Exception exception)
            {
                ErrorMessage= "Ошибк случилсо:" + Environment.NewLine + exception.Message;
            }
        }

        public ObservableCollection<ProjectDTO> Projects { get; private set; }
        public ObservableCollection<PhraseDTO> Phrases { get; private set; }

        public ProjectDTO SelectedProject
        {
            get { return _selectedProject; }
            set { _selectedProject = value; OnPropertyChanged(()=>SelectedProject);}
        }

        public PhraseDTO SelectedPhrase
        {
            get { return _selectedPhrase; }
            set { _selectedPhrase = value; OnPropertyChanged(()=>SelectedPhrase);}
        }

        public DelegateCommand GetProjects { get; } 
        public DelegateCommand AddProject { get;  }
        public DelegateCommand EditProject { get; }
        public DelegateCommand DeleteProject { get; }

        public DelegateCommand GetPhrases { get;  }
        public DelegateCommand AddPhrase { get; }
        public DelegateCommand EditPhrase { get;  }
        public DelegateCommand DeletePhrase { get;  }

        public String NewProjectSite
        {
            get { return _newProjectSite; }
            set { _newProjectSite = value; OnPropertyChanged(()=>NewProjectSite);}
        }

        public String NewProjectName
        {
            get { return _newProjectName; }
            set { _newProjectName = value; OnPropertyChanged(()=>NewProjectName);}
        }

        public String NewPhrase
        {
            get { return _newPhrase; }
            set { _newPhrase = value; OnPropertyChanged(()=>NewPhrase);}
        }

        public String ErrorMessage
        {
            get { return _errorMessage; }
            private set
            {
                _errorMessage = value; OnPropertyChanged(()=>ErrorMessage);
                HasError = !String.IsNullOrWhiteSpace(_errorMessage); OnPropertyChanged(()=>HasError);
            }
        }

        public Boolean HasError { get; private set; }
    }
}