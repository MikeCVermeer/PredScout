using System;

namespace PredScout
{
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
