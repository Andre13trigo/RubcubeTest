namespace QuakeLogParser.Models
{
    public class MetaData
    {
        // means of death
        public enum meansOfDeath_t
        {
            MOD_UNKNOWN,
            MOD_SHOTGUN,
            MOD_GAUNTLET,
            MOD_MACHINEGUN,
            MOD_GRENADE,
            MOD_GRENADE_SPLASH,
            MOD_ROCKET,
            MOD_ROCKET_SPLASH,
            MOD_PLASMA,
            MOD_PLASMA_SPLASH,
            MOD_RAILGUN,
            MOD_LIGHTNING,
            MOD_BFG,
            MOD_BFG_SPLASH,
            MOD_WATER,
            MOD_SLIME,
            MOD_LAVA,
            MOD_CRUSH,
            MOD_TELEFRAG,
            MOD_FALLING,
            MOD_SUICIDE,
            MOD_TARGET_LASER,
            MOD_TRIGGER_HURT,
            MISSIONPACK,
            MOD_NAIL,
            MOD_CHAINGUN,
            MOD_PROXIMITY_MINE,
            MOD_KAMIKAZE,
            MOD_JUICED,
            MOD_GRAPPLE
        }

        public class Jogada
        {
            public string jogador1 { get; set; }
            public string jogador2 { get; set; }
            public meansOfDeath_t meansOfDeath_T { get; set; }
        }

        public class Jogos
        {
            public Dictionary<Jogada,int> Jogadas { get; set; }
        }

        public static void ParseMeansOfDeath(string modo, out meansOfDeath_t valorEnum)
        {
            Enum.TryParse(modo, out valorEnum);
        }

    }
}
