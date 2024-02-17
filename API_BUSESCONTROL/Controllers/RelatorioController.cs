using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API_BUSESCONTROL.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class RelatorioController : ControllerBase {
        
        private readonly IRelatorioRepository _relatorioRepository;

        public RelatorioController(IRelatorioRepository relatorioRepository) {
            _relatorioRepository = relatorioRepository;
        }

        [HttpGet("ReturnRelatorio")]
        public  IActionResult ReturnRelatorio() {
            try {
                Relatorio relatorio = new Relatorio();
                relatorio.ValTotAprovados = _relatorioRepository.ValorTotAprovados();
                relatorio.ValTotEmAnalise = _relatorioRepository.ValorTotEmAnalise();
                relatorio.ValTotReprovados = _relatorioRepository.ValorTotReprovados();
                relatorio.ValTotReceitas = _relatorioRepository.ValorTotReceitas();
                relatorio.ValTotDespesas = _relatorioRepository.ValorTotDespesas();
                relatorio.ValTotEfetuadoReceita = _relatorioRepository.ValorTotPagoReceitas();
                relatorio.ValTotEfetuadoDespesa = _relatorioRepository.ValorTotPagoDespesas();
                relatorio.ValTotContratos = relatorio.ValTotAprovados + relatorio.ValTotEmAnalise;
                relatorio.ValTotPago = _relatorioRepository.ValorTotPagoContrato();
                relatorio.ValTotPendente = relatorio.ReturnValorPendente();
                relatorio.ValorJurosAndMultas = _relatorioRepository.ValorJurosAndMultas();
                relatorio.ValorReceitasComuns = _relatorioRepository.ValorReceitasComuns();
                relatorio.QtContratos = _relatorioRepository.QtContratos();
                relatorio.QtContratosEncerrados = _relatorioRepository.QtContratosEncerrados();
                relatorio.QtContratosAprovados = _relatorioRepository.QtContratosAprovados();
                relatorio.QtContratosNegados = _relatorioRepository.QtContratosNegados();
                relatorio.QtContratosEmAnalise = _relatorioRepository.QtContratosEmAnalise();
                relatorio.QtClientes = _relatorioRepository.QtClientes();
                relatorio.QtClientesAdimplente = _relatorioRepository.QtClientesAdimplentes();
                relatorio.QtClientesInadimplente = _relatorioRepository.QtClientesInadimplentes();
                relatorio.QtClientesVinculados = _relatorioRepository.QtClientesVinculados();
                relatorio.SimpleAnalytics = _relatorioRepository.ReturnSimpleAnalytics();
                return Ok(relatorio);
            }
            catch (Exception error) {
                return BadRequest($"Desculpe, houve um conflito ao processar sua solicitação. Detalhes do erro: {error.Message}");
            }
        }

        [HttpGet("ListContratosAprovados/{statusAndamento}/{pesquisa?}")]
        public IActionResult ListContratos(int statusAndamento = 0, string? pesquisa = "") {
            var listContratos = _relatorioRepository.ListContratosAprovados(pesquisa, statusAndamento);
            return Ok(listContratos);
        }

        [HttpGet("PdfContrato/{id}")]
        public IActionResult PdfContrato(int id) {
            try {
                Contrato contrato = _relatorioRepository.GetContratoById(id);
                if (contrato == null) {
                    return NotFound("Desculpe, nenhum registro encontrado!");
                }
                var pxPorMm = 72 / 25.2f;
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
                var titulo = new Paragraph($"Relatório - contrato Nº {contrato.Id}\n\n", fonteParagrafo);
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

                var tabela = new PdfPTable(5);
                float[] larguraColunas = { 1.5f, 1f, 1f, 1f, 1f };
                tabela.SetWidths(larguraColunas);
                tabela.DefaultCell.BorderWidth = 0;
                tabela.WidthPercentage = 105;
                CriarCelulaTexto(tabela, "Cliente", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Situação", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Total pago", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Total de juros", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Total de juros pago", PdfPCell.ALIGN_LEFT, true);
                foreach (var item in contrato.Financeiros) {
                    string situacao;
                    if (!string.IsNullOrEmpty(item.PessoaFisicaId.ToString())) {
                        situacao = (item.PessoaFisica.Adimplente == Adimplencia.Adimplente) ? "Adimplente" : "Inadimplente";
                        CriarCelulaTexto(tabela, item.PessoaFisica.Name, PdfPCell.ALIGN_LEFT);
                        CriarCelulaTexto(tabela, situacao, PdfPCell.ALIGN_LEFT);
                    }
                    else {
                        situacao = (item.PessoaJuridica.Adimplente == Adimplencia.Adimplente) ? "Adimplente" : "Inadimplente";
                        CriarCelulaTexto(tabela, item.PessoaJuridica.NomeFantasia, PdfPCell.ALIGN_LEFT);
                        CriarCelulaTexto(tabela, situacao, PdfPCell.ALIGN_LEFT);
                    }
                    if (!string.IsNullOrEmpty(item.ValorTotalPago.ToString())) {
                        CriarCelulaTexto(tabela, item.ValorTotalPago.Value.ToString("C2"), PdfPCell.ALIGN_LEFT);
                    }
                    else {
                        CriarCelulaTexto(tabela, "R$ 0,00", PdfPCell.ALIGN_LEFT);
                    }
                    decimal? valorTotJuros = _relatorioRepository.ValorTotJurosCliente(item.Id);
                    if (!string.IsNullOrEmpty(valorTotJuros.ToString())) {
                        CriarCelulaTexto(tabela, valorTotJuros.Value.ToString("C2"), PdfPCell.ALIGN_LEFT);
                        if (!string.IsNullOrEmpty(item.ValorTotTaxaJurosPaga.ToString())) {
                            CriarCelulaTexto(tabela, item.ValorTotTaxaJurosPaga.Value.ToString("C2"), PdfPCell.ALIGN_LEFT);
                        }
                        else {
                            CriarCelulaTexto(tabela, "R$ 0,00", PdfPCell.ALIGN_LEFT);
                        }
                    }
                }
                string pularLinha = "\n\n";
                if (!string.IsNullOrEmpty(contrato.ValorTotalPagoContrato.ToString())) {
                    string paragrafoValoresContrato = $"(Valor total pago: {contrato.ValorTotalPagoContrato.Value.ToString("C2")}; " +
                    $"Valor total pendente: {ReturnValorPendente(contrato.ValorMonetario, contrato.ValorTotalPagoContrato)}; Valor total do contrato: {contrato.ValorMonetario.Value.ToString("C2")})\n";
                    string paragrafoValoresPorCliente = $"( Quantidade de clientes: {contrato.ClientesContrato.Count}; Valor total por cliente: {contrato.ReturnValorTotCliente()} )\n\n";
                    paragrofoJustificado.Add(pularLinha);
                    paragrofoJustificado.Add(paragrafoValoresContrato);
                    paragrofoJustificado.Alignment = Element.ALIGN_CENTER;
                    paragrofoJustificado.Add(paragrafoValoresPorCliente);
                }
                else {
                    string paragrafoValoresContrato = $"( Valor total pago: R$ 0,00; " +
                    $"Valor total pendente: {contrato.ValorMonetario.Value.ToString("C2")}; Valor total do contrato: {contrato.ValorMonetario.Value.ToString("C2")} )\n";
                    string paragrafoValoresPorCliente = $"( Quantidade de clientes: {contrato.ClientesContrato.Count}; Valor total por cliente: {contrato.ReturnValorTotCliente()} )\n\n";
                    paragrofoJustificado.Add(pularLinha);
                    paragrofoJustificado.Add(paragrafoValoresContrato);
                    paragrofoJustificado.Alignment = Element.ALIGN_CENTER;
                    paragrofoJustificado.Add(paragrafoValoresPorCliente);
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

                doc.Add(titulo);
                doc.Add(paragrofoJustificado);
                doc.Add(tabela);
                if (contrato.Rescisoes.Count > 0) {
                    Paragraph tituloJustificado = new Paragraph("",
                    new Font(fonteBase, 13, Font.NORMAL, BaseColor.DARK_GRAY));
                    tituloJustificado.Alignment = Element.ALIGN_CENTER;
                    string tituloRescisao = "\n\nTabela de rescisão do contrato\n\n";
                    //Tabela que contém os clientes que rescendiram o contrato. 
                    var tabelaRescisao = new PdfPTable(5);
                    float[] larguraColunas2 = { 1.5f, 1f, 1f, 1f, 1f };
                    tabelaRescisao.SetWidths(larguraColunas2);
                    tabelaRescisao.DefaultCell.BorderWidth = 0;
                    tabelaRescisao.WidthPercentage = 105;
                    CriarCelulaTexto(tabelaRescisao, "Cliente", PdfPCell.ALIGN_LEFT, true);
                    CriarCelulaTexto(tabelaRescisao, "Situação", PdfPCell.ALIGN_LEFT, true);
                    CriarCelulaTexto(tabelaRescisao, "Data de rescisão", PdfPCell.ALIGN_LEFT, true);
                    CriarCelulaTexto(tabelaRescisao, "Total pago", PdfPCell.ALIGN_LEFT, true);
                    CriarCelulaTexto(tabelaRescisao, "Valor efetuado pela multa", PdfPCell.ALIGN_LEFT, true);

                    foreach (var item in contrato.Rescisoes) {
                        string situacao;
                        if (!string.IsNullOrEmpty(item.PessoaFisicaId.ToString())) {
                            situacao = (item.PessoaFisica.Adimplente == Adimplencia.Adimplente) ? "Adimplente" : "Inadimplente";
                            CriarCelulaTexto(tabelaRescisao, item.PessoaFisica.Name, PdfPCell.ALIGN_LEFT);
                            CriarCelulaTexto(tabelaRescisao, situacao, PdfPCell.ALIGN_LEFT);
                        }
                        else {
                            situacao = (item.PessoaJuridica.Adimplente == Adimplencia.Adimplente) ? "Adimplente" : "Inadimplente";
                            CriarCelulaTexto(tabelaRescisao, item.PessoaJuridica.NomeFantasia, PdfPCell.ALIGN_LEFT);
                            CriarCelulaTexto(tabelaRescisao, situacao, PdfPCell.ALIGN_LEFT);
                        }
                        CriarCelulaTexto(tabelaRescisao, item.DataRescisao.Value.ToString("dd/MM/yyyy"), PdfPCell.ALIGN_LEFT);
                        if (!string.IsNullOrEmpty(item.ValorPagoContrato.ToString())) {
                            CriarCelulaTexto(tabelaRescisao, item.ValorPagoContrato.Value.ToString("C2"), PdfPCell.ALIGN_LEFT);
                        }
                        else {
                            CriarCelulaTexto(tabelaRescisao, "R$ 0,00", PdfPCell.ALIGN_LEFT);
                        }
                        CriarCelulaTexto(tabelaRescisao, item.Multa.Value.ToString("C2"), PdfPCell.ALIGN_LEFT);

                        //Adicionando a tabela no documento e posicionando os mesmos.
                    }
                    tituloJustificado.Add(tituloRescisao);
                    doc.Add(tituloJustificado);
                    doc.Add(tabelaRescisao);
                }
                doc.Add(paragrofoRodape);
                doc.Close();

                string nomeContrato = $"contrato {contrato.Id}";
                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", $"Relatório - {nomeContrato}.pdf");
            }
            catch (Exception error) {
                return BadRequest($"Desculpe, houve um erro ao processar sua requisição. Detalhes do erro: {error.Message}");
            }
        }
        
        static public string ReturnValorPendente(decimal? valueTotal, decimal? valuePago) {
            decimal result = valueTotal.Value - valuePago.Value;
            return $"{result.ToString("C2")}";
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
