using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace Embodied
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _path = "null";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateFileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    FileName = "New File.txt",
                    DefaultExt = "txt",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Title = "Save As"
                };

                if(dialog.ShowDialog() == true)
                {
                    string selectedPath = dialog.FileName;
                    File.Create(selectedPath);
                }
                statusBar.Text = "File is open";
            }
            catch(Exception ex)
            {
                statusBar.Text = $"Error creating file: {ex.Message}";
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_path != "null")
                {
                    TextRange range = new(Input.Document.ContentStart, Input.Document.ContentEnd);

                    using FileStream fs = new(_path, FileMode.Open, FileAccess.Write);
                    range.Save(fs, DataFormats.Rtf);

                    statusBar.Text = "Save complete";
                }
                else
                {
                    CreateFileClick(sender, e);
                }
            }
            catch (Exception ex)
            {
                statusBar.Text = $"Error loading file: {ex.Message}";
            }
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            try
            {
                TextRange range = new(Input.Document.ContentStart, Input.Document.ContentEnd);

                OpenFileDialog ofd = new()
                {
                    Filter = "All Files (*.*)|*.*|ReachText Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt"
                };
                Nullable<bool> result = ofd.ShowDialog();

                if (result == true)
                {
                    _path = ofd.FileName;
                }

                using FileStream fs = new(_path, FileMode.Open, FileAccess.Read);
                range.Load(fs, DataFormats.Rtf);

                statusBar.Text = "Open complete";
            }
            catch(Exception ex)
            {
                statusBar.Text = $"Error loading file: {ex.Message}";
            }
        }

        private void ApplicationClose(object sender, RoutedEventArgs e)
        {
            statusBar.Text = "Closing...";
            Environment.Exit(0);
        }

        private void WindowedClick(object sender, RoutedEventArgs e)
        {
            var grid = (Grid)FindName("MainGrid");
            if(grid == null) return;

            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    this.WindowState = WindowState.Normal;
                    grid.Margin = new Thickness(0);
                    break;
                case WindowState.Normal:
                    this.WindowState = WindowState.Maximized;
                    grid.Margin = new Thickness(7);
                    break;
            }

            statusBar.Text = "Ready";
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.Opacity = 0.6;
                try
                {
                    this.DragMove();
                }
                catch (InvalidOperationException)
                { }

                this.Opacity = 1;
            }
        }
    }
}