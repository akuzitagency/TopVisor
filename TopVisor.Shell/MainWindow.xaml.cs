using System;
using System.Windows;
using TopVisor.Core.Services;
using TopVisor.Core.Services.DataLoader;
using TopVisor.Shell.ViewModels;

namespace TopVisor.Shell
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TestAPIViewModel();
        }

        private void TestAPI_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new TestAPI().Show();
        }

        private void TestAPILoader_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new TestAPILoader().Show();
        }

        private void TestXMLLoader_Click(object sender, RoutedEventArgs e)
        {
            new TestXMLLoader().Show();
        }

        private async void TestSynchronization_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sourceDataLoader = new XMLDataLoader();
                await sourceDataLoader.LoadAll();
                var destDataLoader = new APIDataLoader();
                await destDataLoader.LoadAll();

                var syncService = new SynchronizationService
                {
                    SourceData = sourceDataLoader.Data,
                    DestData = destDataLoader.Data
                };
                await syncService.Synchronize();

                MessageBox.Show("Complete");
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}