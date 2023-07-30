using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceiroController : ControllerBase {

        private readonly IFinanceiroRepository _financeiroRepository;

        public FinanceiroController(IFinanceiroRepository financeiroRepository) {
            _financeiroRepository = financeiroRepository;
        }

        [HttpPost]
        public IActionResult NewLancamento([FromBody] Financeiro financeiro) {
            try {
                if (ModelState.IsValid) {
                    if (financeiro.ValidarValorMonetario()) {
                        return BadRequest("Valor monetário menor que R$ 150.00!");
                    }
                    else if (financeiro.ValidationDatas()) {
                        return BadRequest("Data de vencimento anterior à data de emissão!");
                    }
                    else if (financeiro.ValidationDataEmissao()) {
                        return BadRequest("Data de emissão não pode ser diferente do dia atual!");
                    }
                    else if (financeiro.ValidationDataVencimento()) {
                        return BadRequest("Financeiro não pode ser superior a quatro anos!");
                    }
                    else if (financeiro.ValidationQtParcelas()) {
                        return BadRequest("Quantidade de parcelas inválida!");
                    }
                    _financeiroRepository.AdicionarLancamento(financeiro);
                    return Ok(financeiro);
                }
                return BadRequest(financeiro);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("GetFinanceiro/{pageNumber}/{pesquisa?}")]
        public IActionResult ListarFinanceiro(int pageNumber = 1,  string? pesquisa = "") {
            try {
                var list = _financeiroRepository.GetPaginationAndFiltro(pageNumber, pesquisa);
                var data = new {
                    listFinanceiro = list,
                    qtPaginas = _financeiroRepository.ReturnQtPaginas(pesquisa)
                };
                return Ok(data);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }
    }
}
