using System.Windows;
using TopVisor.Core.Services;
using TopVisor.Core.Services.DataLoader;

namespace TopVisor.Shell
{
    /// <summary>
    /// Interaction logic for TestAPILoader.xaml
    /// </summary>
    public partial class TestAPILoader 
    {
        public TestAPILoader()
        {
            InitializeComponent();
        }

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            ItemsView.DataContext = null;

            var loader = new APIDataLoader();
            await loader.LoadAll();
            var data = loader.Data;

            ItemsView.DataContext = data;
        }
    }
}
