using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Assistente, Administrador")]
    public class PaletaCoresController : ControllerBase {

        private readonly IPaletaCoresRepository _paletaCoresRepository;

        public PaletaCoresController(IPaletaCoresRepository paletaCoresRepository) {
            _paletaCoresRepository = paletaCoresRepository;
        }

        [HttpGet]
        public IActionResult GetPaletaCores() {
            List<PaletaCores> list = _paletaCoresRepository.ListPaletaCores();
            if (list == null) return NotFound();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult CreatePaleta([FromBody] PaletaCores paletaCores) {
            try {
                if (ModelState.IsValid) {
                    _paletaCoresRepository.CreatePaletaCores(paletaCores);
                    return Ok(paletaCores);
                }
                return BadRequest(paletaCores);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePaleta(int? id) {
            try {
                _paletaCoresRepository.DeletePaletaCores(id);
                return Ok();
            } catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }
    }
}
