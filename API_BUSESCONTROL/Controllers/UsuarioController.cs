using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers
{
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

        [HttpPost("EsqueceuSenha")]
        [AllowAnonymous]
        public IActionResult EsqueceuSenha(EsqueceuSenha esqueceuSenha) {
            try {
                if (ModelState.IsValid) {
                    _usuarioService.EsqueceuSenha(esqueceuSenha);
                    return Ok();
                }
                return BadRequest(esqueceuSenha);
            }
            catch (Exception error) {
                return BadRequest(error.Message);
            }
        }

        [HttpGet("ConsultChaveRedefinition/{chaveRedefinition}")]
        public IActionResult ConsultChaveRedefinition(string chaveRedefinition) {
            try {
                _usuarioService.ConsulteChaveRedefinition(chaveRedefinition);
                return Ok();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPut("RedefinirSenha")]
        [AllowAnonymous]
        public IActionResult RedefinirSenha(RedefinirSenha redefinirSenha) {
            try {
                if (ModelState.IsValid) {
                    if (redefinirSenha.NovaSenha != redefinirSenha.ConfirmarSenha) return BadRequest("Nova senha não pode ser diferente de confirmar senha!");
                    _usuarioService.RedefinirSenha(redefinirSenha);
                    return Ok();
                }
                return BadRequest(redefinirSenha);
            }
            catch (Exception error) {
                return NotFound(error.Message);
            }
        }

        [HttpPut("AlterarSenha")]
        [Authorize]
        public IActionResult AlterarSenha(AlterarSenha alterarSenha) {
            try {
                if (ModelState.IsValid) {
                    _usuarioService.AlterarSenha(alterarSenha);
                    return Ok();
                }
                return BadRequest(alterarSenha);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

    }
}
