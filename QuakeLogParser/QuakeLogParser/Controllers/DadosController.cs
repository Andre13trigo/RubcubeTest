using Microsoft.AspNetCore.Mvc;
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
                        jogo_aux = new Jogos();
                        jogo_aux.Jogadas = new Dictionary<Jogada, int>();
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

                            Jogada jogada = new Jogada()
                            {
                                jogador1 = jogador1,
                                jogador2 = jogador2,
                                meansOfDeath_T = valorEnum
                            };
                            jogo_aux.Jogadas.Add(jogada, numero_jogo);

                        }
                    }
                }

                return Ok(new { success = true, mensagem = "Dados lidos com sucesso!", jogos = jogos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao ler o arquivo: {ex.Message}");
            }
        }

    }
}
