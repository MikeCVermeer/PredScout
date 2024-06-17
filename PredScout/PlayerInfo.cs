namespace PredScout
{
    public class PlayerInfo
    {
        public string UserId { get; set; } // From the log file
        public string PlayerName { get; set; } // From the log file
        public string Hero { get; set; } // From the log file
        public string HeroName { get; set; } // From the API
        public int Team { get; set; } // From the log file
        public string TeamRole { get; set; } // From the log file
        public int HeroId { get; set; } // Fetched from HeroNameToId
        public string MMR { get; set; } // Fetched from the API
        public string Rank { get; set; } // Fetched from the API
        public string RankIconPath { get; set; } // Fetched from the API
        public string RoleWinrate { get; set; } // Fetched from the API
        public string OverallWinrate { get; set; } // Fetched from the API
        public string HeroWinrate { get; set; } // Fetched from the API
        public double AvgKills { get; set; } // Fetched from the API
        public double AvgDeaths { get; set; } // Fetched from the API
        public double AvgAssists { get; set; } // Fetched from the API
        public string GamesPlayedWithHero { get; set; } // Fetched from the API
        public string HeroIconPath { get; set; } // Fetched from the API
        public string RoleIconPath { get; set; } // Fetched from the API
        public string FavoriteRole { get; set; } // Fetched from the API

        // Computed properties for PlayerInfo Summary
        public string HeroAndGamesPlayed => $"{HeroName} - {GamesPlayedWithHero}";
    }
}
