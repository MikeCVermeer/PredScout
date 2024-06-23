using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace PredScout
{
    public static class RoleCalculations
    {
        public static async Task GetRoleGamesTotal(PlayerInfo playerInfo, ApiService apiService)
        {
            int totalMatches = playerInfo.TotalGames;
            int pageSize = 100; // Assuming we fetch 100 matches per page
            int pages = (int)Math.Ceiling(totalMatches / (double)pageSize);

            //Console.WriteLine($"Fetching {totalMatches} matches for {playerInfo.PlayerName} as {playerInfo.TeamRole}...");

            var tasks = new List<Task<JObject>>();
            for (int page = 1; page <= pages; page++)
            {
                tasks.Add(apiService.GetMatchesPerPageAndRole(playerInfo.UserId, page, playerInfo.TeamRole));
            }

            // Fetch all pages concurrently
            var results = await Task.WhenAll(tasks);

            // Flatten the matches into a single collection
            var matches = results.SelectMany(result => result["matches"]).ToList();

            // Use PLINQ to process matches in parallel
            var matchResults = matches.AsParallel().Select(match =>
            {
                var players = match["players"];
                var player = players.FirstOrDefault(p => p["id"].ToString() == playerInfo.UserId);

                bool isRoleMatch = false;
                bool isWin = false;

                if (player != null && player["role"].ToString().Equals(playerInfo.TeamRole, StringComparison.OrdinalIgnoreCase))
                {
                    isRoleMatch = true;

                    if (match["winning_team"].ToString().Equals(player["team"].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        isWin = true;
                    }
                }

                return (isRoleMatch, isWin);
            }).ToList();

            int totalGamesPlayedAsRole = matchResults.Count(result => result.isRoleMatch);
            int totalWinsAsRole = matchResults.Count(result => result.isWin);

            playerInfo.RoleWinrate = totalGamesPlayedAsRole > 0 ? $"{Math.Round((totalWinsAsRole / (double)totalGamesPlayedAsRole) * 100)}%" : "0%";
            playerInfo.GamesPlayedWithRole = $"{totalGamesPlayedAsRole} games played";

            //Console.WriteLine($"Role Winrate for {playerInfo.PlayerName} as {playerInfo.TeamRole}: {playerInfo.RoleWinrate}");
            //Console.WriteLine($"{playerInfo.PlayerName}'s Total games played as {playerInfo.TeamRole}: {totalGamesPlayedAsRole}");
        }
    }
}
