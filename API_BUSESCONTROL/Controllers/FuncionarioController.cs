using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom.Compiler;

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

        [HttpGet("PaginateAtivos/{paginaAtual}/{statusPaginacao}")]
        public IActionResult GetFuncionariosAtivosPaginate(int paginaAtual, bool statusPaginacao) {
            List<Funcionario> list = _funcionarioRepository.PaginateListAtivos(paginaAtual, statusPaginacao);
            list = list.Select(item => { item.Telefone = item.ReturnTelefoneFuncionario(); return item; }).ToList();
            var response = new {
                funciList = list,
                qtPaginate = _funcionarioRepository.QtPaginasAtivas()
            };
            return Ok(response);
        }

        [HttpGet("PaginateInativos/{paginaAtual}/{statusPaginacao}")]
        public IActionResult GetFuncionariosInativosPaginate(int paginaAtual, bool statusPaginacao) {
            List<Funcionario> list = _funcionarioRepository.PaginateListInativos(paginaAtual, statusPaginacao);
            list = list.Select(item => { item.Telefone = item.ReturnTelefoneFuncionario(); return item; }).ToList();
            var response = new {
                funciList = list,
                qtPaginate = _funcionarioRepository.QtPaginasInativas()
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
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
        public IActionResult GetMotoristasAll() {
            var list = _funcionarioRepository.GetAllMotoristas();
            return Ok(list);
        }
    }
}
