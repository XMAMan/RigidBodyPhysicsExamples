using System.Windows;

namespace Gripper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new ViewModel(this.canvas);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);

            window.KeyDown += (this.DataContext as ViewModel).HandleKeyDown;
            window.KeyUp += (this.DataContext as ViewModel).HandleKeyUp;
        }
    }
}