﻿using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Assistente, Administrador")]
    public class ClienteController : ControllerBase {

        private readonly IClienteRepository _clienteRepository;

        public ClienteController(IClienteRepository clienteRepository) {
            _clienteRepository = clienteRepository;
        }

        [HttpGet("Ativos/{paginaAtual}/{pesquisa?}")]
        public IActionResult GetPessoaFisica(int paginaAtual = 1, string? pesquisa = "") {
            List<PessoaFisica> list = _clienteRepository.GetClientesAtivos(paginaAtual, pesquisa);
            list = list.Select(x => { x.Telefone = x.ReturnTelefoneCliente(); return x; }).ToList();
            var response = new {
                clienteList = list,
                qtPaginas = _clienteRepository.QtPaginasClientesAtivos(pesquisa)
            };
            return Ok(response);
        }

        [HttpGet("Inativos/{paginaAtual}/{pesquisa?}")]
        public IActionResult GetPessoasInativas(int paginaAtual = 1, string? pesquisa = "") {
            List<PessoaFisica> list = _clienteRepository.GetClientesInativos(paginaAtual, pesquisa);
            list = list.Select(x => { x.Telefone = x.ReturnTelefoneCliente(); return x; }).ToList();
            var response = new {
                clienteList = list,
                qtPaginas = _clienteRepository.QtPaginasClientesInativos(pesquisa)
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetClienteById(int? id) {
            try {
                PessoaFisica cliente = _clienteRepository.GetClienteFisicoById(id);
                return Ok(cliente);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateCliente(PessoaFisica pessoaFisica) {
            try {
                if (ModelState.IsValid) {
                    if (pessoaFisica.ValidationMenorIdade() && string.IsNullOrEmpty(pessoaFisica.IdVinculacaoContratual.ToString())) {
                        return BadRequest("Cliente menor de idade sem vínculo contratual!");
                    }
                    if (!pessoaFisica.ValidationMenorIdade() && !string.IsNullOrEmpty(pessoaFisica.IdVinculacaoContratual.ToString())) {
                        return BadRequest("Não é possível vincular cliente maior de idade!");
                    }
                    _clienteRepository.CreateCliente(pessoaFisica);
                    return Ok(pessoaFisica);
                }
                return BadRequest(pessoaFisica);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateCliente(PessoaFisica pessoaFisica) {
            try {
                if (ModelState.IsValid) {
                    if (pessoaFisica.ValidationMenorIdade() && string.IsNullOrEmpty(pessoaFisica.IdVinculacaoContratual.ToString())) {
                        return BadRequest("Cliente menor de idade sem vínculo contratual!");
                    }
                    if (!pessoaFisica.ValidationMenorIdade() && !string.IsNullOrEmpty(pessoaFisica.IdVinculacaoContratual.ToString())) {
                        return BadRequest("Não é possível vincular cliente maior de idade!");
                    }
                    _clienteRepository.UpdateCliente(pessoaFisica);
                    return Ok(pessoaFisica);
                }
                return BadRequest(pessoaFisica);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }
        

        [HttpPatch("InativarCliente/{id}")]
        public IActionResult InativarCliente(int? id) {
            try {
                _clienteRepository.InativarCliente(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("AtivarCliente/{id}")]
        public IActionResult AtivarCliente(int? id) {
            try {
                _clienteRepository.AtivarCliente(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("ClientesAutorizados")]
        public IActionResult ClientesAutorizados() {
            var list = _clienteRepository.GetClientesParaVinculacao();
            return Ok(list);
        }

        [HttpGet("ClientesAdimplentes")]
        public IActionResult ClientesAdimplentes() {
            var list = _clienteRepository.GetClientesAdimplentes();
            return Ok(list);
        }

        [HttpGet("ClientesAdimplentesContratoEdit/{contratoId}")]
        public IActionResult ClientesContractEditAdimplentes(int contratoId) {
            var list = _clienteRepository.GetClientesContractEditAdimplentes(contratoId);
            return Ok(list);
        }

        [HttpGet("ClienteResponsavel/{id}")]
        public IActionResult ClienteResponsavel(int id) {
            var cliente = _clienteRepository.ClienteResponsavel(id);
            return Ok(cliente);
        }
    }
}
