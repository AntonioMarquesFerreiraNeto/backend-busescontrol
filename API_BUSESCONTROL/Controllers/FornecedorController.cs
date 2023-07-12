using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers {
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("GetAtivos/{paginaAtual}/{statusPag}")]
        public IActionResult GetFornecedores(int paginaAtual, bool statusPag) {
            List<Fornecedor> fornecedores = _fornecedorRepository.GetFornecedoresAtivos(paginaAtual, statusPag);
            fornecedores = fornecedores.Select(x => { x.Telefone = x.ReturnTelefoneFornecedor(); return x; }).ToList();
            var response = new {
                fornecedorList = fornecedores,
                qtPaginas = _fornecedorRepository.GetTotPaginasAtivos()
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

        [HttpGet("GetInativos/{paginaAtual}/{status}")]
        public IActionResult GetInativos(int paginaAtual, bool status) {
            var fornecedores = _fornecedorRepository.GetFornecedoresInativos(paginaAtual, status);
            fornecedores = fornecedores.Select(x => { x.Telefone = x.ReturnTelefoneFornecedor(); return x; }).ToList();  
            var response = new {
                fornecedorList = fornecedores,
                qtPaginas = _fornecedorRepository.GetTotPaginasInativos()
            };
            return Ok(response);
        }
    }
}
