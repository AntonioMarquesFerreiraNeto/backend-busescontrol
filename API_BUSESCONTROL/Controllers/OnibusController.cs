﻿using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API_BUSESCONTROL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnibusController : ControllerBase {

        private readonly IOnibusRepository _onibusRepository;

        public OnibusController(IOnibusRepository onibusRepository) {
            _onibusRepository = onibusRepository;
        }


        [HttpGet("PaginateListAtivos/{paginaAtual}/{statusPaginacao}")]
        public IActionResult GetOnibusAtivosPaginate(int paginaAtual, bool statusPaginacao) {
            var onibusList = _onibusRepository.PaginateListAtivos(paginaAtual, statusPaginacao);
            var qtPaginate = _onibusRepository.QtPaginasAtivas();
            var response = new {
                OnibusList = onibusList,
                QtPaginate = qtPaginate
            };
            return Ok(response);
        }
        [HttpGet("PaginateListInativos/{paginaAtual}/{statusPaginacao}")]
        public IActionResult GetOnibusInativosPaginate(int paginaAtual, bool statusPaginacao) {
            var onibusList = _onibusRepository.PaginateListInativos(paginaAtual, statusPaginacao);
            var qtPaginate = _onibusRepository.QtPaginasInativas();
            var response = new {
                OnibusList = onibusList,
                QtPaginate = qtPaginate
            };
            return Ok(response);
        }

        [HttpPost]
        public IActionResult CreateOnibus(Onibus onibus) {
            try {
                if (ModelState.IsValid) {
                    _onibusRepository.CreateOnibus(onibus);
                    return Ok(onibus);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetOnibusById(int id) {
            try {
                var onibus = _onibusRepository.GetOnibusById(id);
                return Ok(onibus);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateOnibus(Onibus onibus) {
            try {
                if (ModelState.IsValid) {
                    _onibusRepository.UpdateOnibus(onibus);
                    return Ok(onibus);
                }
                return BadRequest(ModelState);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("Inativar/{id}")]
        public IActionResult InativarOnibus(int id) {
            try {
                _onibusRepository.InativarOnibus(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }
        [HttpPatch("Ativar/{id}")]
        public IActionResult AtivarOnibus(int? id) {
            try {
                _onibusRepository.AtivarOnibus(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteOnibus(int id) {
            try {
                var existingOnibus = _onibusRepository.GetOnibusById(id);
                if (existingOnibus == null) {
                    return NotFound("Desculpe, ônibus não encontrado!");
                }
                _onibusRepository.DeleteOnibus(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("OnibusVinculacao")]
        public IActionResult GetMotoristasAll() {
            var list = _onibusRepository.GetAll();
            return Ok(list);
        }
    }
}




