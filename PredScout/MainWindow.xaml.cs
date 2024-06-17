using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PredScout
{
    public partial class MainWindow : Window
    {
        private readonly string logFilePath = Environment.ExpandEnvironmentVariables(@"%localappdata%\Predecessor\Saved\Logs\Predecessor.log");
        private readonly FileSystemWatcher fileWatcher;
        private readonly LogFileProcessor logFileProcessor;
        public ObservableCollection<PlayerInfo> Team0Players { get; set; }
        public ObservableCollection<PlayerInfo> Team1Players { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Team0Players = new ObservableCollection<PlayerInfo>();
            Team1Players = new ObservableCollection<PlayerInfo>();

            // Bind the data to the ItemsControls
            TeamDawnItemsControl.ItemsSource = Team0Players;
            TeamDuskItemsControl.ItemsSource = Team1Players;

            logFileProcessor = new LogFileProcessor(logFilePath);

            try
            {
                Console.WriteLine("Initializing FileSystemWatcher...");
                fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(logFilePath), Path.GetFileName(logFilePath))
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
                };
                fileWatcher.Changed += OnLogFileChanged;
                fileWatcher.EnableRaisingEvents = true;
                Console.WriteLine($"FileSystemWatcher initialized. Watching {logFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing FileSystemWatcher: {ex.Message}");
            }

            ProcessLogFile();  // Initial read to catch up on the latest log data
        }

        private void OnLogFileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Log file changed: {e.FullPath}");
            ProcessLogFile();
        }

        private void ProcessLogFile()
        {
            logFileProcessor.ProcessLogFile(Team0Players, Team1Players, status => Dispatcher.Invoke(() => StatusTextBlock.Text = status));
        }

        // Custom Border Functionality
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
