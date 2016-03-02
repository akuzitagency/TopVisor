using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TopVisor.Core.Services.DataLoader;

namespace TopVisor.Shell
{
    /// <summary>
    /// Interaction logic for TestXMLLoader.xaml
    /// </summary>
    public partial class TestXMLLoader : Window
    {
        public TestXMLLoader()
        {
            InitializeComponent();
        }

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            ItemsView.DataContext = null;

            var loader = new XMLDataLoader("LocalData.xml");
            await loader.LoadAll();
            var data = loader.Data;

            ItemsView.DataContext = data;
        }
    }
}
