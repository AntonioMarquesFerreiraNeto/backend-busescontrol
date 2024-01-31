using API_BUSESCONTROL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers {

    [Route("api/[controller]")]
    [Authorize(Roles = "Assistente, Administrador")]
    [ApiController]
    public class InformacoesApoioController : ControllerBase {

        private readonly AjudantesService _ajudantesService;
        
        public InformacoesApoioController(AjudantesService ajudantesService) {
            _ajudantesService = ajudantesService;
        }

        [HttpGet("GetEstadoAndUfList")]
        public IActionResult GetEstadoAndUfList() {
            var list = _ajudantesService.ReturnListEstadoUF();
            return Ok(list);
        }

    }
}
