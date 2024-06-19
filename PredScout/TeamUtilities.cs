using System.Collections.ObjectModel;
using System.Linq;

namespace PredScout
{
    public static class TeamUtilities
    {
        public static void SortTeamPlayers(ObservableCollection<PlayerInfo> players)
        {
            var sortedPlayers = players.OrderBy(p => GetRoleOrder(p.TeamRole)).ToList();
            players.Clear();
            foreach (var player in sortedPlayers) players.Add(player);
        }

        private static int GetRoleOrder(string role)
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
