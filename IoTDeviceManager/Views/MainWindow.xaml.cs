using System.Windows;
using IoTDeviceManager.ViewModel;

namespace IoTDeviceManager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Code-behind is kept minimal following MVVM pattern
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set DataContext to MainViewModel
            DataContext = new MainViewModel();
        }
    }
}