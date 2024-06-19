using System;
using System.Linq;
using System.Threading.Tasks;

namespace PredScout
{
    public static class PlayerProcessor
    {
        public static async Task FetchAndPopulatePlayerStatistics(PlayerInfo playerInfo, ApiService apiService)
        {
            try
            {
                var playerRankStats = await apiService.GetPlayerRank(playerInfo.UserId);
                playerInfo.MMR = playerRankStats?["mmr"]?.ToObject<string>()?.PadLeft(4, '0').Substring(0, 4) ?? "Error";
                playerInfo.Rank = playerRankStats?["rank_title"]?.ToString() ?? "Unranked";

                var heroStats = await apiService.GetPlayerHeroStatistics(playerInfo.UserId, playerInfo.HeroId);
                var heroStatsData = heroStats?["hero_statistics"]?.FirstOrDefault();

                playerInfo.HeroWinrate = heroStatsData != null
                    ? Math.Round((heroStatsData["winrate"]?.ToObject<decimal>() ?? 0) * 100) + "%"
                    : "0%";
                playerInfo.HeroName = heroStatsData?["display_name"]?.ToObject<string>() ?? "Error";

                var playerStats = await apiService.GetPlayerStatistics(playerInfo.UserId);
                playerInfo.OverallWinrate = Math.Round((playerStats?["winrate"]?.ToObject<decimal>() ?? 0) * 100).ToString("0.##") + "% Winrate";
                playerInfo.RoleWinrate = "Not Available yet..";

                var favoriteRole = playerStats?["favorite_role"];
                playerInfo.FavoriteRole = favoriteRole == null || string.IsNullOrWhiteSpace(favoriteRole.ToString())
                    ? "Favorite Role = None yet"
                    : $"Favorite Role = {favoriteRole}";

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
    }
}
