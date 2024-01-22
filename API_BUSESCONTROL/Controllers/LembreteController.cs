using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Assistente, Administrador")]
    public class LembreteController : ControllerBase {

        private readonly ILembreteRepository _lembreteRepository;

        public LembreteController(ILembreteRepository lembreteRepository) {
            _lembreteRepository = lembreteRepository;
        }

        [HttpPost("EnviarMensagem")]
        public IActionResult EnviarMensagem([FromBody] Lembrete lembrete) {
            try {
                if (ModelState.IsValid) {
                    _lembreteRepository.CreateLembreteMensagem(lembrete);
                    return Ok(lembrete);
                }
                return BadRequest(lembrete);
            }
            catch (Exception error) {
                return StatusCode(500, $"Desculpe, houve um erro na requisição: {error.Message}");
            }
        }

        [HttpGet("GetAllLembreteMensagens/{usuarioId}/{roleNumber}")]
        public IActionResult GetAllLembreteMensagens(int usuarioId, int roleNumber) {
            List<Lembrete> list = _lembreteRepository.GetAllLembreteMensagens(usuarioId, roleNumber);
            list = list.OrderByDescending(x => x.Id).ToList();
            return Ok(list);
        }

        [HttpGet("GetCountLembreteMensagens/{usuarioId}/{roleNumber}")]
        public IActionResult GetCountLembreteMensagens(int usuarioId, int roleNumber) {
            int count = _lembreteRepository.GetCountLembreteMensagens(usuarioId, roleNumber);
            return Ok(count);
        }

        [HttpGet("GetAllLembreteNotificacoes/{usuarioId}/{roleNumber}")]
        public IActionResult GetAllLembreteNotificacoes(int usuarioId, int roleNumber) {
            List<Lembrete> list = _lembreteRepository.GetAllLembreteNotificacoes(usuarioId, roleNumber);
            list = list.OrderByDescending(x => x.Id).ToList();
            return Ok(list);
        }

        [HttpGet("GetCountLembreteNotificacoes/{usuarioId}/{roleNumber}")]
        public IActionResult GetCountLembreteNotificacoes(int usuarioId, int roleNumber) {
            int count = _lembreteRepository.GetCountLembreteNotificacoes(usuarioId, roleNumber);
            return Ok(count);
        }
    }
}
