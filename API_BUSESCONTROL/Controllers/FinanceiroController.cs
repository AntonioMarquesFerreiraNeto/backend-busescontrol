using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Font = iTextSharp.text.Font;
using Microsoft.AspNetCore.Authorization;

namespace API_BUSESCONTROL.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
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

        [HttpGet("GetFinanceiro/{pageNumber}/{filtro}/{pageSize}/{pesquisa?}/")]
        public IActionResult ListarFinanceiro(int pageNumber = 1, FiltroFinanceiro filtro = FiltroFinanceiro.Todos, int pageSize = 10, string? pesquisa = "") {
            try {
                var list = _financeiroRepository.GetPaginationAndFiltro(pageNumber, pesquisa,  filtro, pageSize);

                var data = new {
                    listFinanceiro = list,
                    qtPaginas = _financeiroRepository.ReturnQtPaginas(pesquisa, filtro, pageSize)
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

        [HttpGet("GetPlanilhaFinanceiro/{filtro}/{pesquisa?}")]
        public IActionResult GetPlanilhaExcelFinanceiro(FiltroFinanceiro filtro = FiltroFinanceiro.Todos, string? pesquisa = "") {
            try {
                List<Financeiro> financeiros = _financeiroRepository.ListFinanceiroRelatorio(filtro, pesquisa);
                if (financeiros == null) {
                    return NotFound("Desculpe, nenhum registro encontrado.");
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
                    var colunas = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I"};
                    foreach (var coluna in colunas) {
                        var col = folha.Column(coluna);
                        col.Width = 20;
                        col.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                        var titulo = folha.Cell(1, coluna);
                        titulo.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        titulo.Style.Font.SetBold();
                        titulo.Style.Font.FontColor = XLColor.DarkBlue;
                    }
                    var col1 = folha.Column("A");
                    var col2 = folha.Column("B");
                    var col3 = folha.Column("C");
                    col1.Width = 10;
                    col2.Width = 15;
                    col3.Width = 40;

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
                return BadRequest("Desculpe, houve um erro interno, notifique o problema para solucionarmos." + $"{erro.Message}");
            }
        }

        [HttpGet("GetRelatorioFinanceiroPdf/{filtro}/{pesquisa?}")]
        public IActionResult PdfRelatorioFinanceiro(FiltroFinanceiro filtro = FiltroFinanceiro.Todos, string? pesquisa = "") {
            try {
                List<Financeiro> financeiros = _financeiroRepository.ListFinanceiroRelatorio(filtro, pesquisa);
                if (financeiros == null) {
                    return NotFound("Desculpe, nenhum registro encontrado.");
                }
                var pxPorMm = 72 / 35.2f;
                Document doc = new Document(PageSize.A4, 15 * pxPorMm, 15 * pxPorMm,
                    15 * pxPorMm, 15 * pxPorMm);
                MemoryStream stream = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                writer.CloseStream = false;
                doc.Open();
                var fonteBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                var fonteParagrafo = new iTextSharp.text.Font(fonteBase, 16,
                    iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
                Paragraph paragrofoJustificado = new Paragraph("",
                new Font(fonteBase, 10, Font.NORMAL));
                Paragraph paragrofoRodape = new Paragraph("",
                new Font(fonteBase, 09, Font.NORMAL));
                paragrofoJustificado.Alignment = Element.ALIGN_JUSTIFIED;
                var titulo = new Paragraph($"Relatório financeiro\n\n\n", fonteParagrafo);
                titulo.Alignment = Element.ALIGN_CENTER;

                var caminhoImgLeft = Path.Combine("ImagensPDF", "LogoPdf.jpeg");
                if (caminhoImgLeft != null) {
                    Image logo = Image.GetInstance(caminhoImgLeft);
                    float razaoImg = logo.Width / logo.Height;
                    float alturaImg = 84;
                    float larguraLogo = razaoImg * alturaImg - 6f;
                    logo.ScaleToFit(larguraLogo, alturaImg);
                    var margemEsquerda = doc.PageSize.Width - doc.RightMargin - larguraLogo - 2;
                    var margemTopo = doc.PageSize.Height - doc.TopMargin - 60;
                    logo.SetAbsolutePosition(margemEsquerda, margemTopo);
                    writer.DirectContent.AddImage(logo, false);
                }
                var caminhoImgRight = Path.Combine("ImagensPDF", "LogoPdfRight.jpg");
                if (caminhoImgRight != null) {
                    Image logo2 = Image.GetInstance(caminhoImgRight);
                    float razaoImg = logo2.Width / logo2.Height;
                    float alturaImg = 84;
                    float larguraLogo = razaoImg * alturaImg - 6f;
                    logo2.ScaleToFit(larguraLogo, alturaImg);
                    var margemRight = pxPorMm * 15;
                    var margemTopo = doc.PageSize.Height - doc.TopMargin - 60;
                    logo2.SetAbsolutePosition(margemRight, margemTopo);
                    writer.DirectContent.AddImage(logo2, false);
                }
                var tabela = new PdfPTable(7);
                float[] larguraColunas = { 0.5f, 1f, 1f, 1f, 1f, 1f, 1f };
                tabela.SetWidths(larguraColunas);
                tabela.DefaultCell.BorderWidth = 0;
                tabela.WidthPercentage = 105;
                CriarCelulaTexto(tabela, "ID", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Credor/Devedor", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Status financeiro", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Receita/Despesas", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Val total", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Val efetuado", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Vencimento", PdfPCell.ALIGN_LEFT, true);
                decimal valorTotAtivos = 0, valorTotInativos = 0, valEfetuado = 0;
                foreach (var item in financeiros.OrderBy(x => x.DespesaReceita)) {
                    if (item.FinanceiroStatus == FinanceiroStatus.Ativo) {
                        valorTotAtivos += item.ValorTotDR!.Value;
                    }
                    else {
                        valorTotInativos += item.ValorTotDR!.Value;
                    }
                    if (!string.IsNullOrEmpty(item.ValorTotalPago.ToString())) {
                        valEfetuado += item.ValorTotalPago!.Value;
                    }
                    CriarCelulaTexto(tabela, item.Id.ToString(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.ReturnNameClienteOrCredor(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.ReturnStatusFinanceiro(), PdfPCell.ALIGN_CENTER);
                    CriarCelulaTexto(tabela, item.ReturnTypeFinanceiro(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.ReturnValorTot(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.ReturnValorTotEfetuado(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.DataVencimento!.Value.ToString("dd/MM/yyyy"), PdfPCell.ALIGN_LEFT);
                }

                Paragraph footer = new Paragraph($"Data de emissão do documento: {DateTime.Now:dd/MM/yyyy}", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                //footer.Alignment = Element.ALIGN_LEFT;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.WidthPercentage = 100f;
                footerTbl.TotalWidth = 1000f;
                footerTbl.HorizontalAlignment = 0;
                PdfPCell cell = new PdfPCell(footer);
                cell.Border = 0;
                cell.Colspan = 1;
                cell.PaddingLeft = 0;
                cell.HorizontalAlignment = 0;
                footerTbl.DefaultCell.HorizontalAlignment = 0;
                footerTbl.WidthPercentage = 100;
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -30, 350, 30, writer.DirectContent);

                string rodape = $"Quantidade de lançamentos solicitados: {financeiros.Count}" +
                    $"\nValor total listado (ativos): {valorTotAtivos.ToString("C2")}" +
                    $"\nValor total listado (inativos): {valorTotInativos.ToString("C2")}" +
                    $"\nValor total efetuado: {valEfetuado.ToString("C2")}" +
                    $"\nFiltro aplicado: {ReturnFiltroString(filtro)}" + 
                    $"\nPesquisa aplicada: {(!string.IsNullOrEmpty(pesquisa) ? pesquisa : "nenhuma")}";
                string rodape2 = $"\nDocumento gerado em: {DateTime.Now.ToString("dd/MM/yyyy")}";
                paragrofoRodape.Add(rodape);
                paragrofoRodape.Add(rodape2);
                doc.Add(titulo);
                doc.Add(paragrofoJustificado);
                doc.Add(tabela);
                doc.Add(paragrofoRodape);
                doc.Close();

                string nomeContrato = $"relatório financeiro";
                stream.Flush();
                stream.Position = 0;
                return File(stream, "application/pdf", $"{nomeContrato}.pdf");
            }
            catch (Exception error) {
                return BadRequest("Desculpe, houve um erro interno, notifique o problema para solucionarmos." + $"{error.Message}");
            }
        }
        private static string ReturnFiltroString(FiltroFinanceiro filtro) {
            switch (filtro) {
                case FiltroFinanceiro.Todos: return "Todos";
                case FiltroFinanceiro.Contrato: return "Contratos";
                case FiltroFinanceiro.Atrasadas: return "Atrasados";
                case FiltroFinanceiro.Receita: return "Receitas";
                default: return "Despesas";
            }
        }

        [HttpGet("GetRelatorioParcelas/{financeiroId}")]
        public IActionResult GetRelatorioParcelas(int financeiroId) {
            try {
                Financeiro financeiro = _financeiroRepository.listPorIdFinanceiro(financeiroId);
                if (financeiro == null || !financeiro.Parcelas.Any()) return NotFound("Desculpe, não encontramos nenhum registro.");
                using (var folhaBook = new XLWorkbook()) {
                    var folha = folhaBook.AddWorksheet("Sample sheet");

                    folha.Cell(1, "A").Value = "Parcela";
                    folha.Cell(1, "B").Value = "Situação";
                    folha.Cell(1, "C").Value = "Valor da parcela";
                    folha.Cell(1, "D").Value = "Taxa de juros";
                    folha.Cell(1, "E").Value = "Data de vencimento";
                    folha.Cell(1, "F").Value = "Data de efetuação";

                    var colunas = new[] { "A", "B", "C", "D", "E", "F" };

                    foreach (var item in colunas) {
                        var col = folha.Column(item);
                        col.Width = 20;
                        col.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                        var titulo = folha.Cell(1, item);
                        titulo.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        titulo.Style.Font.SetBold();
                        titulo.Style.Font.FontColor = XLColor.DarkBlue;
                    }
                    var col2 = folha.Column("B");
                    col2.Width = 30;

                    foreach (var item in financeiro.Parcelas!) {
                        folha.Cell(financeiro.Parcelas.IndexOf(item) + 2, "A").Value = $"{item.ReturnNomeParcela()}";
                        folha.Cell(financeiro.Parcelas.IndexOf(item) + 2, "B").Value = $"{item.ReturnStatusPagamento()}";
                        folha.Cell(financeiro.Parcelas.IndexOf(item) + 2, "C").Value = $"{financeiro.ValorParcelaDR!.Value.ToString("C2")}";
                        folha.Cell(financeiro.Parcelas.IndexOf(item) + 2, "D").Value = (!string.IsNullOrEmpty(item.ValorJuros.ToString())) ? $"{item.ValorJuros!.Value.ToString("C2")}" : "R$ 0,00";
                        folha.Cell(financeiro.Parcelas.IndexOf(item) + 2, "E").Value = item.DataVencimentoParcela!.Value.ToString("dd/MM/yyyy");
                        folha.Cell(financeiro.Parcelas.IndexOf(item) + 2, "F").Value = (!string.IsNullOrEmpty(item.DataEfetuacao.ToString())) ? item.DataEfetuacao!.Value.ToString("dd/MM/yyyy") : "Não possui";
                    }
                    using (MemoryStream stream = new MemoryStream()) {
                        folhaBook.SaveAs(stream);
                        string nomeArquivo = $"Parcelas - {financeiro.ReturnNameClienteOrCredor()}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeArquivo);
                    }
                }
            }
            catch (Exception error) {
                return BadRequest("Desculpe, houve um erro interno, notifique o problema para solucionarmos." + $"{error.Message}");
            }
        }

        [HttpGet("GetRelatorioParcelasPdf/{financeiroId}")]
        public IActionResult GetRelatorioParcelasPdf(int? financeiroId) {
            try {
                Financeiro financeiro = _financeiroRepository.listPorIdFinanceiro(financeiroId);
                if (financeiro == null) {
                    return NotFound("Desculpe, nenhum registro encontrado.");
                }
                var pxPorMm = 72 / 35.2f;
                Document doc = new Document(PageSize.A4, 15 * pxPorMm, 15 * pxPorMm,
                    15 * pxPorMm, 15 * pxPorMm);
                MemoryStream stream = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                writer.CloseStream = false;
                doc.Open();
                var fonteBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                var fonteParagrafo = new iTextSharp.text.Font(fonteBase, 16,
                    iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
                Paragraph paragrofoJustificado = new Paragraph("",
                new Font(fonteBase, 10, Font.NORMAL));
                Paragraph paragrofoRodape = new Paragraph("",
                new Font(fonteBase, 09, Font.NORMAL));
                paragrofoJustificado.Alignment = Element.ALIGN_JUSTIFIED;
                var titulo = new Paragraph($"Parcelas - {financeiro.ReturnNameClienteOrCredor()}\n\n\n", fonteParagrafo);
                titulo.Alignment = Element.ALIGN_LEFT;

                var caminhoImgLeft = Path.Combine("ImagensPDF", "LogoPdf.jpeg");
                if (caminhoImgLeft != null) {
                    Image logo = Image.GetInstance(caminhoImgLeft);
                    float razaoImg = logo.Width / logo.Height;
                    float alturaImg = 84;
                    float larguraLogo = razaoImg * alturaImg - 6f;
                    logo.ScaleToFit(larguraLogo, alturaImg);
                    var margemEsquerda = doc.PageSize.Width - doc.RightMargin - larguraLogo - 2;
                    var margemTopo = doc.PageSize.Height - doc.TopMargin - 60;
                    logo.SetAbsolutePosition(margemEsquerda, margemTopo);
                    writer.DirectContent.AddImage(logo, false);
                }

                var tabela = new PdfPTable(7);
                float[] larguraColunas = { 0.4f, 0.7f, 1.1f, 1f, 1f, 1f, 1.3f };
                tabela.SetWidths(larguraColunas);
                tabela.DefaultCell.BorderWidth = 0;
                tabela.WidthPercentage = 105;
                CriarCelulaTexto(tabela, "ID", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Nome", PdfPCell.ALIGN_CENTER, true);
                CriarCelulaTexto(tabela, "Valor das parcelas", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Taxa de juros", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Vencimento", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Efetuação", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Status da parcela", PdfPCell.ALIGN_CENTER, true);

                foreach (var item in financeiro.Parcelas.OrderBy(x => x.DataVencimentoParcela)) {
                    CriarCelulaTexto(tabela, item.Id.ToString(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.ReturnNomeParcela(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.Financeiro.ReturnValorParcela(), PdfPCell.ALIGN_CENTER);
                    CriarCelulaTexto(tabela, item.ReturnValorJuros(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.ReturnDateVencimento(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.ReturnDateEfetuacao(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.ReturnStatusPagamento(), PdfPCell.ALIGN_LEFT);
                }

                Paragraph footer = new Paragraph($"Data de emissão do documento: {DateTime.Now:dd/MM/yyyy}", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                //footer.Alignment = Element.ALIGN_LEFT;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.WidthPercentage = 100f;
                footerTbl.TotalWidth = 1000f;
                footerTbl.HorizontalAlignment = 0;
                PdfPCell cell = new PdfPCell(footer);
                cell.Border = 0;
                cell.Colspan = 1;
                cell.PaddingLeft = 0;
                cell.HorizontalAlignment = 0;
                footerTbl.DefaultCell.HorizontalAlignment = 0;
                footerTbl.WidthPercentage = 100;
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -30, 350, 30, writer.DirectContent);

                string rodape = $"Quantidade de parcelas: {financeiro.Parcelas.Count} " +
                                $"\nValor efetuado: {financeiro.ReturnValorTotEfetuado()}" +
                                $"\nValor total: {financeiro.ReturnValorTot()}";
                paragrofoRodape.Add(rodape);
                doc.Add(titulo);
                doc.Add(paragrofoJustificado);
                doc.Add(tabela);
                doc.Add(paragrofoRodape);
                doc.Close();

                string nomeContrato = $"Parcelas - {financeiro.ReturnNameClienteOrCredor()}";
                stream.Flush();
                stream.Position = 0;
                return File(stream, "application/pdf", $"{nomeContrato}.pdf");
            }
            catch (Exception erro) {
                return BadRequest($"Desculpe, houve um erro: {erro.Message}");
            }
        }


        static void CriarCelulaTexto(PdfPTable tabela, string texto, int alinhamentoHorz = PdfPCell.ALIGN_LEFT,
                bool negrito = false, bool italico = false, int tamanhoFont = 10, int alturaCelula = 30) {

            int estilo = Font.NORMAL;
            if (negrito && italico) {
                estilo = Font.BOLDITALIC;
            }
            else if (negrito) {
                estilo = Font.BOLD;
            }
            else if (italico) {
                estilo = Font.ITALIC;
            }
            var fonteBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            var fonteCelula = new Font(fonteBase, tamanhoFont, estilo, BaseColor.DARK_GRAY);
            var bgColor = BaseColor.WHITE;
            if (tabela.Rows.Count % 2 == 1) {
                bgColor = new BaseColor(0.95f, 0.95f, 0.95f);
            }
            var celula = new PdfPCell(new Phrase(texto, fonteCelula));
            celula.HorizontalAlignment = alinhamentoHorz;
            celula.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            celula.Border = 0;
            celula.BorderWidthBottom = 1;
            celula.BackgroundColor = bgColor;
            celula.FixedHeight = alturaCelula;
            tabela.AddCell(celula);
        }
    }
}
