using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using static QuakeLogParser.Models.MetaData;

namespace QuakeLogParser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DadosController : Controller
    {
        private readonly IConfiguration _configuration;

        public DadosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [HttpPost]
        [Route("getdados")]
        public IActionResult GetDados()
        {
            try
            {
                int TotalMortesJogo = 0;

                int TotalMortesWorld = 0;

                string caminhoArquivoConfig = _configuration["CaminhoArquivoLog"];

                if (string.IsNullOrEmpty(caminhoArquivoConfig))
                {
                    return BadRequest("O caminho do arquivo não foi configurado corretamente.");
                }

                string caminhoArquivo = Path.Combine(AppContext.BaseDirectory, _configuration["CaminhoArquivoLog"]);

                if (!System.IO.File.Exists(caminhoArquivo))
                {
                    return NotFound("O arquivo não foi encontrado.");
                }

                string[] linhas = System.IO.File.ReadAllLines(caminhoArquivo);
                List<string> lines = linhas.ToList();
                List<string> important_lines = new List<string>();
                foreach (var item in lines)
                {
                    if (!item.Contains("Item") && !item.Contains("ClientConnect") && !item.Contains("-----------") && !item.Contains("Exit") && !item.Contains("ClientUserinfoChanged") && !item.Contains("ClientBegin") && !item.Contains("ClientDisconnect"))
                    {
                        important_lines.Add(item);
                    }
                }

                bool inicio_jogo = false;
                int numero_jogo = 0;
                List<Jogos> jogos = new List<Jogos>();
                Jogos jogo_aux = null;
                foreach (var item in important_lines)
                {
                    if (item.Contains("InitGame") && (numero_jogo > 0 && inicio_jogo))
                    {
                        jogos.Add(jogo_aux);
                        //nao deu ShutdownGame, reinicio manual
                        inicio_jogo = false;
                        jogo_aux = null;
                    }

                    if (item.Contains("InitGame") && !inicio_jogo)
                    {
                        inicio_jogo = true;
                        numero_jogo++;
                        jogo_aux = new Jogos(numero_jogo);
                        jogo_aux.Jogadas = new List<Jogada>();
                    }
                    else if (item.Contains("ShutdownGame") && (jogo_aux != null && jogo_aux.Jogadas.Count() > 0))
                    {
                        jogos.Add(jogo_aux);
                        inicio_jogo = false;
                        jogo_aux = null;

                    }
                    else
                    {
                        if (inicio_jogo && item.Contains("Kill"))
                        {
                            string[] partes = item.Split(':');
                            string conteudoAposTerceiroDoisPontos = string.Join(":", partes[3..]).Trim();
                            conteudoAposTerceiroDoisPontos = conteudoAposTerceiroDoisPontos.Replace("killed", ",");
                            conteudoAposTerceiroDoisPontos = conteudoAposTerceiroDoisPontos.Replace("by", ",");

                            string[] infos = conteudoAposTerceiroDoisPontos.Split(',');
                            string jogador1 = infos[0].Trim();
                            string jogador2 = infos[1].Trim();
                            string modo = infos[2].Trim();
                            meansOfDeath_t valorEnum;
                            ParseMeansOfDeath(modo, out valorEnum);

                            Jogada jogada = new Jogada(numero_jogo)
                            {
                                jogador1 = jogador1,
                                jogador2 = jogador2,
                                meansOfDeath_T = valorEnum
                            };
                            jogo_aux.AdicionarJogada(jogada);

                        }
                    }
                }

                Retorno ret = new Retorno();

                foreach (var jogo in jogos)
                {
                    if (numero_jogo > 0)
                    {
                        //total de mortes do jogo
                        TotalMortesJogo = jogo.Jogadas.Count();
                        TotalMortesWorld = jogo.Jogadas.Where(x => x.jogador1 == "<world>").Count();
                        Partida partida = new Partida()
                        {
                            numero_jogo = numero_jogo,
                            total_mortes_por_jogo = TotalMortesJogo,
                            total_mortes_por_world = TotalMortesWorld
                        };

                        List<Causa_Mortis> causa_Mortis = new List<Causa_Mortis>();
                        foreach (var jogada in jogo.Jogadas)
                        {
                            if (causa_Mortis.Where(c => c.meansOfDeath_T == jogada.meansOfDeath_T).Any())
                            {
                                causa_Mortis.Where(c => c.meansOfDeath_T == jogada.meansOfDeath_T).FirstOrDefault().morteXCausa++;
                            }
                            else
                            {
                                meansOfDeath_t CAUSA = jogada.meansOfDeath_T;

                                Causa_Mortis causa_Nova = new Causa_Mortis()
                                {
                                    morteXCausa = 1,
                                    meansOfDeath_T = CAUSA,
                                    descricao_meansOfDeath = Enum.GetName(typeof(meansOfDeath_t), CAUSA)
                                };
                                causa_Mortis.Add(causa_Nova);
                            }
                        }

                        partida.lst_causa.AddRange(causa_Mortis);
                        ret.Ret.Add(partida);
                        numero_jogo--;
                    }
                }

                return Ok(new { success = true, mensagem = "Dados lidos com sucesso!", jogos = ret });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao ler o arquivo: {ex.Message}");
            }
        }

        [HttpGet]
        [HttpPost]
        [Route("getranking")]
        public IActionResult GetRanking()
        {
            try
            {
                string caminhoArquivoConfig = _configuration["CaminhoArquivoLog"];

                if (string.IsNullOrEmpty(caminhoArquivoConfig))
                {
                    return BadRequest("O caminho do arquivo não foi configurado corretamente.");
                }

                string caminhoArquivo = Path.Combine(AppContext.BaseDirectory, _configuration["CaminhoArquivoLog"]);

                if (!System.IO.File.Exists(caminhoArquivo))
                {
                    return NotFound("O arquivo não foi encontrado.");
                }

                string[] linhas = System.IO.File.ReadAllLines(caminhoArquivo);
                List<string> lines = linhas.ToList();
                List<string> important_lines = new List<string>();
                foreach (var item in lines)
                {
                    if (!item.Contains("Item") && !item.Contains("ClientConnect") && !item.Contains("-----------") && !item.Contains("Exit") && !item.Contains("ClientBegin") && !item.Contains("ClientDisconnect"))
                    {
                        important_lines.Add(item);
                    }
                }

                bool inicio_jogo = false;
                int numero_jogo = 0;
                List<Jogos> jogos = new List<Jogos>();
                List<Jogadores> players = new List<Jogadores>();
                Jogos jogo_aux = null;
                foreach (var item in important_lines)
                {
                    if (item.Contains("InitGame") && (numero_jogo > 0 && inicio_jogo))
                    {
                        jogos.Add(jogo_aux);
                        //nao deu ShutdownGame, reinicio manual
                        inicio_jogo = false;
                        jogo_aux = null;
                    }

                    if (item.Contains("InitGame") && !inicio_jogo)
                    {
                        inicio_jogo = true;
                        numero_jogo++;
                        jogo_aux = new Jogos(numero_jogo);
                        jogo_aux.Jogadas = new List<Jogada>();
                    }
                    else if (item.Contains("ShutdownGame") && (jogo_aux != null && jogo_aux.Jogadas.Count() > 0))
                    {
                        jogos.Add(jogo_aux);
                        inicio_jogo = false;
                        jogo_aux = null;

                    }
                    else
                    {
                        if (inicio_jogo && item.Contains("Kill"))
                        {
                            string[] partes = item.Split(':');
                            string conteudoAposTerceiroDoisPontos = string.Join(":", partes[3..]).Trim();
                            conteudoAposTerceiroDoisPontos = conteudoAposTerceiroDoisPontos.Replace("killed", ",");
                            conteudoAposTerceiroDoisPontos = conteudoAposTerceiroDoisPontos.Replace("by", ",");

                            string[] infos = conteudoAposTerceiroDoisPontos.Split(',');
                            string jogador1 = infos[0].Trim();
                            string jogador2 = infos[1].Trim();
                            string modo = infos[2].Trim();
                            meansOfDeath_t valorEnum;
                            ParseMeansOfDeath(modo, out valorEnum);

                            Jogada jogada = new Jogada(numero_jogo)
                            {
                                jogador1 = jogador1,
                                jogador2 = jogador2,
                                meansOfDeath_T = valorEnum
                            };
                            jogo_aux.AdicionarJogada(jogada);

                        }
                        else
                        {
                            if (inicio_jogo && item.Contains("ClientUserinfoChanged"))
                            {
                                try
                                {
                                    string[] partes = item.Split(':');
                                    string conteudoAposTerceiroDoisPontos = string.Join(":", partes[2..]).Trim();
                                    int indice = conteudoAposTerceiroDoisPontos.IndexOf("\\t\\");
                                    string novaString = conteudoAposTerceiroDoisPontos.Substring(0, indice);
                                    novaString = novaString.Replace("n\\", ",");
                                    string[] infos = novaString.Split(',');
                                    string Id = infos[0].Trim();
                                    string jogador = infos[1].Trim();

                                    Jogadores jNew = new Jogadores(numero_jogo)
                                    {
                                        Id = Convert.ToInt32(Id),
                                        NomeJogador = jogador,
                                        Pontuacao = 0
                                    };
                                    players.Add(jNew);
                                }
                                catch (Exception ex)
                                {
                                    return StatusCode(500, $"Ocorreu um erro ao ler o arquivo: {ex.Message}");
                                }
                            }
                        }
                    }
                }
                players = players.OrderBy(p => p.NumeroDoJogo).ToList();
                List<Jogadores> jogadoresSemDuplicatas = players
                                .GroupBy(j => new { j.Id, j.NomeJogador, j.NumeroDoJogo })
                                .Select(g => g.First())
                                .ToList();

                jogos = jogos.OrderByDescending(c => c.NumeroDoJogo).ToList();
                List<Jogadores> lst_jogadoresXJogo = null;
                List<Jogadores> lst_jogadoresXJogo_retorno = new List<Jogadores>();
                foreach (var jogo in jogos)
                {
                    if (numero_jogo > 0)
                    {
                        lst_jogadoresXJogo = jogadoresSemDuplicatas.Where(j => j.NumeroDoJogo == numero_jogo).ToList();
                        foreach (var jogada in jogo.Jogadas)
                        {
                            Jogadores jogador_matou = lst_jogadoresXJogo.Where(x => x.NomeJogador == jogada.jogador1 && x.NomeJogador != "<world>").FirstOrDefault();
                            if (jogador_matou != null)
                            {
                                jogador_matou.Pontuacao++;
                            }
                            Jogadores jogador_morreu = lst_jogadoresXJogo.Where(x => x.NomeJogador == jogada.jogador2).FirstOrDefault();
                            jogador_morreu.Pontuacao--;

                        }
                        numero_jogo--;
                        lst_jogadoresXJogo_retorno.AddRange(lst_jogadoresXJogo);
                    }
                }



                return Ok(new { success = true, mensagem = "Dados lidos com sucesso!", jogos = lst_jogadoresXJogo_retorno });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao ler o arquivo: {ex.Message}");
            }
        }
    }
}
