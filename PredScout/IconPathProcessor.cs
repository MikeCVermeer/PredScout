using System;
using System.Collections.Generic;

namespace PredScout
{
    public class IconPathProcessor
    {
        private readonly Dictionary<string, string> heroIconDictionary;
        private readonly Dictionary<string, string> rankIconDictionary;
        private readonly Dictionary<string, string> roleIconDictionary;

        public IconPathProcessor()
        {
            // Initialize the dictionary with hero names and their corresponding icon URLs.
            heroIconDictionary = new Dictionary<string, string>
            {
                { "Countess", "pack://application:,,,/assets/heroes/Countess.webp" },
                { "Crunch", "pack://application:,,,/assets/heroes/Crunch.webp" },
                { "Dekker", "pack://application:,,,/assets/heroes/Dekker.webp" },
                { "Drongo", "pack://application:,,,/assets/heroes/Drongo.webp" },
                { "FengMao", "pack://application:,,,/assets/heroes/FengMao.webp" },
                { "TheFey", "pack://application:,,,/assets/heroes/Fey.webp" },
                { "Gadget", "pack://application:,,,/assets/heroes/Gadget.webp" },
                { "Gideon", "pack://application:,,,/assets/heroes/Gideon.webp" },
                { "Grux", "pack://application:,,,/assets/heroes/Grux.webp" },
                { "Howitzer", "pack://application:,,,/assets/heroes/Howitzer.webp" },
                { "Kallari", "pack://application:,,,/assets/heroes/Kallari.webp" },
                { "Khaimera", "pack://application:,,,/assets/heroes/Khaimera.webp" },
                { "LtBelica", "pack://application:,,,/assets/heroes/LtBelica.webp" },
                { "Murdock", "pack://application:,,,/assets/heroes/Murdock.webp" },
                { "Muriel", "pack://application:,,,/assets/heroes/Muriel.webp" },
                { "Narbash", "pack://application:,,,/assets/heroes/Narbash.webp" },
                { "Rampage", "pack://application:,,,/assets/heroes/Rampage.webp" },
                { "Riktor", "pack://application:,,,/assets/heroes/Riktor.webp" },
                { "Sevarog", "pack://application:,,,/assets/heroes/Sevarog.webp" },
                { "Sparrow", "pack://application:,,,/assets/heroes/Sparrow.webp" },
                { "Steel", "pack://application:,,,/assets/heroes/Steel.webp" },
                { "Revenant", "pack://application:,,,/assets/heroes/Revenant.webp" },
                { "Shinbi", "pack://application:,,,/assets/heroes/Shinbi.webp" },
                { "Huntress", "pack://application:,,,/assets/heroes/Huntress.webp" },
                { "Phase", "pack://application:,,,/assets/heroes/Phase.webp" },
                { "Morigesh", "pack://application:,,,/assets/heroes/Morigesh.webp" },
                { "Greystone", "pack://application:,,,/assets/heroes/Greystone.webp" },
                { "TwinBlast", "pack://application:,,,/assets/heroes/TwinBlast.webp" },
                { "Lizard", "pack://application:,,,/assets/heroes/Lizard.webp" },
                { "Serath", "pack://application:,,,/assets/heroes/Serath.webp" },
                { "Wraith", "pack://application:,,,/assets/heroes/Wraith.webp" },
                { "IggyScorch", "pack://application:,,,/assets/heroes/IggyScorch.webp" },
                { "Kwang", "pack://application:,,,/assets/heroes/Kwang.webp" },
                { "Argus", "pack://application:,,,/assets/heroes/Emerald.webp" },
                { "GRIMexe", "pack://application:,,,/assets/heroes/GRIMexe.webp" },
                { "Aurora", "pack://application:,,,/assets/heroes/Aurora.webp" }
            };

            rankIconDictionary = new Dictionary<string, string>
            {
                { "Bronze", "pack://application:,,,/assets/ranks/bronze_1.png" },
                { "Bronze II", "pack://application:,,,/assets/ranks/bronze_2.png" },
                { "Bronze III", "pack://application:,,,/assets/ranks/bronze_3.png" },
                { "Silver I", "pack://application:,,,/assets/ranks/silver_1.png" },
                { "Silver II", "pack://application:,,,/assets/ranks/silver_2.png" },
                { "Silver III", "pack://application:,,,/assets/ranks/silver_3.png" },
                { "Gold I", "pack://application:,,,/assets/ranks/gold_1.png" },
                { "Gold II", "pack://application:,,,/assets/ranks/gold_2.png" },
                { "Gold III", "pack://application:,,,/assets/ranks/gold_3.png" },
                { "Platinum I", "pack://application:,,,/assets/ranks/platinum_1.png" },
                { "Platinum II", "pack://application:,,,/assets/ranks/platinum_2.png" },
                { "Platinum III", "pack://application:,,,/assets/ranks/platinum_3.png" },
                { "Diamond I", "pack://application:,,,/assets/ranks/diamond_1.png" },
                { "Diamond II", "pack://application:,,,/assets/ranks/diamond_2.png" },
                { "Diamond III", "pack://application:,,,/assets/ranks/diamond_3.png" },
                { "Master I", "pack://application:,,,/assets/ranks/master_1.png" },
                { "Master II", "pack://application:,,,/assets/ranks/master_2.png" },
                { "Master III", "pack://application:,,,/assets/ranks/master_3.png" },
                { "Grandmaster", "pack://application:,,,/assets/ranks/grandmaster.png" }
            };

            roleIconDictionary = new Dictionary<string, string>
            {
                { "Offlane", "pack://application:,,,/assets/roles/offlane.png" },
                { "Jungle", "pack://application:,,,/assets/roles/jungle.png" },
                { "Midlane", "pack://application:,,,/assets/roles/midlane.png" },
                { "Carry", "pack://application:,,,/assets/roles/carry.png" },
                { "Support", "pack://application:,,,/assets/roles/support.png" }
            };
        }

        public string GetHeroIcon(string heroName)
        {
            if (heroIconDictionary.TryGetValue(heroName, out string heroIconPath))
            {
                return heroIconPath;
            }
            else
            {
                throw new ArgumentException("Hero name not found");
            }
        }

        public string GetRankIcon(string rankName)
        {
            if (rankIconDictionary.TryGetValue(rankName, out string rankIconPath))
            {
                return rankIconPath;
            }
            else
            {
                throw new ArgumentException("Rank name not found");
            }
        }

        public string GetRoleIcon(string roleName)
        {
            if (roleIconDictionary.TryGetValue(roleName, out string roleIconPath))
            {
                return roleIconPath;
            }
            else
            {
                throw new ArgumentException("Role name not found");
            }
        }
    }
}
