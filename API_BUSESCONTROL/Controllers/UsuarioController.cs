using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using API_BUSESCONTROL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase {

        private readonly IUsuarioService _usuarioService;
        private readonly ITokenService _tokenService;

        public UsuarioController(IUsuarioService usuarioService, ITokenService tokenService) {
            _usuarioService = usuarioService;
            _tokenService = tokenService;
        }

        [HttpPost("Autenticar")]
        [AllowAnonymous]
        public IActionResult Autenticar(Login login) {
            try {
                if (ModelState.IsValid) {
                    login.Cpf = login.Cpf.Replace(".", "").Replace("-", "");
                    Funcionario usuario = _usuarioService.ValidationCredenciasUser(login);
                    var token = _tokenService.GenerateToken(usuario);
                    return Ok(new { token });
                }
                return BadRequest(login);
            }
            catch (Exception error) {
                return BadRequest(error.Message);
            }
        }

    }
}
