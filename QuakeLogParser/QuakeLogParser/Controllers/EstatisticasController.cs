using Microsoft.AspNetCore.Mvc;
using QuakeLogParser.Models;
using System.Diagnostics;

namespace QuakeLogParser.Controllers
{
    public class EstatisticasController : Controller
    {
        private readonly ILogger<EstatisticasController> _logger;

        public EstatisticasController(ILogger<EstatisticasController> logger)
        {
            _logger = logger;
        }

        public IActionResult Estatisticas()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
