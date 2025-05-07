using GraphicPanels;
using GraphicPanelWpf;
using System.Windows;

namespace JumpingStick
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Attention: The following must be added to the JumpingStick.csproj so that you can
            //use the DrawingPanel-object: <UseWindowsForms>true</UseWindowsForms>
            var panel = new DrawingPanel.DrawingPanel(100, 100);
            this.graphicControlBorder.Child = new DrawingPanel.GraphicControl(panel); //The GraphicPanel2D-object is used by the view and viewmodel

            this.DataContext = new ViewModel(panel); //the GraphicPanel2D-object is used by the viewmodel to send drawing commands
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);

            window.KeyDown += (this.DataContext as ViewModel).HandleKeyDown;
            window.KeyUp += (this.DataContext as ViewModel).HandleKeyUp;
        }
    }
}