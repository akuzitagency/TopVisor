using TopVisor.Shell.ViewModels;

namespace TopVisor.Shell
{
    /// <summary>
    /// Interaction logic for TestAPI.xaml
    /// </summary>
    public partial class TestAPI 
    {
        public TestAPI()
        {
            InitializeComponent();
            DataContext = new TestAPIViewModel();
        }
    }
}
