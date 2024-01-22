using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncionarioController : ControllerBase {

        private readonly IFuncionarioRepository _funcionarioRepository;

        public FuncionarioController(IFuncionarioRepository funcionarioRepository) {
            _funcionarioRepository = funcionarioRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult CreateFuncionario(Funcionario funcionario) {
            try {
                if (ModelState.IsValid) {
                    _funcionarioRepository.CreateFuncionario(funcionario);
                    return Ok(funcionario);
                }
                return BadRequest(funcionario);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("PaginateAtivos/{paginaAtual}/{pesquisa?}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult GetFuncionariosAtivosPaginate(int paginaAtual = 1, string? pesquisa = "") {
            List<Funcionario> list = _funcionarioRepository.PaginateListAtivos(paginaAtual, pesquisa);
            list = list.Select(item => { item.Telefone = item.ReturnTelefoneFuncionario(); return item; }).ToList();
            var response = new {
                funciList = list,
                qtPaginate = _funcionarioRepository.QtPaginasAtivas(pesquisa)
            };
            return Ok(response);
        }

        [HttpGet("PaginateInativos/{paginaAtual}/{pesquisa?}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult GetFuncionariosInativosPaginate(int paginaAtual = 1, string? pesquisa = "") {
            List<Funcionario> list = _funcionarioRepository.PaginateListInativos(paginaAtual, pesquisa);
            list = list.Select(item => { item.Telefone = item.ReturnTelefoneFuncionario(); return item; }).ToList();
            var response = new {
                funciList = list,
                qtPaginate = _funcionarioRepository.QtPaginasInativas(pesquisa)
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult GetFuncionarioById(int? id) {
            try {
                Funcionario funcionario = _funcionarioRepository.GetFuncionarioById(id);
                return Ok(funcionario);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrador")]
        public IActionResult UpdateFuncionario(Funcionario funcionario) {
            try {
                if (ModelState.IsValid) {
                    _funcionarioRepository.UpdateFuncionario(funcionario);
                    return Ok(funcionario);
                }
                return BadRequest(funcionario);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("InativarFuncionario/{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult InativarFuncionario(int? id) {
            try {
                _funcionarioRepository.InativarFuncionario(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch ("AtivarFuncionario/{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult AtivarFuncionario(int? id) {
            try {
                _funcionarioRepository.AtivarFuncionario(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("InativarUsuario/{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult InativarUsuario(int? id) {
            try {
                _funcionarioRepository.InativarUsuario(id);
                return NoContent(); 
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }
        [HttpPatch("AtivarUsuario/{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult AtivarUsuario(int? id) {
            try {
                _funcionarioRepository.AtivarUsuario(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("MotoristasVinculacao")]
        [Authorize(Roles = "Assistente, Administrador")]
        public IActionResult GetMotoristasAll() {
            var list = _funcionarioRepository.GetAllMotoristas();
            return Ok(list);
        }

        [HttpGet("GetAllUsuarios")]
        public IActionResult GetAllUsuarios() {
            var list = _funcionarioRepository.GetAllUsuarios();
            return Ok(list);
        }
    }
}
