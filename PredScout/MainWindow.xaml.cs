using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Path = System.IO.Path;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace PredScout
{
    public partial class MainWindow : Window
    {
        private readonly string logFilePath = Environment.ExpandEnvironmentVariables(@"%localappdata%\Predecessor\Saved\Logs\Predecessor.log");
        private readonly FileSystemWatcher fileWatcher;
        public ObservableCollection<PlayerInfo> Team0Players { get; set; }
        public ObservableCollection<PlayerInfo> Team1Players { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Team0Players = new ObservableCollection<PlayerInfo>();
            Team1Players = new ObservableCollection<PlayerInfo>();

            // Bind the data to the ListViews
            Team0ListView.ItemsSource = Team0Players;
            Team1ListView.ItemsSource = Team1Players;

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

            //ProcessLogFile();  // Initial read to catch up on the latest log data
        }

        private void OnLogFileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Log file changed: {e.FullPath}");
            ProcessLogFile();
        }

        private void ProcessLogFile()
        {
            try
            {
                Console.WriteLine("Reading log file...");
                string[] lines = File.ReadAllLines(logFilePath);
                bool inCurrentMatch = true;

                Console.WriteLine("Clearing current player information...");
                Dispatcher.Invoke(() =>
                {
                    Team0Players.Clear();
                    Team1Players.Clear();
                });

                //for (int i = lines.Length - 1; i >= 0; i--)
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    Console.WriteLine($"Processing line {i}: {line}");

                    if (line.Contains("LogPredLoadingScreenManager: Displaying pre game loading screen with player data"))
                    {
                        Console.WriteLine("Found start of new match block.");
                        inCurrentMatch = true;
                        continue;
                    }

                    if (inCurrentMatch && line.Contains("LogPredLoadingScreenManager: UserID:"))
                    {
                        var match = Regex.Match(line, @"UserID:\s(\S+),.*Player Name:\s([^,]+),.*HeroData:\sHero_([^,]+),.*Team:\s(\d),.*Team Role:\s(\w+)");
                        if (match.Success)
                        {
                            string userId = match.Groups[1].Value;
                            string playerName = match.Groups[2].Value;
                            string hero = match.Groups[3].Value;
                            int team = int.Parse(match.Groups[4].Value);
                            string teamRole = match.Groups[5].Value;

                            Console.WriteLine($"Match found: UserID={userId}, PlayerName={playerName}, Hero={hero}, Team={team}, TeamRole={teamRole}");

                            PlayerInfo playerInfo = new PlayerInfo
                            {
                                UserId = userId,
                                PlayerName = playerName,
                                Hero = hero,
                                Team = team,
                                TeamRole = teamRole,
                                MMR = "N/A",  // Placeholder for MMR
                                RoleWinrate = "N/A",  // Placeholder for Role Winrate
                                OverallWinrate = "N/A",  // Placeholder for Overall Winrate
                                HeroWinrate = "N/A"  // Placeholder for Hero Winrate
                            };

                            Dispatcher.Invoke(() =>
                            {
                                if (team == 0)
                                {
                                    Team0Players.Add(playerInfo);
                                }
                                else
                                {
                                    Team1Players.Add(playerInfo);
                                }
                                Console.WriteLine($"Player info added: {playerName}");
                            });
                        }
                        else
                        {
                            Console.WriteLine("Regex match failed.");
                        }
                    }

                    //if (inCurrentMatch && !line.Contains("LogPredLoadingScreenManager: UserID:") && !line.Contains("LogPredLoadingScreenManager: Displaying pre game loading screen with player data"))
                    //{
                    //    Console.WriteLine("End of current match block.");
                    //    break;
                    //}
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IOException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected exception: {ex.Message}");
            }
        }
    }

    public class PlayerInfo
    {
        public string UserId { get; set; }
        public string PlayerName { get; set; }
        public string Hero { get; set; }
        public int Team { get; set; }
        public string TeamRole { get; set; }
        public string MMR { get; set; }
        public string RoleWinrate { get; set; }
        public string OverallWinrate { get; set; }
        public string HeroWinrate { get; set; }
    }
}

