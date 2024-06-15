using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace PredScout
{
    public class LogFileProcessor
    {
        private readonly string logFilePath;
        private long lastFilePosition;
        private bool inMatch;

        public LogFileProcessor(string logFilePath)
        {
            this.logFilePath = logFilePath;
            this.lastFilePosition = 0;
            this.inMatch = false;
        }

        public void ProcessLogFile(ObservableCollection<PlayerInfo> team0Players, ObservableCollection<PlayerInfo> team1Players, Action<string> updateStatus)
        {
            try
            {
                Console.WriteLine("Reading log file...");

                using (FileStream fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs))
                {
                    // Move to the last read position
                    fs.Seek(lastFilePosition, SeekOrigin.Begin);

                    string line;
                    bool inCurrentMatch = false;

                    while ((line = sr.ReadLine()) != null)
                    {
                        // Update the last file position
                        lastFilePosition = fs.Position;

                        Console.WriteLine($"Processing line: {line}");

                        if (line.Contains("Pre game screen is now active"))
                        {
                            Console.WriteLine("Found start of new match block.");
                            inCurrentMatch = true;
                            inMatch = true;
                            updateStatus("In a match");
                            continue;
                        }

                        if (line.Contains("Matchmaking: State changed MatchStart -> None"))
                        {
                            Console.WriteLine("Match finished, clearing teams.");
                            inMatch = false;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                inCurrentMatch = false;
                                team0Players.Clear();
                                team1Players.Clear();
                                updateStatus("Not currently in a match");
                            });
                            continue; // Exit since the match has ended
                        }

                        if (inCurrentMatch && line.Contains("LogPredLoadingScreenManager: UserID:"))
                        {
                            var match = Regex.Match(line, @"UserID:\s(\S+),.*Player Name:\s([^,]+),.*HeroData:\sHero_([^,]+),.*Team:\s(\d),.*Team Role:\s(\w+)");
                            if (!match.Success)
                            {
                                Console.WriteLine("Regex match failed.");
                                continue;
                            }

                            string userId = match.Groups[1].Value;
                            string playerName = match.Groups[2].Value;
                            string hero = match.Groups[3].Value;
                            int team = int.Parse(match.Groups[4].Value);
                            string teamRole = match.Groups[5].Value;

                            Console.WriteLine($"Match found: UserID={userId}, PlayerName={playerName}, Hero={hero}, Team={team}, TeamRole={teamRole}");

                            var playerInfo = new PlayerInfo
                            {
                                UserId = string.IsNullOrEmpty(userId) ? "Error" : userId,
                                PlayerName = string.IsNullOrEmpty(playerName) ? "Error" : playerName,
                                Hero = string.IsNullOrEmpty(hero) ? "Error" : hero,
                                Team = team,
                                TeamRole = string.IsNullOrEmpty(teamRole) ? "Error" : teamRole,
                                MMR = "Error",  // Placeholder for MMR
                                RoleWinrate = "Error",  // Placeholder for Role Winrate
                                OverallWinrate = "Error",  // Placeholder for Overall Winrate
                                HeroWinrate = "Error"  // Placeholder for Hero Winrate
                            };

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (team == 0)
                                {
                                    team0Players.Add(playerInfo);
                                }
                                else
                                {
                                    team1Players.Add(playerInfo);
                                }
                                Console.WriteLine($"Player info added: {playerName}");

                                // Sort players within each team
                                SortTeamPlayers(team0Players);
                                SortTeamPlayers(team1Players);
                            });
                        }
                    }
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

        private void SortTeamPlayers(ObservableCollection<PlayerInfo> players)
        {
            var sortedPlayers = players.OrderBy(p => GetRoleOrder(p.TeamRole)).ToList();
            players.Clear();
            foreach (var player in sortedPlayers)
            {
                players.Add(player);
            }
        }

        private int GetRoleOrder(string role)
        {
            return role switch
            {
                "Offlane" => 0,
                "Jungle" => 1,
                "Midlane" => 2,
                "Carry" => 3,
                "Support" => 4,
                _ => 5,
            };
        }
    }
}
