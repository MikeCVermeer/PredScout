using System;
using System.Collections.Generic;
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
                    var playerInfos = new List<PlayerInfo>();

                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        lastFilePosition = fs.Position;

                        // Matchmaking: State changed MatchStart -> None = For Standard and Brawl - Data loaded: false = for Ranked
                        if (line.Contains("Matchmaking: State changed MatchStart -> None") || line.Contains("Data loaded: false"))
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                inCurrentMatch = false;
                                team0Players.Clear();
                                team1Players.Clear();
                                updateStatus("Not currently in a match");
                            });
                            continue;
                        }

                        if (line.Contains("Pre game screen is now active"))
                        {
                            inCurrentMatch = true;
                            updateStatus("In a match");
                            continue;
                        }

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
                                HeroIconPath = "",
                                MMR = "MMR: 0000",
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

                            playerInfos.Add(playerInfo);
                        }
                    }

                    if (playerInfos.Any())
                    {
                        await ProcessPlayers(playerInfos, team0Players, team1Players);
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
            finally
            {
                // Force garbage collection after processing the log file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private async Task ProcessPlayers(List<PlayerInfo> playerInfos, ObservableCollection<PlayerInfo> team0Players, ObservableCollection<PlayerInfo> team1Players)
        {
            var tasks = playerInfos.Select(async playerInfo =>
            {
                // Fetch and populate player statistics
                await PlayerProcessor.FetchAndPopulatePlayerStatistics(playerInfo, apiService);

                // Calculate Role Statistics
                await RoleCalculations.GetRoleGamesTotal(playerInfo, apiService);

                // Fetch icon paths
                try
                {
                    var iconPathProcessor = new IconPathProcessor();
                    playerInfo.RankIconPath = iconPathProcessor.GetRankIcon(playerInfo.Rank);
                    playerInfo.HeroIconPath = iconPathProcessor.GetHeroIcon(playerInfo.Hero);
                    playerInfo.RoleIconPath = iconPathProcessor.GetRoleIcon(playerInfo.TeamRole);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (playerInfo.Team == 0) team0Players.Add(playerInfo);
                    else team1Players.Add(playerInfo);
                    TeamUtilities.SortTeamPlayers(team0Players);
                    TeamUtilities.SortTeamPlayers(team1Players);
                });
            });

            await Task.WhenAll(tasks);
        }
    }
}
