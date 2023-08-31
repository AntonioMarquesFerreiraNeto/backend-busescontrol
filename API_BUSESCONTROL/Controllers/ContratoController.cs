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
    [Authorize(Roles = "Assistente, Administrador")]
    public class ContratoController : ControllerBase {

        private readonly IContratoRepository _contratoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IFinanceiroRepository _financeiroRepository;
        public string textoContratante;
        public string nomeCliente;

        public ContratoController(IContratoRepository contratoRepository, IClienteRepository clienteRepository, IFinanceiroRepository financeiroRepository) {
            _contratoRepository = contratoRepository;
            _clienteRepository = clienteRepository;
            _financeiroRepository = financeiroRepository;
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

        [HttpGet("GetContratosAtivos/{paginaAtual}/{filtro}/{pesquisa?}")]
        public IActionResult GetContratosAtivos(int paginaAtual = 1, FiltroContrato filtro = FiltroContrato.Todos, string? pesquisa = "") {
            _financeiroRepository.TaskMonitorPdfRescisao();
            List<Contrato> contratos = _contratoRepository.GetContratosAtivos(paginaAtual, filtro, pesquisa);
            var response = new {
                contractList = contratos,
                qtPaginas = _contratoRepository.ReturnQtPaginasAtivos(filtro, pesquisa)
            };
            return Ok(response);
        }

        [HttpGet("GetContratosInativos/{paginaAtual}/{pesquisa?}")]
        public IActionResult GetContratosInativos(int paginaAtual, string? pesquisa = "") {
            List<Contrato> contratos = _contratoRepository.GetContratosInativos(paginaAtual, pesquisa);
            var response = new {
                contractList = contratos,
                qtPaginas = _contratoRepository.ReturnQtPaginasInativos(pesquisa)
            };
            return Ok(response);
        }

        [HttpPatch("Aprovar/{id}")]
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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

        [HttpDelete("ConfirmRescisao/{contratoId}/{clienteId}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult RescendirContrato(int contratoId, int clienteId) {
            try {
                _financeiroRepository.RescisaoContrato(contratoId, clienteId);
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
                        titulo.Style.Font.FontColor = XLColor.DarkBlue;
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

        [HttpGet("RelatorioPDF/{ativo}")]
        public IActionResult RelatorioPdf(bool ativo) {
            try {
                List<Contrato> contratos = (ativo) ? _contratoRepository.GetAllContratosAtivos() : _contratoRepository.GetAllContratosInativos();
                if (!contratos.Any() || contratos == null) {
                    return NotFound("Nenhum registro encontrado!");
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
                string txtTitulo = (ativo) ? "ativos" : "inativos";
                var titulo = new Paragraph($"Contratos {txtTitulo}\n\n\n", fonteParagrafo);
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
                float[] larguraColunas = { 0.4f, 1.7f, 1f, 1f, 1f, 1f, 1f };
                tabela.SetWidths(larguraColunas);
                tabela.DefaultCell.BorderWidth = 0;
                tabela.WidthPercentage = 105;
                CriarCelulaTexto(tabela, "ID", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Datas", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Valor total", PdfPCell.ALIGN_LEFT, true);
                CriarCelulaTexto(tabela, "Qt parcelas", PdfPCell.ALIGN_CENTER, true);
                CriarCelulaTexto(tabela, "Pagamento", PdfPCell.ALIGN_CENTER, true);
                CriarCelulaTexto(tabela, "Aprovação", PdfPCell.ALIGN_CENTER, true);
                CriarCelulaTexto(tabela, "Andamento", PdfPCell.ALIGN_CENTER, true);
                foreach (var item in contratos.OrderBy(x => x.Andamento)) {
                    string valorTot = item.ValorMonetario!.Value.ToString("C2");
                    CriarCelulaTexto(tabela, item.Id.ToString(), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, ReturnPeriodoContrato(item), PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, valorTot, PdfPCell.ALIGN_LEFT);
                    CriarCelulaTexto(tabela, item.QtParcelas.ToString(), PdfPCell.ALIGN_CENTER);
                    CriarCelulaTexto(tabela, (item.Pagament == ModelPagament.Parcelado) ? "Parcelado" : "À vista", PdfPCell.ALIGN_CENTER);
                    CriarCelulaTexto(tabela, ReturnAprovacao(item.Aprovacao), PdfPCell.ALIGN_CENTER);
                    CriarCelulaTexto(tabela, ReturnAndamento(item.Andamento), PdfPCell.ALIGN_CENTER);
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
                string rodape2 = $"\nDocumento gerado em: {DateTime.Now.ToString("dd/MM/yyyy")}";
                paragrofoRodape.Add(rodape2);
                doc.Add(titulo);
                doc.Add(paragrofoJustificado);
                doc.Add(tabela);
                doc.Add(paragrofoRodape);
                doc.Close();

                string nomeContrato = (ativo) ? "Relatório - contratos ativos" : "Relatório - contratos inativos";
                stream.Flush();
                stream.Position = 0;
                return File(stream, "application/pdf", $"{nomeContrato}.pdf");
            }
            catch (Exception erro) {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("PdfContratoCliente/{id}/{clienteFisicoId}/{clienteJuridicoId}")]
        public IActionResult ReturnPdfContratoCliente(int id, int clienteFisicoId, int clienteJuridicoId) {
            try {
                Contrato contrato = _contratoRepository.GetContratoById(id);
                if (contrato == null) {
                    return NotFound("Nenhum registro encontrado!");
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
                new Font(fonteBase, 12, Font.NORMAL));
                paragrofoJustificado.Alignment = Element.ALIGN_JUSTIFIED;
                Paragraph paragrafoCenter = new Paragraph("", new Font(fonteBase, 12, Font.NORMAL));
                paragrafoCenter.Alignment = Element.ALIGN_CENTER;
                var titulo = new Paragraph($"Contrato de serviço Nº {contrato.Id}\n\n", fonteParagrafo);
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

                string titulo_contratante = "\nCONTRATANTE:";

                if (clienteFisicoId != 0) {
                    var data = contrato.ClientesContrato.FirstOrDefault(x => x.PessoaFisicaId == clienteFisicoId);
                    PessoaFisica pessoaFisica = data!.PessoaFisica;
                    if (pessoaFisica == null) {
                        return NotFound("Nenhum registro encontrado!");
                    }
                    if (!string.IsNullOrEmpty(pessoaFisica.IdVinculacaoContratual.ToString())) {
                        PessoaFisica pessoaFisicaResponsavel = _clienteRepository.GetClienteFisicoById(pessoaFisica.IdVinculacaoContratual!.Value);
                        if (pessoaFisicaResponsavel != null) {
                            nomeCliente = pessoaFisica.Name!;
                            textoContratante = $"{titulo_contratante}\n{pessoaFisicaResponsavel.Name} portador(a) do " +
                                $"CPF: {pessoaFisicaResponsavel.ReturnCpfCliente()}, RG: {pessoaFisicaResponsavel.Rg}, filho(a) da Sr. {pessoaFisicaResponsavel.NameMae}, residente domiciliado no imovel Nº {pessoaFisicaResponsavel.NumeroResidencial}({pessoaFisicaResponsavel.Logradouro}), próximo ao complemento residencial {pessoaFisicaResponsavel.ComplementoResidencial}, no bairro {pessoaFisicaResponsavel.Bairro}," +
                                $" da cidade de {pessoaFisicaResponsavel.Cidade} — {pessoaFisicaResponsavel.Estado}. Tendo como forma de contato os seguintes canais: ({pessoaFisicaResponsavel.Ddd}){pessoaFisicaResponsavel.ReturnTelefoneCliente()}, {pessoaFisicaResponsavel.Email}. Neste ato representado(a) como responsável legal pelo(a) requerente do contrato que será descrito a seguir: " +
                                $"\n{pessoaFisica.Name} portador(a) do " +
                                $"CPF: {pessoaFisica.ReturnCpfCliente()}, RG: {pessoaFisica.Rg}, filho(a) da Sr. {pessoaFisica.NameMae}, residente domiciliado no imovel Nº {pessoaFisica.NumeroResidencial}({pessoaFisica.Logradouro}), próximo ao complemento residencial {pessoaFisica.ComplementoResidencial}, no bairro {pessoaFisica.Bairro}," +
                                $" da cidade de {pessoaFisica.Cidade} — {pessoaFisica.Estado}. Tendo como forma de contato os seguintes canais: ({pessoaFisica.Ddd}){pessoaFisica.ReturnTelefoneCliente()}, {pessoaFisica.Email}.\n\n\n";
                        }
                        else {
                            nomeCliente = pessoaFisica.Name!;
                            PessoaJuridica pessoaJuridicaResponsavel = _clienteRepository.GetClienteByIdPJ(pessoaFisica.IdVinculacaoContratual.Value);
                            textoContratante = $"{titulo_contratante}\n{pessoaJuridicaResponsavel.RazaoSocial}, inscrita no CNPJ: {pessoaJuridicaResponsavel.ReturnCnpjCliente()}, inscrição estadual: {pessoaJuridicaResponsavel.InscricaoEstadual}, inscrição municipal: {pessoaJuridicaResponsavel.InscricaoMunicipal}, portadora do nome fantasia {pessoaJuridicaResponsavel.NomeFantasia}, " +
                            $"residente domiciliado no imovel Nº {pessoaJuridicaResponsavel.NumeroResidencial} ({pessoaJuridicaResponsavel.Logradouro}), próximo ao complemento residencial {pessoaJuridicaResponsavel.ComplementoResidencial}, no bairro {pessoaJuridicaResponsavel.Bairro}," +
                            $" da cidade de {pessoaJuridicaResponsavel.Cidade} — {pessoaJuridicaResponsavel.Estado}. Tendo como forma de contato os seguintes canais: ({pessoaJuridicaResponsavel.Ddd}){pessoaJuridicaResponsavel.ReturnTelefoneCliente()}, {pessoaJuridicaResponsavel.Email}. Neste ato representada como responsável legal pelo(a) requerente do contrato que será descrito a seguir:" +
                            $"\n{pessoaFisica.Name} portador(a) do " +
                                $"CPF: {pessoaFisica.ReturnCpfCliente()}, RG: {pessoaFisica.Rg}, filho(a) da Sr. {pessoaFisica.NameMae}, residente domiciliado no imovel Nº {pessoaFisica.NumeroResidencial}({pessoaFisica.Logradouro}), próximo ao complemento residencial {pessoaFisica.ComplementoResidencial}, no bairro {pessoaFisica.Bairro}," +
                                $" da cidade de {pessoaFisica.Cidade} — {pessoaFisica.Estado}. Tendo como forma de contato os seguintes canais: ({pessoaFisica.Ddd}){pessoaFisica.ReturnTelefoneCliente()}, {pessoaFisica.Email}.\n\n\n";
                        }

                    }
                    else {
                        nomeCliente = pessoaFisica.Name!;
                        textoContratante = $"{titulo_contratante}\n{pessoaFisica.Name} portador(a) do " +
                        $"CPF: {pessoaFisica.ReturnCpfCliente()}, RG: {pessoaFisica.Rg}, filho(a) da Sr. {pessoaFisica.NameMae}, residente domiciliado no imovel Nº {pessoaFisica.NumeroResidencial}({pessoaFisica.Logradouro}), próximo ao complemento residencial {pessoaFisica.ComplementoResidencial}, no bairro {pessoaFisica.Bairro}," +
                        $" da cidade de {pessoaFisica.Cidade} — {pessoaFisica.Estado}. Tendo como forma de contato os seguintes canais: ({pessoaFisica.Ddd}){pessoaFisica.ReturnTelefoneCliente()}, {pessoaFisica.Email}. Neste ato representado(a) como o requerente do contrato.\n\n\n";
                    }
                }
                else {
                    var data = contrato.ClientesContrato.FirstOrDefault(x => x.PessoaJuridicaId == clienteJuridicoId);
                    PessoaJuridica pessoaJuridica = data!.PessoaJuridica;
                    if (pessoaJuridica == null) {
                        return NotFound("Nenhum registro encontrado!");
                    }
                    nomeCliente = pessoaJuridica.NomeFantasia!;
                    textoContratante = $"{titulo_contratante}\n{pessoaJuridica.RazaoSocial}, inscrita no CNPJ: {pessoaJuridica.ReturnCnpjCliente()}, inscrição estadual: {pessoaJuridica.InscricaoEstadual}, inscrição municipal: {pessoaJuridica.InscricaoMunicipal}, portadora do nome fantasia {pessoaJuridica.NomeFantasia}, " +
                    $"residente domiciliado no imovel Nº {pessoaJuridica.NumeroResidencial} ({pessoaJuridica.Logradouro}), próximo ao complemento residencial {pessoaJuridica.ComplementoResidencial}, no bairro {pessoaJuridica.Bairro}," +
                    $" da cidade de {pessoaJuridica.Cidade} — {pessoaJuridica.Estado}. Tendo como forma de contato os seguintes canais: ({pessoaJuridica.Ddd}){pessoaJuridica.ReturnTelefoneCliente()}, {pessoaJuridica.Email}. Neste ato representada como a requerente do contrato.\n\n\n";
                }

                string titulo_contratada = $"CONTRATADA:";
                string textoContratada = $"{titulo_contratada}\nBuss viagens LTDA, pessoa jurídica de direito privado para prestação de serviço, na proteção da LEI Nº 13.429º. " +
                    $"Localizada na cidade de Goianésia (GO) — Brasil, inscrita no CNPJ nº 02.116.484/0001-02, sobre a liderança do sócio fundador Manoel Hamilton Rodrigues." +
                    $" Neste ato representada como  a empresa responsável pela realização da prestações de serviços do contrato.\n\n\n";

                string titulo_primeira_clausula = $"1 — CLÁUSULA PRIMEIRA";
                string PrimeiraClausula = $"{titulo_primeira_clausula}\nO presente contrato tem por objeto a prestação de serviço especial de transporte rodoviário na rota definida no registro do contrato: {contrato.Detalhamento}\n\n\n";

                string titulo_segunda_clausula = $"2 — CLÁUSULA SEGUNDA";
                string SegundaClausula = $"{titulo_segunda_clausula} \nO(s) veículo(s) que realizará(ão) o transporte será(ão) discriminado(s) a seguir: \n" +
                    $"  • Veículo {contrato.Onibus!.Marca}, modelo {contrato.Onibus.NameBus}, placa {contrato.Onibus.Placa}, número de chassi {contrato.Onibus.Chassi}, veículo {contrato.Onibus.CorBus!.ToLower()}, fabricado em {contrato.Onibus.DataFabricacao}, e com capacidade de lotação para {contrato.Onibus.Assentos} passageiros.\n No caso de problemas com o(s) veículo(s) acima designado(s), " +
                    $"poderá ser utilizado outro veículo, desde que conste habilitado no Sistema de Habilitação de Transportes de Passageiros – SisHAB, da ANTT. \n\n";

                string titulo_terceira_clausula = $"\n3 — CLÁUSULA TERCEIRA";
                string TerceiraClausula = $"{titulo_terceira_clausula} \nO contratante deve estar ciente que deverá cumprir com as datas de pagamento determinadas do contrato. Desta forma, estando ciente de valores de juros adicionais em caso de inadimplência. Nos quais são 2% ao mês por parcela atrasada.\n\n\n";

                string titulo_quarta_clausula = $"4 — CLÁUSULA QUARTA";
                string QuartaClausula;
                if (contrato.Pagament == Models.Enums.ModelPagament.Avista) {
                    QuartaClausula = $"{titulo_quarta_clausula} \nPelos serviços prestados a Contratante pagará a Contratada o valor de {contrato.ReturnValorTotCliente()}, na data atual com três dias úteis. Em parcela única, pois, o contrato foi deferido como à vista.\n\n";
                }
                else {
                    QuartaClausula = $"{titulo_quarta_clausula} \nPelos serviços prestados a Contratante pagará a Contratada o valor de {contrato.ReturnValorTotCliente()}, e os respectivos pagamentos serão realizados dia {contrato.ReturnDiaPagamento()} de cada mês. Dividos em {contrato.QtParcelas} parcelas no valor {contrato.ValorParcelaContratoPorCliente!.Value.ToString("C2")}. No entanto, a primeira parcela do contrato terá três dias úteis para realização do pagamento após a aprovação do contrato.\n\n";
                }
                string titulo_quinta_clausula = $"\n5 — CLÁUSULA QUINTA";
                string QuintaClausula = $"{titulo_quinta_clausula}\nEm caso de rescisão de contrato anterior a data acordada sem o devido pagamento da(s) parcela(s), o cliente deve estar ciente que haverá multa de 3% do valor total por cliente ( {contrato.ReturnValorTotCliente()} ), pela rescisão do contrato.\n\n\n";

                string titulo_sexta_clausula = $"6 — CLÁUSULA SEXTA";
                string SextaClausula = $"{titulo_sexta_clausula} \nO período da prestação do serviço será de  {ReturnPeriodoContrato(contrato)}, que é a data acordada no registro do contrato.\n\n\n";

                string titulo_setima_clausula = $"7 — CLÁUSULA SÉTIMA";
                string SetimaClausula = $"{titulo_setima_clausula}\nO contratante fica ciente que somente será permitido o transporte de passageiros limitados à capacidade de passageiros sentados no(s) veículo(s) utilizado(s), ficando expressamente proibido o transporte de passageiros em pé ou acomodados no corredor, bem como passageiros que não estiverem constando na relação autorizada pela ANTT.\n\n\n";

                string traco = "\n___________________________________________\n";
                string assinaturaCliente = "Assinatura do representante legal contratante\n\n";
                string traco2 = "___________________________________________________________\n";
                string assinaturaEmpresa = "Assinatura da empresa representante da prestação do serviço";
                string traco3 = "________________________________________________\n";
                string assinaturaAdm = "Assinatura do administrador que aprovou o contrato\n\n";

                paragrofoJustificado.Add(textoContratante);
                paragrofoJustificado.Add(textoContratada);
                paragrofoJustificado.Add(PrimeiraClausula);
                paragrofoJustificado.Add(SegundaClausula);
                paragrofoJustificado.Add(TerceiraClausula);
                paragrofoJustificado.Add(QuartaClausula);
                paragrofoJustificado.Add(QuintaClausula);
                paragrofoJustificado.Add(SextaClausula);
                paragrofoJustificado.Add(SetimaClausula);

                paragrafoCenter.Add(traco);
                paragrafoCenter.Add(assinaturaCliente);
                paragrafoCenter.Add(traco3);
                paragrafoCenter.Add(assinaturaAdm);
                paragrafoCenter.Add(traco2);
                paragrafoCenter.Add(assinaturaEmpresa);

                doc.Add(titulo);
                doc.Add(paragrofoJustificado);
                doc.Add(paragrafoCenter);

                doc.Close();

                stream.Flush();
                stream.Position = 0;
                return File(stream, "application/pdf", $"Contrato - {nomeCliente}.pdf");
            }
            catch (Exception erro) {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("PdfRescisao/{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult PdfRescisao(int id) {
            try {
                ClientesContrato clientesContrato = _contratoRepository.GetClientesContratoById(id);
                if (clientesContrato == null) {
                    return NotFound("Desculpe, nenhum registro encontrado");
                }
                var pxPorMm = 72 / 25.2f;
                Document doc = new Document(PageSize.A4, 15 * pxPorMm, 15 * pxPorMm,
                    15 * pxPorMm, 15 * pxPorMm);
                MemoryStream stream = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                writer.CloseStream = false;
                doc.Open();
                var fonteBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                var fonteParagrafo = new iTextSharp.text.Font(fonteBase, 14,
                    iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
                Paragraph paragrofoJustificado = new Paragraph("",
                new Font(fonteBase, 12, Font.NORMAL));
                paragrofoJustificado.Alignment = Element.ALIGN_JUSTIFIED;
                Paragraph paragrafoCenter = new Paragraph("", new Font(fonteBase, 12, Font.NORMAL));
                paragrafoCenter.Alignment = Element.ALIGN_CENTER;

                var caminhoImgLeft = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "C:\\Antonio\\Faculdade\\Sétimo período\\BusesControl--TCC-master\\BusesControl\\wwwroot\\css\\Imagens\\LogoPdf.jpeg");
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

                string titulo_contratante = "1 - CONTRATANTE";

                if (!string.IsNullOrEmpty(clientesContrato.PessoaFisicaId.ToString())) {
                    if (!string.IsNullOrEmpty(clientesContrato!.PessoaFisica!.IdVinculacaoContratual.ToString())) {
                        PessoaFisica pessoaFisicaResponsavel = _clienteRepository.GetClienteFisicoById(clientesContrato.PessoaFisica.IdVinculacaoContratual!.Value);
                        if (pessoaFisicaResponsavel != null) {
                            nomeCliente = clientesContrato.PessoaFisica.Name!;
                            textoContratante = $"{titulo_contratante}\n{pessoaFisicaResponsavel.Name} portador(a) do " +
                                $"CPF: {pessoaFisicaResponsavel.ReturnCpfCliente()}, RG: {pessoaFisicaResponsavel.Rg}, filho(a) da Sr. {pessoaFisicaResponsavel.NameMae}, residente domiciliado no imovel Nº {pessoaFisicaResponsavel.NumeroResidencial}({pessoaFisicaResponsavel.Logradouro}), próximo ao complemento residencial {pessoaFisicaResponsavel.ComplementoResidencial}, no bairro {pessoaFisicaResponsavel.Bairro}," +
                                $" da cidade de {pessoaFisicaResponsavel.Cidade} — {pessoaFisicaResponsavel.Estado}. Tendo como forma de contato os seguintes canais: ({pessoaFisicaResponsavel.Ddd}){pessoaFisicaResponsavel.ReturnTelefoneCliente()}, {pessoaFisicaResponsavel.Email}. Neste ato, sendo o representante legal do processo de rescisão de contrato do cliente vinculado que será descrito a seguir: " +
                                $"\n{clientesContrato.PessoaFisica.Name} portador(a) do " +
                                $"CPF: {clientesContrato.PessoaFisica.ReturnCpfCliente()}, RG: {clientesContrato.PessoaFisica.Rg}, filho(a) da Sr. {clientesContrato.PessoaFisica.NameMae}, residente domiciliado no imovel Nº {clientesContrato.PessoaFisica.NumeroResidencial}({clientesContrato.PessoaFisica.Logradouro}), próximo ao complemento residencial {clientesContrato.PessoaFisica.ComplementoResidencial}, no bairro {clientesContrato.PessoaFisica.Bairro}," +
                                $" da cidade de {clientesContrato.PessoaFisica.Cidade} — {clientesContrato.PessoaFisica.Estado}. Tendo como forma de contato os seguintes canais: ({clientesContrato.PessoaFisica.Ddd}){clientesContrato.PessoaFisica.ReturnTelefoneCliente()}, {clientesContrato.PessoaFisica.Email}.\n\n\n";
                        }
                        else {
                            nomeCliente = clientesContrato.PessoaFisica.Name!;
                            PessoaJuridica pessoaJuridicaResponsavel = _clienteRepository.GetClienteByIdPJ(clientesContrato.PessoaFisica.IdVinculacaoContratual.Value);
                            textoContratante = $"{titulo_contratante}\n{pessoaJuridicaResponsavel.RazaoSocial}, inscrita no CNPJ: {pessoaJuridicaResponsavel.ReturnCnpjCliente()}, inscrição estadual: {pessoaJuridicaResponsavel.InscricaoEstadual}, inscrição municipal: {pessoaJuridicaResponsavel.InscricaoMunicipal}, portadora do nome fantasia {pessoaJuridicaResponsavel.NomeFantasia}, " +
                            $"residente domiciliado no imovel Nº {pessoaJuridicaResponsavel.NumeroResidencial} ({pessoaJuridicaResponsavel.Logradouro}), próximo ao complemento residencial {pessoaJuridicaResponsavel.ComplementoResidencial}, no bairro {pessoaJuridicaResponsavel.Bairro}," +
                            $" da cidade de {pessoaJuridicaResponsavel.Cidade} — {pessoaJuridicaResponsavel.Estado}. Tendo como forma de contato os seguintes canais: ({pessoaJuridicaResponsavel.Ddd}){pessoaJuridicaResponsavel.ReturnTelefoneCliente()}, {pessoaJuridicaResponsavel.Email}. Neste ato, sendo o representante legal do processo de rescisão de contrato do cliente vinculado que será descrito a seguir:" +
                            $"\n{clientesContrato.PessoaFisica.Name} portador(a) do " +
                                $"CPF: {clientesContrato.PessoaFisica.ReturnCpfCliente()}, RG: {clientesContrato.PessoaFisica.Rg}, filho(a) da Sr. {clientesContrato.PessoaFisica.NameMae}, residente domiciliado no imovel Nº {clientesContrato.PessoaFisica.NumeroResidencial}({clientesContrato.PessoaFisica.Logradouro}), próximo ao complemento residencial {clientesContrato.PessoaFisica.ComplementoResidencial}, no bairro {clientesContrato.PessoaFisica.Bairro}," +
                                $" da cidade de {clientesContrato.PessoaFisica.Cidade} — {clientesContrato.PessoaFisica.Estado}. Tendo como forma de contato os seguintes canais: ({clientesContrato.PessoaFisica.Ddd}){clientesContrato.PessoaFisica.ReturnTelefoneCliente()}, {clientesContrato.PessoaFisica.Email}.\n\n\n";
                        }

                    }
                    else {
                        nomeCliente = clientesContrato.PessoaFisica.Name;
                        textoContratante = $"{titulo_contratante}\n{clientesContrato.PessoaFisica.Name} portador(a) do " +
                        $"CPF: {clientesContrato.PessoaFisica.ReturnCpfCliente()}, RG: {clientesContrato.PessoaFisica.Rg}, filho(a) da Sr. {clientesContrato.PessoaFisica.NameMae}, residente domiciliado no imovel Nº {clientesContrato.PessoaFisica.NumeroResidencial}({clientesContrato.PessoaFisica.Logradouro}), próximo ao complemento residencial {clientesContrato.PessoaFisica.ComplementoResidencial}, no bairro {clientesContrato.PessoaFisica.Bairro}," +
                        $" da cidade de {clientesContrato.PessoaFisica.Cidade} — {clientesContrato.PessoaFisica.Estado}. Tendo como forma de contato os seguintes canais: ({clientesContrato.PessoaFisica.Ddd}){clientesContrato.PessoaFisica.ReturnTelefoneCliente()}, {clientesContrato.PessoaFisica.Email}. Neste ato, sendo o responsável e solicitador do processo de rescisão do contrato, garantindo seus direitos legais definidos pela lei, e garantidos pela cláusula cinco do contrato.\n\n\n";
                    }
                }
                else {
                    PessoaJuridica pessoaJuridica = _clienteRepository.GetClienteByIdPJ(clientesContrato!.PessoaJuridicaId.Value);
                    if (pessoaJuridica == null) {
                        return NotFound("Desculpe, nenhum registro encontrado!");
                    }
                    nomeCliente = pessoaJuridica.NomeFantasia!;
                    textoContratante = $"{titulo_contratante}\n{pessoaJuridica.RazaoSocial}, inscrita no CNPJ: {pessoaJuridica.ReturnCnpjCliente()}, inscrição estadual: {pessoaJuridica.InscricaoEstadual}, inscrição municipal: {pessoaJuridica.InscricaoMunicipal}, portadora do nome fantasia {pessoaJuridica.NomeFantasia}, " +
                    $"residente domiciliado no imovel Nº {pessoaJuridica.NumeroResidencial} ({pessoaJuridica.Logradouro}), próximo ao complemento residencial {pessoaJuridica.ComplementoResidencial}, no bairro {pessoaJuridica.Bairro}," +
                    $" da cidade de {pessoaJuridica.Cidade} — {pessoaJuridica.Estado}. Tendo como forma de contato os seguintes canais: ({pessoaJuridica.Ddd}){pessoaJuridica.ReturnTelefoneCliente()}, {pessoaJuridica.Email}. Neste ato, sendo o responsável e solicitador do processo de rescisão do contrato, garantindo seus direitos legais definidos pela lei, e garantidos pela cláusula cinco do contrato.\n\n\n";
                }

                string titulo_contratada = $"2 - CONTRATADA";
                string textoContratada = $"{titulo_contratada}\nBuss viagens LTDA, pessoa jurídica de direito privado para prestação de serviço, na proteção da LEI Nº 13.429º. " +
                    $"Localizada na cidade de Goianésia (GO) — Brasil, inscrita no CNPJ nº 03.115.484/0001-02, sobre a liderança do sócio fundador Manoel Rodrigues." +
                    $" Neste ato representada como  a empresa responsável pela realização da prestações de serviços do contrato.\n\n\n";


                decimal? valorTotCliente = clientesContrato.Contrato.ValorParcelaContratoPorCliente * clientesContrato.Contrato.QtParcelas;
                decimal valorMulta = (valorTotCliente.Value * 3) / 100;

                string titulo_quinta_clausula = $"3 - PROCESSO DE RESCISÃO";
                string QuintaClausula = $"{titulo_quinta_clausula}\n“Em caso de rescisão de contrato anterior a data acordada sem o devido pagamento da(s) parcela(s), o cliente deve estar ciente que haverá multa de 3% do valor total por cliente ( {clientesContrato.Contrato.ReturnValorTotCliente()} ), pela rescisão do contrato.”. " +
                    $"\nCom base e asseguração da quinta cláusula do contrato, é dever do cliente realizar o pagamento de {valorMulta.ToString("C2")} para rescindir o contrato.\n\n\n";

                string traco = "\n___________________________________________\n";
                string assinaturaCliente = "Assinatura do representante legal contratante\n\n";
                string traco2 = "___________________________________________________________\n";
                string assinaturaEmpresa = "Assinatura da empresa representante da prestação do serviço";

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

                paragrofoJustificado.Add(textoContratante);
                paragrofoJustificado.Add(textoContratada);
                paragrofoJustificado.Add(QuintaClausula);

                paragrafoCenter.Add(traco);
                paragrafoCenter.Add(assinaturaCliente);
                paragrafoCenter.Add(traco2);
                paragrafoCenter.Add(assinaturaEmpresa);

                var titulo = new Paragraph($"Rescisão contrato Nº {clientesContrato.ContratoId} - {nomeCliente} \n\n\n", fonteParagrafo);
                titulo.Alignment = Element.ALIGN_LEFT;
                doc.Add(titulo);
                doc.Add(paragrofoJustificado);
                doc.Add(paragrafoCenter);

                doc.Close();

                stream.Flush();
                stream.Position = 0;

                _financeiroRepository.ConfirmarImpressaoPdfRescisao(clientesContrato);

                return File(stream, "application/pdf", $"Rescisão - {nomeCliente}.pdf");
            }
            catch (Exception error) {
                return BadRequest($"Desculpe, algum erro inesperado aconteceu! Detalhes do erro: {error.Message}");
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
                case Andamento.Aguardando: return "Em tramitação";
                case Andamento.EmAndamento: return "Em andamento";
                case Andamento.Encerrado: return "Encerrado";
                default: return "status não encontrado";
            }
        }
        string ReturnPeriodoContrato(Contrato contrato) {
            return $"{contrato.DataEmissao!.Value.ToString("dd/MM/yyyy")} até {contrato.DataVencimento!.Value.ToString("dd/MM/yyyy")}";
        }
    }
}
