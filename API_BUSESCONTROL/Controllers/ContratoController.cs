using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics.Contracts;

namespace API_BUSESCONTROL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContratoController : ControllerBase {

        private readonly IContratoRepository _contratoRepository;

        public ContratoController(IContratoRepository contratoRepository) {
            _contratoRepository = contratoRepository;
        }

        [HttpPost]
        public IActionResult CreateContrato(DataResponse data) {
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
                    else if (contrato.ValidationDataEmissao()) {
                        return BadRequest("Data de emissão não pode ser diferente do dia atual!");
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

        [HttpPut]
        public IActionResult UpdateContrato(DataResponse data) {
            try {
                Contrato contrato = data.Contrato;
                if (ModelState.IsValid) {
                    if (contrato!.ValidarValorMonetario()) {
                        return BadRequest("Valor monetário menor que R$ 150.00!");
                    }
                    else if (contrato.ValidationDatas()) {
                        return BadRequest("Data de vencimento anterior à data de emissão!");
                    }
                    else if (contrato.ValidationDataVencimento()) {
                        return BadRequest("O contrato não pode ser superior a dois anos!");
                    }
                    else if (contrato.ValidationQtParcelas()) {
                        return BadRequest("Quantidade de parcelas inválida!");
                    }
                    _contratoRepository.UpdateContrato(contrato, data.Lista);
                    return Ok(contrato);
                }
                return BadRequest(contrato);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }

        }

        [HttpGet("{id}")]
        public ActionResult GetContratoById(int id) {
            try {
                Contrato contrato = _contratoRepository.GetContratoById(id);
                return Ok(contrato);
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
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

        [HttpGet("GetContratosInativos/{paginaAtual}/{statusPag}")]
        public IActionResult GetContratosInativos(int paginaAtual, bool statusPag) {
            List<Contrato> contratos = _contratoRepository.GetContratosInativos(paginaAtual, statusPag);
            var response = new {
                contractList = contratos,
                qtPaginas = _contratoRepository.ReturnQtPaginasInativos()
            };
            return Ok(response);
        }

        [HttpPatch("Aprovar/{id}")]
        public IActionResult AprovarContrato(int id) {
            try {
                _contratoRepository.AprovarContrato(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("Revogar/{id}")]
        public IActionResult RevogarContrato(int id) {
            try {
                _contratoRepository.RevogarContrato(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpPatch("Inativar/{id}")]
        public IActionResult InativarContrato(int id) {
            try {
                _contratoRepository.InativarContrato(id);
                return NoContent();
            }
            catch (Exception error) {
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet("RelatorioExcel/{ativo}")]
        public IActionResult ReturnPlanilhaExcel(bool ativo) {
            try {
                List<Contrato> contratos = (ativo) ? _contratoRepository.GetAllContratosAtivos() : _contratoRepository.GetAllContratosInativos();
                if (contratos == null || contratos.Count == 0) return NotFound("Nenhum registro encontrado!");

                using (var folhaBook = new XLWorkbook()) {
                    var folha = folhaBook.AddWorksheet("sample sheet");

                    folha.Cell(1, "A").Value = "ID";
                    folha.Cell(1, "B").Value = "Qt. clientes";
                    folha.Cell(1, "C").Value = "Vencimento";
                    folha.Cell(1, "D").Value = "Valor total";
                    folha.Cell(1, "E").Value = "Pagamento";
                    folha.Cell(1, "F").Value = "Aprovação";
                    folha.Cell(1, "G").Value = "Andamento";

                    var colunas = new[] { "A", "B", "C", "D", "E", "F", "G" };

                    foreach (var coluna in colunas) {
                        var col = folha.Column(coluna);
                        col.Width = 20;
                        col.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                        var titulo = folha.Cell(1, coluna);
                        titulo.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        titulo.Style.Font.SetBold();
                        titulo.Style.Font.FontColor = XLColor.Blue;
                    }


                    foreach (var item in contratos) {
                        folha.Cell(contratos.IndexOf(item) + 2, "A").Value = item.Id;
                        folha.Cell(contratos.IndexOf(item) + 2, "B").Value = item.ClientesContrato!.Count;
                        folha.Cell(contratos.IndexOf(item) + 2, "C").Value = item.DataVencimento!.Value.ToString("dd/MM/yyyy");
                        folha.Cell(contratos.IndexOf(item) + 2, "D").Value = item.ValorMonetario!.Value.ToString("C2");
                        folha.Cell(contratos.IndexOf(item) + 2, "E").Value = (item.Pagament == 0) ? "Parcelado" : "À vista";
                        folha.Cell(contratos.IndexOf(item) + 2, "F").Value = ReturnAprovacao(item.Aprovacao);
                        folha.Cell(contratos.IndexOf(item) + 2, "G").Value = ReturnAndamento(item.Andamento);
                    }
                    string ReturnAprovacao(StatusAprovacao status) {
                        switch (status) {
                            case StatusAprovacao.EmAnalise: return "Em análise";
                            case StatusAprovacao.Negado: return "Negado";
                            case StatusAprovacao.Aprovado: return "Aprovado";
                            default: return "Status não encontrado.";
                        }
                    }
                    string ReturnAndamento(Andamento andamento) {
                        switch (andamento) {
                            case Andamento.Aguardando: return "Aguardando";
                            case Andamento.EmAndamento: return "Em andamento";
                            case Andamento.Encerrado: return "Encerrado";
                            default: return "status não encontrado";
                        }

                    }

                    using (MemoryStream stream = new MemoryStream()) {
                        folhaBook.SaveAs(stream);
                        string nameContrato = (ativo) ? "Contratos ativos" : "Contratos inativos";
                        string nomeArquivo = $"Buses control - {nameContrato}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeArquivo);
                    }
                }
            }
            catch (Exception error) {
                return BadRequest(error.Message);
            }
        }
    }
}
