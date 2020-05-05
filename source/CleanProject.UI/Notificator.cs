using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CleanProject.Service.Interfaces;

namespace CleanProject.UI
{
    public class Notificator : INotificationHelper
    {
        public TextBlock TextBlockStatus { get; private set; }
        public Dispatcher Dispatcher { get; private set; }
        public bool Verbose { get; private set; }

        public void Initialize(TextBlock textBlockStatus, Dispatcher dispatcher, bool verbose)
        {
            TextBlockStatus = textBlockStatus;
            Dispatcher = dispatcher;
            Verbose = verbose;
        }

        public void WriteColorMessage(ConsoleColor colour, string message)
        {
            Write(message, ConsoleColor.Black); 
        }

        public void WriteVerboseMessage(string message)
        {
            if (Verbose)
            {
                WriteMessage(message);
            }
        }

        public void WriteMessage(string message)
        {
            Write(message, ConsoleColor.Black);
        }

        public void Clear()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                this.TextBlockStatus.Text = string.Empty;
            }));
        }

        private void Write(string message, ConsoleColor colour)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                this.TextBlockStatus.Text += $"{message}{Environment.NewLine}";
                this.TextBlockStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colour.ToString()));
            }));
        }
    }
}
