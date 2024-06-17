using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PredScout
{
    using System;
    using System.Collections.Generic;

    public class HeroNameToId
    {
        private Dictionary<string, int> heroDictionary;

        public HeroNameToId()
        {
            // Initialize the dictionary with some example hero names and IDs.
            heroDictionary = new Dictionary<string, int>
            {
                { "Countess", 1 },
                { "Crunch", 2 },
                { "Dekker", 3 },
                { "Drongo", 4 },
                { "FengMao", 5 },
                { "Fey", 6 }, // The Fey
                { "Gadget", 7 },
                { "Gideon", 8 },
                { "Grux", 9 },
                { "Howitzer", 10 },
                { "Kallari", 11 },
                { "Khaimera", 12 },
                { "LtBelica", 13 },
                { "Murdock", 14 },
                { "Muriel", 15 },
                { "Narbash", 16 },
                { "Rampage", 17 },
                { "Riktor", 18 },
                { "Sevarog", 19 },
                { "Sparrow", 20 },
                { "Steel", 21 },
                { "Revenant", 22 },
                { "Shinbi", 23 },
                { "Huntress", 24 }, // Kira
                { "Phase", 25 },
                { "Morigesh", 27 },
                { "Greystone", 29 },
                { "TwinBlast", 30 },
                { "Lizard", 31 }, // Zarus
                { "Serath", 39 },
                { "Wraith", 41 },
                { "IggyScorch", 42 },
                { "Kwang", 44 },
                { "Emerald", 49 }, // Argus
                { "GRIMexe", 52 },
                { "Aurora", 53 }
            };
        }

        public int GetHeroId(string heroName)
        {
            if (heroDictionary.TryGetValue(heroName, out int heroId))
            {
                return heroId;
            }
            else
            {
                throw new ArgumentException("Hero name not found");
            }
        }
    }

}
