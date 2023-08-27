using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Assistente, Administrador")]
    public class ClienteJuridicoController : ControllerBase {

        private readonly IClienteRepository _clienteRepository;

        public ClienteJuridicoController(IClienteRepository clienteRepository) {
            _clienteRepository = clienteRepository;
        }

        [HttpGet("Ativos/{paginaAtual}/{pesquisa?}")]
        public IActionResult GetClientesAtivos(int paginaAtual = 1, string? pesquisa = "") {
            List<PessoaJuridica> list = _clienteRepository.GetClientesAtivosPJ(paginaAtual, pesquisa);
            list = list.Select(x => { x.Telefone = x.ReturnTelefoneCliente(); return x; }).ToList();
            var response = new {
                clienteList = list,
                qtPaginas = _clienteRepository.QtPaginasClientesAtivosPJ(pesquisa)
            };
            return Ok(response);
        }

        [HttpGet("Inativos/{paginaAtual}/{pesquisa?}")]
        public IActionResult GetClientesInativos(int paginaAtual = 1, string? pesquisa = "") {
            List<PessoaJuridica> list = _clienteRepository.GetClientesInativosPJ(paginaAtual, pesquisa);
            list = list.Select(x => { x.Telefone = x.ReturnTelefoneCliente(); return x; }).ToList();
            var response = new {
                clienteList = list,
                qtPaginas = _clienteRepository.QtPaginasClientesInativosPJ(pesquisa)
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetClienteById(int? id) {
            try {
                PessoaJuridica pessoaJuridica = _clienteRepository.GetClienteByIdPJ(id);
                return Ok(pessoaJuridica);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }


        [HttpPost]
        public IActionResult CreateCliente(PessoaJuridica pessoaJuridica) {
            try {
                if (ModelState.IsValid) {
                    _clienteRepository.CreateClientePJ(pessoaJuridica);
                    return Ok(pessoaJuridica);
                }
                return BadRequest(pessoaJuridica);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateCliente(PessoaJuridica pessoaJuridica) {
            try {
                if (ModelState.IsValid) {
                    _clienteRepository.UpdateClientePJ(pessoaJuridica);
                    return Ok(pessoaJuridica);
                }
                return BadRequest(pessoaJuridica);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("Inativar/{id}")]
        public IActionResult InativarCliente(int? id) {
            try {
                _clienteRepository.InativarClientePJ(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("Ativar/{id}")]
        public IActionResult AtivarCliente(int? id) {
            try {
                _clienteRepository.AtivarClientePJ(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("ClientesAutorizados")]
        public IActionResult ClientesAutorizados() {
            var list = _clienteRepository.GetClientesParaVinculacaoPJ();
            return Ok(list);
        }

    }
}
