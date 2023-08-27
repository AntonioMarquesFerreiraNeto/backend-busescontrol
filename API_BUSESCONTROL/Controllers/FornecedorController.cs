using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Assistente, Administrador")]
    public class FornecedorController : ControllerBase {

        private readonly IFornecedorRepository _fornecedorRepository;

        public FornecedorController(IFornecedorRepository fornecedorRepository) {
            _fornecedorRepository = fornecedorRepository;
        }

        [HttpPost]
        public IActionResult CreateFornecedor(Fornecedor fornecedor) {
            try {
                if (ModelState.IsValid) {
                    if (fornecedor.TypePessoa == TypePessoa.PessoaFisica) {
                        if (!fornecedor.ValidarCpf(fornecedor.Cpf)) return BadRequest("Cpf inválido!");
                    }
                    else {
                        if (!fornecedor.ValidaCNPJ(fornecedor.Cnpj)) return BadRequest("Cnpj inválido!");
                    }
                    if (fornecedor.ValidationDate()) {
                        return BadRequest("Por favor, informe uma data válida!");
                    }
                    _fornecedorRepository.CreateFornecedor(fornecedor);
                    return Ok(fornecedor);
                }
                return BadRequest(fornecedor);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateFornecedor(Fornecedor fornecedor) {
            try {
                if (ModelState.IsValid) {
                    if (fornecedor.TypePessoa == TypePessoa.PessoaFisica) {
                        if (!fornecedor.ValidarCpf(fornecedor.Cpf)) return BadRequest("Cpf inválido!");
                    }
                    else {
                        if (!fornecedor.ValidaCNPJ(fornecedor.Cnpj)) return BadRequest("Cnpj inválido!");
                    }
                    if (fornecedor.ValidationDate()) {
                        return BadRequest("Por favor, informe uma data válida!");
                    }
                    _fornecedorRepository.UpdateFornecedor(fornecedor);
                    return Ok(fornecedor);
                }
                return BadRequest(fornecedor);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("GetAtivos/{paginaAtual}/{filtro}/{pesquisa?}")]
        public IActionResult GetFornecedores(int paginaAtual = 1, int filtro = 2, string? pesquisa = "") {
            List<Fornecedor> fornecedores = _fornecedorRepository.GetFornecedoresAtivos(paginaAtual, filtro, pesquisa);
            fornecedores = fornecedores.Select(x => { x.Telefone = x.ReturnTelefoneFornecedor(); return x; }).ToList();
            var response = new {
                fornecedorList = fornecedores,
                qtPaginas = _fornecedorRepository.GetTotPaginasAtivos(pesquisa, filtro)
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetFornecedorById(int id) {
            try {
                Fornecedor fornecedor = _fornecedorRepository.GetFornecedorById(id);
                return Ok(fornecedor);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("Inativar/{id}")]
        public IActionResult InativarFornecedor(int id) {
            try {
                _fornecedorRepository.InativarFornecedor(id);
                return NoContent();    
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("Ativar/{id}")]
        public IActionResult AtivarFornecedor(int id) {
            try {
                _fornecedorRepository.AtivarFornecedor(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("GetInativos/{paginaAtual}/{filtro}/{pesquisa?}")]
        public IActionResult GetInativos(int paginaAtual = 1, int filtro = 2, string? pesquisa = "") {
            var fornecedores = _fornecedorRepository.GetFornecedoresInativos(paginaAtual, filtro, pesquisa);
            fornecedores = fornecedores.Select(x => { x.Telefone = x.ReturnTelefoneFornecedor(); return x; }).ToList();  
            var response = new {
                fornecedorList = fornecedores,
                qtPaginas = _fornecedorRepository.GetTotPaginasInativos(pesquisa, filtro)
            };
            return Ok(response);
        }

        [HttpGet("FornecedoresAutorizados")]
        public IActionResult GetAllFornecedores() {
            var list = _fornecedorRepository.GetAllFornecedoresAutorizados();
            return Ok(list);
        }
    }
}
