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
            public int NumeroDoJogo { get; set; }
            public string jogador1 { get; set; }
            public string jogador2 { get; set; }
            public meansOfDeath_t meansOfDeath_T { get; set; }

            public Jogada(int numeroDoJogo)
            {
                NumeroDoJogo = numeroDoJogo;
            }
        }

        public class Jogos
        {
            public List<Jogada> Jogadas { get; set; }
            public int NumeroDoJogo { get; set; }

            public Jogos()
            {
                Jogadas = new List<Jogada>();
            }

            public void AdicionarJogada(Jogada jogada)
            {
                Jogadas.Add(jogada);
            }

            public Jogos(int numeroDoJogo)
            {
                NumeroDoJogo = numeroDoJogo;
            }
        }

        public static void ParseMeansOfDeath(string modo, out meansOfDeath_t valorEnum)
        {
            Enum.TryParse(modo, out valorEnum);
        }

        public class Partida
        {
            public int total_mortes_por_jogo { get; set; }
            public int total_mortes_por_world { get; set; }
            public int total_mortes_por_causa { get; set; }
            public int numero_jogo { get; set; }
            public List<Causa_Mortis> lst_causa { get; set; }
            public Partida()
            {
                lst_causa = new List<Causa_Mortis>();
            }
        }

        public class Retorno
        {
            public List<Partida> Ret { get; set; }
            public Retorno()
            {
                Ret = new List<Partida>();
            }
        }

        public class Causa_Mortis
        {
            public meansOfDeath_t meansOfDeath_T { get; set; }
            public string descricao_meansOfDeath { get; set; }
            public int morteXCausa { get;set; }

        }

        public class Jogadores
        {
            public int? Id { get; set; }
            public string NomeJogador { get; set; }
            public int Pontuacao { get; set; }
            public int NumeroDoJogo { get; set; }
            public Jogadores(int numeroDoJogo)
            {
                NumeroDoJogo = numeroDoJogo;
            }
        }
    }
}
