using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using CleanProject.Service;
using CleanProject.Service.Interfaces;
using CleanProject.Service.Models;
using Form = System.Windows.Forms;

namespace CleanProject.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICleanProjectService _service;
        private Notificator _notificatior;

        public MainWindow(ICleanProjectService service, INotificationHelper notificatior)
        {
            InitializeComponent();

            this._service = service;

            var wpfNotificatior = notificatior as Notificator;
            wpfNotificatior.Initialize(this.TextBlockStatus, Dispatcher, true);
            this._notificatior = wpfNotificatior;

            this.Title = CleanProjectService.TITLE;
            this.ZipProject.IsChecked = true;
            this.SourceControl.IsChecked = false;
        }

        //public void Report(string text)
        //{
        //    Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
        //    {
        //        this.TextBlockStatus.Text += $"{text}{Environment.NewLine}";
        //    }));
        //}

        private void Browse_Button_Click(object sender, RoutedEventArgs e)
        {
            string path = null;

            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == Form.DialogResult.OK)
            {
                path = dialog.SelectedPath;
                this._notificatior.WriteColorMessage(ConsoleColor.Green, "Directory selected... Press 'Clean' button");
            }

            this.Directory.Text = path;
        }

        private async void Clean_Button_Click(object sender, RoutedEventArgs e)
        {
            string directory = this.Directory.Text;
            bool zipProject = this.ZipProject.IsChecked ?? false;
            bool sourceControl = this.SourceControl.IsChecked ?? false;

            var request = new CleanRequest(new[] { directory }, zipProject, sourceControl, null, null, false);

            bool result = await Task.Run(() =>
            {
                this._notificatior.Clear();
                bool isSuccess = this._service.Run(request);
                return isSuccess;
            });

            if (result)
            {
                Form.MessageBox.Show("Cleaning complete", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string message = "Cleaning failed!";
                this._notificatior.WriteColorMessage(ConsoleColor.Red, message);

                Form.MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, (Form.MessageBoxOptions)0x40000);
            }
        }
    }
}
