using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using iTextSharp.text;
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

        [HttpPut]
        public IActionResult EditLancamento(Financeiro financeiro) {
            try {
                if (ModelState.IsValid) {
                    if (financeiro.ValidarValorMonetario()) {
                        return BadRequest("Valor monetário menor que R$ 150.00!");
                    }
                    else if (financeiro.ValidationDatas()) {
                        return BadRequest("Data de vencimento anterior à data de emissão!");
                    }
                    else if (financeiro.ValidationDataVencimento()) {
                        return BadRequest("Financeiro não pode ser superior a quatro anos!");
                    }
                    else if (financeiro.ValidationQtParcelas()) {
                        return BadRequest("Quantidade de parcelas inválida!");
                    }
                    _financeiroRepository.EditarLancamento(financeiro);
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
                _financeiroRepository.TaskMonitorParcelas();
                _financeiroRepository.TaskMonitorParcelasLancamento();

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

        [HttpPatch("InativarFinanceiro/{id}")]
        public IActionResult Inativar(int id) {
            try {
                _financeiroRepository.InativarReceitaOrDespesa(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("GetFinanceiroById/{id}")]
        public IActionResult Contabilizacoes(int id) {
            try{
                var financeiro = _financeiroRepository.listPorIdFinanceiroNoJoinParcelas(id);
                return Ok(financeiro);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("ContabilizarPagamento/{id}")]
        public IActionResult Contabilizar(int id) {
            try {
                _financeiroRepository.ContabilizarParcela(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("PagAndFiltrosParcelas/{id}/{pageNumber}/{pesquisa?}")]
        public IActionResult PagAndFiltrosParcelas(int id, int pageNumber = 1, string? pesquisa = "") {
            var list = _financeiroRepository.GetPaginationAndFiltroParcelas(id, pageNumber, pesquisa);
            var data = new {
                listParcela = list,
                qtPaginas = _financeiroRepository.ReturnQtPaginasParcelas(id, pesquisa)
            };
            return Ok(data);
        }

        [HttpGet("GetPlanilhaFinanceiro")]
        public IActionResult GetPlanilhaExcelFinanceiro() {
            try {
                List<Financeiro> financeiros = _financeiroRepository.ListFinanceiros();
                if (financeiros == null) {
                    return NotFound("Desculpe, nenhum registro encontrado!");
                }
                using (var folhaBook = new XLWorkbook()) {
                    var folha = folhaBook.Worksheets.Add("Sample Sheet");
                    folha.Cell(1, "A").Value = "Código";
                    folha.Cell(1, "B").Value = "Contrato ID";
                    folha.Cell(1, "C").Value = "Credor/Devedor";
                    folha.Cell(1, "D").Value = "Status";
                    folha.Cell(1, "E").Value = "Receita/Despesa";
                    folha.Cell(1, "F").Value = "Valor total";
                    folha.Cell(1, "G").Value = "Valor efetuado";
                    folha.Cell(1, "H").Value = "Vencimento";
                    folha.Cell(1, "I").Value = "Pagamento";

                    //Definindo o tamanho das colunas. 
                    var col1 = folha.Column("A");
                    var col2 = folha.Column("B");
                    var col3 = folha.Column("C");
                    var col4 = folha.Column("D");
                    var col5 = folha.Column("E");
                    var col6 = folha.Column("F");
                    var col7 = folha.Column("G");
                    var col8 = folha.Column("H");
                    var col9 = folha.Column("I");

                    col1.Width = 10;
                    col2.Width = 15;
                    col3.Width = 40;
                    col4.Width = 20;
                    col5.Width = 20;
                    col6.Width = 20;
                    col7.Width = 20;
                    col8.Width = 20;
                    col9.Width = 20;

                    foreach (var financeiro in financeiros) {
                        folha.Cell(financeiros.IndexOf(financeiro) + 2, "A").Value = financeiro.Id;
                        if (!string.IsNullOrEmpty(financeiro.ContratoId.ToString())) {
                            folha.Cell(financeiros.IndexOf(financeiro) + 2, "B").Value = financeiro.ContratoId.ToString();
                        }
                        else {
                            folha.Cell(financeiros.IndexOf(financeiro) + 2, "B").Value = "Nulo";
                        }
                        folha.Cell(financeiros.IndexOf(financeiro) + 2, "C").Value = financeiro.ReturnNameClienteOrCredor();
                        folha.Cell(financeiros.IndexOf(financeiro) + 2, "D").Value = financeiro.ReturnStatusFinanceiro();
                        folha.Cell(financeiros.IndexOf(financeiro) + 2, "E").Value = financeiro.ReturnTypeFinanceiro();
                        folha.Cell(financeiros.IndexOf(financeiro) + 2, "F").Value = financeiro.ReturnValorTot();
                        folha.Cell(financeiros.IndexOf(financeiro) + 2, "G").Value = financeiro.ReturnValorTotEfetuado();
                        folha.Cell(financeiros.IndexOf(financeiro) + 2, "H").Value = financeiro.DataVencimento.Value.ToString("dd/MM/yyyy");
                        folha.Cell(financeiros.IndexOf(financeiro) + 2, "I").Value = financeiro.ReturnTypePagament();
                    }
                    using (MemoryStream stream = new MemoryStream()) {
                        folhaBook.SaveAs(stream);
                        string nomeArquivo = "Buses Control - Financeiro.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeArquivo);
                    }
                }
            }
            catch (Exception erro) {
                return BadRequest("Desculpe, houve um erro interno, notifique o problema para solucionarmos.");
            }
        }
    }
}
