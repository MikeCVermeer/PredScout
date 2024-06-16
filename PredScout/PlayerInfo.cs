using System;

namespace PredScout
{
    public class PlayerInfo
    {
        public string UserId { get; set; } // From the log file
        public string PlayerName { get; set; } // From the log file
        public string Hero { get; set; } // From the log file
        public int Team { get; set; } // From the log file
        public string TeamRole { get; set; } // From the log file
        public int HeroId { get; set; } // Fetched from HeroNameToId
        public string MMR { get; set; } // Fetched from the API
        public string Rank { get; set; } // Fetched from the API
        public string RoleWinrate { get; set; } // Fetched from the API
        public string OverallWinrate { get; set; } // Fetched from the API
        public string HeroWinrate { get; set; } // Fetched from the API

    }
}
