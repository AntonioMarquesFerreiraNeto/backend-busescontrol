using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ContratoController : ControllerBase {

        private readonly IContratoRepository _contratoRepository;

        public ContratoController(IContratoRepository contratoRepository) {
            _contratoRepository = contratoRepository;
        }

        [HttpPost]
        public IActionResult CreateContrato(Data data) {
            try {
                Contrato contrato = data.Contrato!;
                var lista = data.Lista;
                if (ModelState.IsValid) {
                    if (contrato.ValidarValorMonetario()) {
                        return BadRequest("Valor monetário menor que R$ 150.00!");
                    }
                    else if (contrato.ValidationDatas()) {
                        return BadRequest("Data de vencimento anterior à data de emissão!");
                    }
                    else if (contrato.ValidationDataEmissao()){
                        return BadRequest("Data de emissão não pode ser anterior ao dia atual!");
                    }
                    else if (contrato.ValidationDataVencimento()) {
                        return BadRequest("O contrato não pode ser superior a dois anos!");
                    }
                    else if (contrato.ValidationQtParcelas()) {
                        return BadRequest("Quantidade de parcelas inválida!");
                    }
                    _contratoRepository.CreateContrato(contrato, lista);
                    return Ok(contrato);
                }
                return BadRequest(contrato);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }
        public class Data {
            public Contrato? Contrato { get; set; }
            public List<ClientesContrato>? Lista { get; set; }
        }

        [HttpGet("GetContratosAtivos/{paginaAtual}/{statusPag}")]
        public IActionResult GetContratosAtivos(int paginaAtual, bool statusPag) {
            List<Contrato> contratos = _contratoRepository.GetContratosAtivos(paginaAtual, statusPag);
            var response = new {
                contractList = contratos,
                qtPaginas = _contratoRepository.ReturnQtPaginasAtivos()
            };
            return Ok(response);
        }
    }
}
