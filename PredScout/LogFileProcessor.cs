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
                    // Move to the last read position
                    fs.Seek(lastFilePosition, SeekOrigin.Begin);

                    string line;
                    bool inCurrentMatch = false;

                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        // Update the last file position
                        lastFilePosition = fs.Position;

                        if (line.Contains("Pre game screen is now active"))
                        {
                            Console.WriteLine("Found start of new match block.");
                            inCurrentMatch = true;
                            updateStatus("In a match");
                            continue;
                        }

                        if (line.Contains("Matchmaking: State changed MatchStart -> None"))
                        {
                            Console.WriteLine("Match finished, clearing teams.");
                            await Application.Current.Dispatcher.InvokeAsync(() =>
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

                            //Console.WriteLine($"Match found: UserID={userId}, PlayerName={playerName}, Hero={hero}, Team={team}, TeamRole={teamRole}");

                            var playerInfo = new PlayerInfo
                            {
                                UserId = string.IsNullOrEmpty(userId) ? "Error" : userId,
                                PlayerName = string.IsNullOrEmpty(playerName) ? "Error" : playerName,
                                Hero = string.IsNullOrEmpty(hero) ? "Error" : hero,
                                Team = team,
                                TeamRole = string.IsNullOrEmpty(teamRole) ? "Error" : teamRole,
                                GamesPlayedWithHero = "0", // Default value
                                RankIconPath = "", // Default value
                                HeroIconPath = "" // Default value
                            };

                            // Get Rank Icon, Hero ID and Hero Icon
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

                            // Fetch player statistics
                            await FetchAndPopulatePlayerStatistics(playerInfo);

                            // Get Rank, Role and Hero Icons
                            try
                            {
                                var IconPathProcessor = new IconPathProcessor();
                                playerInfo.RankIconPath = IconPathProcessor.GetRankIcon(playerInfo.Rank);
                                playerInfo.HeroIconPath = IconPathProcessor.GetHeroIcon(hero);
                                playerInfo.RoleIconPath = IconPathProcessor.GetRoleIcon(playerInfo.TeamRole);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }


                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                if (team == 0)
                                {
                                    team0Players.Add(playerInfo);
                                }
                                else
                                {
                                    team1Players.Add(playerInfo);
                                }

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

        private async Task FetchAndPopulatePlayerStatistics(PlayerInfo playerInfo)
        {
            try
            {
                // Fetch player rank statistics from the API
                var playerRankStats = await apiService.GetPlayerRank(playerInfo.UserId);
                playerInfo.MMR = playerRankStats?["mmr"]?.ToObject<string>()?.PadLeft(4, '0').Substring(0, 4) ?? "Error";
                playerInfo.Rank = playerRankStats?["rank_title"]?.ToString() ?? "Unranked";

                var heroStats = await apiService.GetPlayerHeroStatistics(playerInfo.UserId, playerInfo.HeroId);
                var heroStatsData = heroStats?["hero_statistics"]?.FirstOrDefault();

                // Fetch hero statistics from the API
                playerInfo.HeroWinrate = heroStatsData != null
                    ? Math.Round((heroStatsData["winrate"]?.ToObject<decimal>() ?? 0) * 100) + "%"
                    : "0%";
                playerInfo.HeroName = heroStatsData?["display_name"]?.ToObject<string>() ?? "Error";

                // Fetch Player statistics from the API
                var playerStats = await apiService.GetPlayerStatistics(playerInfo.UserId);
                playerInfo.OverallWinrate = Math.Round((playerStats?["winrate"]?.ToObject<decimal>() ?? 0) * 100).ToString("0.##") + "% Winrate";
                playerInfo.RoleWinrate = "Not Available yet.."; // Placeholder for Role Winrate

                var favoriteRole = playerStats?["favorite_role"];

                if (favoriteRole == null || string.IsNullOrWhiteSpace(favoriteRole.ToString()))
                {
                    playerInfo.FavoriteRole = "Favorite Role = None yet";
                }
                else
                {
                    playerInfo.FavoriteRole = $"Favorite Role = {favoriteRole}";
                }

                // Set KDA with AvgKills, AvgDeaths, and AvgAssists from API response
                playerInfo.AvgKills = heroStatsData?["avg_kills"]?.ToObject<double>() ?? 0;
                playerInfo.AvgDeaths = heroStatsData?["avg_deaths"]?.ToObject<double>() ?? 0;
                playerInfo.AvgAssists = heroStatsData?["avg_assists"]?.ToObject<double>() ?? 0;

                playerInfo.GamesPlayedWithHero = (heroStatsData?["match_count"]?.ToObject<int>() ?? 0) + " games played";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching player statistics: {ex.Message}");
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
