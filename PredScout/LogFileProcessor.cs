using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PredScout
{
    public class LogFileProcessor
    {
        private readonly string logFilePath;
        private long lastFilePosition;
        private readonly ApiService apiService;

        public LogFileProcessor(string logFilePath)
        {
            this.logFilePath = logFilePath;
            this.lastFilePosition = 0;
            this.apiService = new ApiService();
        }

        public async Task ProcessLogFile(ObservableCollection<PlayerInfo> team0Players, ObservableCollection<PlayerInfo> team1Players, Action<string> updateStatus)
        {
            try
            {
                using (FileStream fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs))
                {
                    fs.Seek(lastFilePosition, SeekOrigin.Begin);

                    string line;
                    bool inCurrentMatch = false;

                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        lastFilePosition = fs.Position;

                        if (line.Contains("Pre game screen is now active"))
                        {
                            inCurrentMatch = true;
                            updateStatus("In a match");
                            continue;
                        }

                        //if (line.Contains("Matchmaking: State changed MatchStart -> None"))
                        //{
                        //    await Application.Current.Dispatcher.InvokeAsync(() =>
                        //    {
                        //        inCurrentMatch = false;
                        //        team0Players.Clear();
                        //        team1Players.Clear();
                        //        updateStatus("Not currently in a match");
                        //    });
                        //    continue;
                        //}

                        if (inCurrentMatch && line.Contains("LogPredLoadingScreenManager: UserID:"))
                        {
                            var match = Regex.Match(line, @"UserID:\s(\S+),.*Player Name:\s([^,]+),.*HeroData:\sHero_([^,]+),.*Team:\s(\d),.*Team Role:\s(\w+)");
                            if (!match.Success) continue;

                            string userId = match.Groups[1].Value;
                            string playerName = match.Groups[2].Value;
                            string hero = match.Groups[3].Value;
                            int team = int.Parse(match.Groups[4].Value);
                            string teamRole = match.Groups[5].Value;

                            var playerInfo = new PlayerInfo
                            {
                                UserId = userId ?? "Error",
                                PlayerName = playerName ?? "Error",
                                Hero = hero ?? "Error",
                                Team = team,
                                TeamRole = teamRole ?? "Error",
                                GamesPlayedWithHero = "0",
                                RankIconPath = "",
                                HeroIconPath = ""
                            };

                            try
                            {
                                var heroNameToId = new HeroNameToId();
                                int heroId = heroNameToId.GetHeroId(hero);
                                playerInfo.HeroId = heroId;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            // Fetch and populate player statistics
                            await PlayerProcessor.FetchAndPopulatePlayerStatistics(playerInfo, apiService);

                            try
                            {
                                var iconPathProcessor = new IconPathProcessor();
                                playerInfo.RankIconPath = iconPathProcessor.GetRankIcon(playerInfo.Rank);
                                playerInfo.HeroIconPath = iconPathProcessor.GetHeroIcon(hero);
                                playerInfo.RoleIconPath = iconPathProcessor.GetRoleIcon(playerInfo.TeamRole);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                if (team == 0) team0Players.Add(playerInfo);
                                else team1Players.Add(playerInfo);
                                TeamUtilities.SortTeamPlayers(team0Players);
                                TeamUtilities.SortTeamPlayers(team1Players);
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
    }
}
