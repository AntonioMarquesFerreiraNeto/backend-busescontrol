using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_BUSESCONTROL.Repository {
    public class FinanceiroRepository : IFinanceiroRepository {

        private readonly BancoContext _bancoContext;

        public FinanceiroRepository(BancoContext bancoContext) {
            _bancoContext = bancoContext;
        }

        public List<Financeiro> ListFinanceiros() {
            return _bancoContext.Financeiro
                .AsNoTracking().Include(x => x.PessoaFisica)
                .AsNoTracking().Include(x => x.PessoaJuridica)
                .AsNoTracking().Include(x => x.Fornecedor)
                .AsNoTracking().Include(x => x.Contrato)
                .AsNoTracking().Include(x => x.Parcelas)
                .ToList();
        }

        public Financeiro ReturnPorId(int id) {
            return _bancoContext.Financeiro.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Nenhum registro encontrado!"); ;
        }

        public Contrato ListarJoinPorId(int id) {
            return _bancoContext.Contrato
                .AsNoTracking().Include("Motorista")
                .AsNoTracking().Include("Onibus")
                .AsNoTracking().Include(x => x!.Financeiros).ThenInclude(x => x!.Parcelas)
                .AsNoTracking().Include(x => x.Financeiros).ThenInclude(x => x.PessoaFisica)
                .AsNoTracking().Include(x => x.Financeiros).ThenInclude(x => x.PessoaJuridica)
                .FirstOrDefault(x => x.Id == id) ?? throw new Exception("Nenhum registro encontrado!");
        }
        public Financeiro listPorIdFinanceiro(int? id) {
            return _bancoContext.Financeiro
                .AsNoTracking().Include(x => x.PessoaFisica)
                .AsNoTracking().Include(x => x.PessoaJuridica)
                .AsNoTracking().Include(x => x.Contrato)
                .AsNoTracking().Include(x => x.Fornecedor)
            .AsNoTracking().Include(x => x.Parcelas)
                .FirstOrDefault(x => x.Id == id) ?? throw new Exception("Nenhum registro encontrado!");
        }
        public Parcela ListarFinanceiroPorId(int id) {
            return _bancoContext.Parcela.Include(x => x.Financeiro).ThenInclude(x => x.Contrato).FirstOrDefault(x => x.Id == id) ?? throw new Exception("Nenhum registro encontrado!"); ;
        }

        public Parcela ContabilizarParcela(int id) {
            try {
                Parcela parcelaDB = ListarFinanceiroPorId(id);
                if (parcelaDB.Financeiro!.FinanceiroStatus == FinanceiroStatus.Inativo) throw new Exception("Desculpe, ID não foi encontrado!");
                if (parcelaDB.StatusPagamento == SituacaoPagamento.PagamentoContabilizado) throw new Exception("Pagamento já realizado!");
                parcelaDB.StatusPagamento = SituacaoPagamento.PagamentoContabilizado;
                parcelaDB.DataEfetuacao = DateTime.Now;
                _bancoContext.Parcela.Update(parcelaDB);
                if (!string.IsNullOrEmpty(parcelaDB.Financeiro.ContratoId.ToString())) {
                    ValidarInadimplenciaCliente(parcelaDB);
                }
                SetValoresPagosContratoAndCliente(parcelaDB);
                _bancoContext.SaveChanges();
                return parcelaDB;
            }
            catch (Exception erro) {
                throw new Exception(erro.Message);
            }
        }
        public void SetValoresPagosContratoAndCliente(Parcela parcelaDB) {
            Financeiro financeiro = parcelaDB.Financeiro;
            if (!string.IsNullOrEmpty(financeiro!.ContratoId.ToString())) {
                if (!string.IsNullOrEmpty(financeiro.ValorTotalPago.ToString())) {
                    financeiro.ValorTotalPago += financeiro.Contrato!.ValorParcelaContratoPorCliente;
                }
                else {
                    financeiro.ValorTotalPago = financeiro.Contrato!.ValorParcelaContratoPorCliente;
                }
                if (!string.IsNullOrEmpty(financeiro.Contrato.ValorTotalPagoContrato.ToString())) {
                    financeiro.Contrato.ValorTotalPagoContrato += financeiro.Contrato.ValorParcelaContratoPorCliente;
                }
                else {
                    financeiro.Contrato.ValorTotalPagoContrato = financeiro.Contrato.ValorParcelaContratoPorCliente;
                }
                if (!string.IsNullOrEmpty(parcelaDB.ValorJuros.ToString())) {
                    if (!string.IsNullOrEmpty(financeiro.ValorTotTaxaJurosPaga.ToString())) {
                        financeiro.ValorTotTaxaJurosPaga += parcelaDB.ValorJuros;
                    }
                    else {
                        financeiro.ValorTotTaxaJurosPaga = parcelaDB.ValorJuros;
                    }
                }
                _bancoContext.Contrato.Update(financeiro.Contrato);
            }
            else {
                if (!string.IsNullOrEmpty(financeiro.ValorTotalPago.ToString())) {
                    financeiro.ValorTotalPago += parcelaDB.Financeiro!.ValorParcelaDR;
                }
                else {
                    financeiro.ValorTotalPago = parcelaDB.Financeiro!.ValorParcelaDR;
                }
            }
            _bancoContext.Financeiro.Update(financeiro);
        }
        public void ValidarInadimplenciaCliente(Parcela value) {
            var pessoaJuridica = _bancoContext.PessoaJuridica.Include(x => x.Financeiros).ThenInclude(x => x.Parcelas).FirstOrDefault(pessoa =>
               pessoa.Financeiros.Any(financeiro => financeiro.Parcelas.Any(parcelas =>
               parcelas.Id == value.Id)) && !string.IsNullOrEmpty(pessoa.Cnpj));

            var pessoaFisica = _bancoContext.PessoaFisica.Include(x => x.Financeiros).ThenInclude(x => x.Parcelas).FirstOrDefault(pessoa =>
                pessoa.Financeiros.Any(financeiro => financeiro.Parcelas.Any(parcelas =>
                parcelas.Id == value.Id) && !string.IsNullOrEmpty(pessoa.Cpf)));

            if (pessoaFisica != null) {
                int result = ReturnQtParcelasAtrasadaCliente(pessoaFisica.Financeiros);
                if (result == 0) {
                    if (!string.IsNullOrEmpty(pessoaFisica.IdVinculacaoContratual.ToString())) {
                        pessoaFisica.Adimplente = Adimplencia.Adimplente;
                        _bancoContext.PessoaFisica.Update(pessoaFisica);
                        //Chama o método que valida e seta com adimplente se passar na validação.
                        ValidarAndSetAdimplenteClienteResponsavel(pessoaFisica.IdVinculacaoContratual);
                    }
                    //Se o cliente for maior de idade, este método que é executado e realiza a validação se o cliente possui clientes vinculados em inadimplência.
                    else {
                        int clientesVinculadosInadimplentes = _bancoContext.PessoaFisica.Where(x => x.IdVinculacaoContratual == pessoaFisica.Id && x.Adimplente == Adimplencia.Inadimplente).ToList().Count;
                        if (clientesVinculadosInadimplentes == 0) {
                            pessoaFisica.Adimplente = Adimplencia.Adimplente;
                            _bancoContext.PessoaFisica.Update(pessoaFisica);
                        }
                    }
                }
            }
            else if (pessoaJuridica != null) {
                int result = ReturnQtParcelasAtrasadaCliente(pessoaJuridica.Financeiros);
                if (result == 0) {
                    int clientesVinculadosInadimplentes = _bancoContext.PessoaFisica.Where(x => x.IdVinculacaoContratual == pessoaJuridica.Id && x.Adimplente == Adimplencia.Inadimplente).ToList().Count;
                    if (clientesVinculadosInadimplentes == 0) {
                        pessoaJuridica.Adimplente = Adimplencia.Adimplente;
                        _bancoContext.PessoaJuridica.Update(pessoaJuridica);
                    }
                }
            }
        }
        public int ReturnQtParcelasAtrasadaCliente(List<Financeiro> financeiros) {
            int cont = 0;
            foreach (var item in financeiros) {
                foreach (var item2 in item.Parcelas!) {
                    if (item2.StatusPagamento == SituacaoPagamento.Atrasada) cont++;
                }
            }
            return cont;
        }
        //Método que valida se o cliente tem parcelas atrasadas e outros clientes menores de idade com parcelas atrasadas,
        //caso não tenha, o mesmo é colocado em adimplência por não ter nenhuma infração das regras do contrato na aplicação. 
        public void ValidarAndSetAdimplenteClienteResponsavel(int? id) {
            PessoaFisica pessoaFisicaResponsavel = _bancoContext.PessoaFisica.Include(x => x.Financeiros).ThenInclude(x => x.Parcelas).FirstOrDefault(x => x.Id == id);
            if (pessoaFisicaResponsavel != null) {
                int resultParcelasAtrasadas = ReturnQtParcelasAtrasadaCliente(pessoaFisicaResponsavel.Financeiros);
                int clientesVinculadosInadimplentes = _bancoContext.PessoaFisica.Where(x => x.IdVinculacaoContratual == id && x.Adimplente == Adimplencia.Inadimplente).ToList().Count;
                if (resultParcelasAtrasadas == 0 && clientesVinculadosInadimplentes == 1) {
                    pessoaFisicaResponsavel.Adimplente = Adimplencia.Adimplente;
                    _bancoContext.Update(pessoaFisicaResponsavel);
                }
            }
            else {
                PessoaJuridica pessoaJuridicaResponsavel = _bancoContext.PessoaJuridica.Include(x => x.Financeiros).ThenInclude(x => x.Parcelas).FirstOrDefault(x => x.Id == id);
                if (pessoaJuridicaResponsavel != null) {
                    int resultParcelasAtrasadas = ReturnQtParcelasAtrasadaCliente(pessoaJuridicaResponsavel.Financeiros);
                    int clientesVinculadosInadimplentes = _bancoContext.PessoaFisica.Where(x => x.IdVinculacaoContratual == id && x.Adimplente == Adimplencia.Inadimplente).ToList().Count;
                    if (resultParcelasAtrasadas == 0 && clientesVinculadosInadimplentes == 1) {
                        pessoaJuridicaResponsavel.Adimplente = Adimplencia.Adimplente;
                        _bancoContext.Update(pessoaJuridicaResponsavel);
                    }
                }
            }
        }
        public void TaskMonitorParcelasLancamento() {
            var financeiros = _bancoContext.Financeiro
                .AsNoTracking().Include(x => x.Parcelas)
                .Where(x => x.Contrato == null).ToList();
            DateTime dateAtual = DateTime.Now.Date;
            foreach (var financeiro in financeiros) {
                foreach (var parcela in financeiro.Parcelas!) {
                    if (dateAtual > parcela.DataVencimentoParcela && parcela.StatusPagamento != SituacaoPagamento.PagamentoContabilizado && parcela.Financeiro!.FinanceiroStatus != FinanceiroStatus.Inativo) {
                        Parcela parcelaDB = _bancoContext.Parcela.FirstOrDefault(x => x.Id == parcela.Id);
                        parcelaDB!.StatusPagamento = SituacaoPagamento.Atrasada;
                        _bancoContext.Parcela.Update(parcelaDB);
                    }
                }
            }
            _bancoContext.SaveChanges();
        }
        //Método agendado que executa sem interação com o usuário. 
        public void TaskMonitorParcelas() {
            var contratos = _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.Aprovado)
                .AsNoTracking().Include(x => x.Financeiros).ThenInclude(x => x.Parcelas)
                .AsNoTracking().Include(x => x.Financeiros).ThenInclude(x => x.PessoaFisica)
                .AsNoTracking().Include(x => x.Financeiros).ThenInclude(x => x.PessoaJuridica)
                .ToList();

            //Verifica se a parcela está atrasada e realiza as devidas medidas. 
            DateTime dateAtual = DateTime.Now.Date;
            foreach (var contrato in contratos) {
                foreach (var financeiro in contrato.Financeiros!) {
                    foreach (var parcela in financeiro.Parcelas!) {
                        if (dateAtual > parcela.DataVencimentoParcela && parcela.StatusPagamento != SituacaoPagamento.PagamentoContabilizado) {
                            Parcela parcelaDB = _bancoContext.Parcela.FirstOrDefault(x => x.Id == parcela.Id);
                            parcelaDB!.StatusPagamento = SituacaoPagamento.Atrasada;
                            parcelaDB.ValorJuros = SetJurosParcela(parcelaDB, contrato);
                            var pessoaFisicaDB = _bancoContext.PessoaFisica.FirstOrDefault(x => x.Id == financeiro.PessoaFisicaId);
                            var pessoaJuridicaDB = _bancoContext.PessoaJuridica.FirstOrDefault(x => x.Id == financeiro.PessoaJuridicaId);
                            _bancoContext.Parcela.Update(parcelaDB);
                            if (pessoaFisicaDB != null) {
                                pessoaFisicaDB.Adimplente = Adimplencia.Inadimplente;
                                _bancoContext.PessoaFisica.Update(pessoaFisicaDB);
                                if (!string.IsNullOrEmpty(pessoaFisicaDB.IdVinculacaoContratual.ToString())) {
                                    SetInadimplenciaClienteResponsavel(pessoaFisicaDB.IdVinculacaoContratual!.Value);
                                }
                            }
                            else {
                                pessoaJuridicaDB!.Adimplente = Adimplencia.Inadimplente;
                                _bancoContext.PessoaJuridica.Update(pessoaJuridicaDB);
                            }
                        }
                    }
                }
                //Realiza a validação se o contrato pode ser encerrado ou não. Caso a condição seja antendida, o contrato é encerrado.
                if (dateAtual > contrato.DataVencimento) {
                    int contParcelasAtrasadasOrPendente = contrato.Financeiros.Where(x => x.Parcelas.Any(x2 => x2.StatusPagamento == SituacaoPagamento.Atrasada
                    || x2.StatusPagamento == SituacaoPagamento.AguardandoPagamento)).ToList().Count;
                    if (contParcelasAtrasadasOrPendente == 0) {
                        Contrato contratoDB = _bancoContext.Contrato.FirstOrDefault(x => x.Id == contrato.Id);
                        contratoDB!.Andamento = Andamento.Encerrado;
                        _bancoContext.Update(contratoDB);
                    }
                }
            }
            _bancoContext.SaveChanges();
        }
        public decimal? SetJurosParcela(Parcela financeiro, Contrato contrato) {
            DateTime dataAtual = DateTime.Now.Date;
            int qtMeses = ReturnQtmMeses(dataAtual) - ReturnQtmMeses(financeiro.DataVencimentoParcela!.Value.Date);
            if (qtMeses == 0) {
                decimal? valorJuros = (contrato.ValorParcelaContratoPorCliente * 2) / 100;
                return valorJuros;
            }
            else {
                decimal? valorJuros = ((contrato.ValorParcelaContratoPorCliente * (2 * (qtMeses + 1))) / 100);
                return valorJuros;
            }
        }
        public void SetInadimplenciaClienteResponsavel(int id) {
            PessoaFisica pessoaFisicaResponsavel = _bancoContext.PessoaFisica.FirstOrDefault(x => x.Id == id);
            if (pessoaFisicaResponsavel != null) {
                pessoaFisicaResponsavel.Adimplente = Adimplencia.Inadimplente;
                _bancoContext.PessoaFisica.Update(pessoaFisicaResponsavel);
            }
            else {
                PessoaJuridica pessoaJuridicaResponsavel = _bancoContext.PessoaJuridica.FirstOrDefault(x => x.Id == id);
                if (pessoaJuridicaResponsavel != null) {
                    pessoaJuridicaResponsavel.Adimplente = Adimplencia.Inadimplente;
                    _bancoContext.PessoaJuridica.Update(pessoaJuridicaResponsavel);
                }
            }
        }
        public int ReturnQtmMeses(DateTime date) {
            return date.Year * 12 + date.Month;
        }


        public ClientesContrato ConfirmarImpressaoPdfRescisao(ClientesContrato clientesContrato) {
            ClientesContrato clientesContratoDB = _bancoContext.ClientesContrato.FirstOrDefault(x => x.Id == clientesContrato.Id);
            clientesContratoDB!.ProcessRescisao = ProcessRescendir.PdfBaixado;
            clientesContratoDB.DataEmissaoPdfRescisao = DateTime.Now.Date;
            _bancoContext.ClientesContrato.Update(clientesContratoDB);
            _bancoContext.SaveChanges();
            return clientesContratoDB;
        }
        public Financeiro ListFinanceiroPorContratoAndClientesContrato(int? id) {
            ClientesContrato clientesContrato = _bancoContext.ClientesContrato.FirstOrDefault(x => x.Id == id);
            if (clientesContrato != null) {
                if (!string.IsNullOrEmpty(clientesContrato.PessoaFisicaId.ToString())) {
                    Financeiro financeiro = _bancoContext.Financeiro
                        .AsNoTracking().Include(x => x.PessoaFisica)
                        .AsNoTracking().Include(x => x.PessoaJuridica)
                        .AsNoTracking().Include(x => x.Contrato)
                        .FirstOrDefault(x => x.ContratoId == clientesContrato.ContratoId
                        && x.PessoaFisicaId == clientesContrato.PessoaFisicaId);
                    return financeiro!;
                }
                else {
                    Financeiro financeiro = _bancoContext.Financeiro
                        .AsNoTracking().Include(x => x.PessoaFisica)
                        .AsNoTracking().Include(x => x.PessoaJuridica)
                        .AsNoTracking().Include(x => x.Contrato)
                        .FirstOrDefault(x => x.ContratoId == clientesContrato.ContratoId
                        && x.PessoaJuridicaId == clientesContrato.PessoaJuridicaId);
                    return financeiro!;
                }
            }
            else {
                return null!;
            }
        }
        public Financeiro RescisaoContrato(int contratoId, int clienteId) {
            try {
                Financeiro financeiro = GetFinanceiroByContratoIdAndClienteId(contratoId, clienteId);
                if (financeiro.Parcelas.Any(x => x.StatusPagamento == SituacaoPagamento.Atrasada)) {
                    throw new Exception("Cliente tem parcelas atrasadas neste contrato!");
                }
                foreach (var parcela in financeiro.Parcelas) {
                    _bancoContext.Parcela.Remove(parcela);
                }
                //chamando o método que cria a rescisão no lugar do clientes contrato.
                Rescisao rescisao = new Rescisao();
                rescisao.DataRescisao = DateTime.Now.Date;
                rescisao.Contrato = financeiro.Contrato;
                rescisao.CalcularMultaContrato();
                if (!string.IsNullOrEmpty(financeiro.ValorTotalPago.ToString())) {
                    rescisao.ValorPagoContrato = financeiro.ValorTotalPago;
                }
                if (!string.IsNullOrEmpty(financeiro.PessoaFisicaId.ToString())) {
                    rescisao.PessoaFisicaId = financeiro.PessoaFisicaId;
                    ClientesContrato clientesContrato = _bancoContext.ClientesContrato.FirstOrDefault(x => x.ContratoId == financeiro.ContratoId
                        && x.PessoaFisicaId == financeiro.PessoaFisicaId);
                    _bancoContext.ClientesContrato.Remove(clientesContrato);
                }
                else {
                    if (!string.IsNullOrEmpty(financeiro.PessoaJuridicaId.ToString())) {
                        rescisao.PessoaJuridicaId = financeiro.PessoaJuridicaId;
                        ClientesContrato clientesContrato = _bancoContext.ClientesContrato.FirstOrDefault(x => x.ContratoId == financeiro.ContratoId
                        && x.PessoaJuridicaId == financeiro.PessoaJuridicaId);
                        _bancoContext.ClientesContrato.Remove(clientesContrato);
                    }
                    else {
                        throw new Exception("Desculpe, ID não foi encontrado!");
                    }
                }
                _bancoContext.Rescisao.Add(rescisao);
                _bancoContext.Financeiro.Remove(financeiro);
                _bancoContext.SaveChanges();
                return financeiro;
            }
            catch (Exception erro) {
                throw new Exception(erro.Message);
            }
        }
        public Financeiro GetFinanceiroByContratoIdAndClienteId(int contratoId, int clienteId) {
            return _bancoContext.Financeiro
                .Include(x => x.Parcelas)
                .Include(x => x.Contrato)
                .Include(x => x.PessoaFisica)
                .Include(x => x.PessoaJuridica)
                .AsNoTracking()      
                .FirstOrDefault(x => x.ContratoId == contratoId && (x.PessoaFisicaId == clienteId || x.PessoaJuridicaId == clienteId)) ?? throw new Exception("Desculpe, nenhum registro encontrado!");
        }

        public void TaskMonitorPdfRescisao() {
            List<ClientesContrato> clientesContratos = _bancoContext.ClientesContrato.ToList();
            DateTime dataAtual = DateTime.Now.Date;
            dataAtual.AddDays(2);
            foreach (var clienteContrato in clientesContratos) {
                if (!string.IsNullOrEmpty(clienteContrato.DataEmissaoPdfRescisao.ToString())) {
                    if (dataAtual > clienteContrato.DataEmissaoPdfRescisao!.Value.Date) {
                        clienteContrato.ProcessRescisao = ProcessRescendir.NoRescisao;
                        _bancoContext.ClientesContrato.Update(clienteContrato);
                    }
                }
            }
            _bancoContext.SaveChanges();
        }

        public Financeiro AdicionarLancamento(Financeiro financeiro) {
            try {
                financeiro.FinanceiroStatus = FinanceiroStatus.Ativo;
                _bancoContext.Financeiro.Add(financeiro);
                financeiro.ValorParcelaDR = financeiro.ValorTotDR / financeiro.QtParcelas;
                AdicionarParcelas(financeiro);
                _bancoContext.SaveChanges();
                return financeiro;
            }
            catch (Exception erro) {
                throw new Exception(erro.Message);
            }
        }
        public void AdicionarParcelas(Financeiro financeiro) {
            for (int parcelas = 1; parcelas <= financeiro.QtParcelas; parcelas++) {
                Parcela parcela = new Parcela {

                    Financeiro = financeiro, StatusPagamento = SituacaoPagamento.AguardandoPagamento,
                    DataVencimentoParcela = financeiro.DataEmissao!.Value.AddMonths(parcelas - 1), NomeParcela = parcelas.ToString()
                };
                if (parcelas == 1) {
                    parcela.DataVencimentoParcela = financeiro.DataEmissao.Value.AddDays(3);
                }
                _bancoContext.Parcela.Add(parcela);
            }
        }

        public Financeiro EditarLancamento(Financeiro financeiro) {
            try {
                Financeiro financeiroDB = _bancoContext.Financeiro.FirstOrDefault(x => x.Id == financeiro.Id);
                if (financeiroDB == null) throw new Exception("Desculpe, ID não foi encontrado!");
                if (!string.IsNullOrEmpty(financeiroDB.ContratoId.ToString())) throw new Exception("Desculpe, financeiro de contratos não podem ser editados!");
                if (!string.IsNullOrEmpty(financeiroDB.ValorTotalPago.ToString())) throw new Exception("Lançamento possuí parcelas paga!");
                if (financeiroDB.FinanceiroStatus == FinanceiroStatus.Inativo) throw new Exception("Desculpe, financeiro inativado!");
                financeiroDB.Pagament = financeiro.Pagament;
                financeiroDB.ValorTotDR = financeiro.ValorTotDR;
                financeiroDB.ValorParcelaDR = financeiro.ValorTotDR / financeiro.QtParcelas;
                financeiroDB.TypeEfetuacao = financeiro.TypeEfetuacao;
                financeiroDB.Detalhamento = financeiro.Detalhamento;
                financeiroDB.DataVencimento = financeiro.DataVencimento;
                if (financeiro.QtParcelas > financeiroDB.QtParcelas) {
                    for (int parcelas = financeiroDB.QtParcelas.Value + 1; parcelas <= financeiro.QtParcelas.Value; parcelas++) {
                        Parcela parcela = new Parcela {
                            FinanceiroId = financeiro.Id, StatusPagamento = SituacaoPagamento.AguardandoPagamento,
                            DataVencimentoParcela = financeiro.DataEmissao!.Value.AddMonths(parcelas - 1), NomeParcela = parcelas.ToString()
                        };
                        _bancoContext.Parcela.Add(parcela);
                    }
                }
                else if (financeiro.QtParcelas != financeiroDB.QtParcelas) {
                    for (int? parcelas = financeiro.QtParcelas + 1; parcelas <= financeiroDB.QtParcelas; parcelas++) {
                        Parcela parcela = _bancoContext.Parcela.FirstOrDefault(x => x.FinanceiroId == financeiro.Id && x.NomeParcela == parcelas.ToString());
                        _bancoContext.Parcela.Remove(parcela);
                    }
                }
                if (financeiroDB.DespesaReceita == DespesaReceita.Receita) {
                    if (!string.IsNullOrEmpty(financeiro.PessoaFisicaId.ToString())) {
                        financeiroDB.PessoaFisicaId = financeiro.PessoaFisicaId;
                        financeiroDB.PessoaJuridicaId = null;
                    }
                    else {
                        financeiroDB.PessoaJuridicaId = financeiro.PessoaJuridicaId;
                        financeiroDB.PessoaFisicaId = financeiro.PessoaFisicaId = null;
                    }
                }
                else {
                    financeiroDB.FornecedorId = financeiro.FornecedorId;
                }
                financeiroDB.QtParcelas = financeiro.QtParcelas;
                _bancoContext.Financeiro.Update(financeiroDB);
                _bancoContext.SaveChanges();
                return financeiroDB;

            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }
        //Atualiza a quantidade de parcelas de clientes que não foram excluídos, mas tiveram a quantidade de parcelas editadas.
        public void UpdateFinanceiro(Financeiro financeiro) {

        }


        public Financeiro InativarReceitaOrDespesa(int id) {
            try {
                Financeiro financeiroDB = listPorIdFinanceiro(id);
                if (financeiroDB == null || !string.IsNullOrEmpty(financeiroDB.ContratoId.ToString())) throw new Exception("Desculpe, ID não foi encontrado!");
                if (financeiroDB.Parcelas.Any(x => x.StatusPagamento == SituacaoPagamento.PagamentoContabilizado)) {
                    throw new Exception("Desculpe, receitas/despesas com parcelas contabilizadas não pode ser inativadas!");
                }
                financeiroDB.FinanceiroStatus = FinanceiroStatus.Inativo;
                _bancoContext.Financeiro.Update(financeiroDB);
                _bancoContext.SaveChanges();
                return financeiroDB;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }

        public List<Financeiro> GetPaginationAndFiltro(int pageNumber, string pesquisa, FiltroFinanceiro filtro) {
            if (pageNumber < 1 || string.IsNullOrEmpty(pageNumber.ToString())) throw new Exception("Ação inválida");
            switch (filtro) {
                case FiltroFinanceiro.Todos:
                    return _bancoContext.Financeiro
                       .OrderByDescending(x => x.Id)
                       .Where(x => x.PessoaFisica.Name.Contains(pesquisa) || x.PessoaJuridica.NomeFantasia.Contains(pesquisa) || x.Fornecedor.NameOrRazaoSocial.Contains(pesquisa) || x.Id.ToString().Contains(pesquisa) || x.ContratoId.ToString().Contains(pesquisa))
                            .Include(x => x.Contrato)
                            .Include(x => x.PessoaFisica)
                            .Include(x => x.PessoaJuridica)
                            .Include(x => x.Fornecedor)
                            .Include(x => x.Parcelas)
                            .AsNoTracking()
                            .Skip((pageNumber - 1) * 10)
                            .Take(10)
                            .ToList();
                case FiltroFinanceiro.Contrato:
                    return _bancoContext.Financeiro
                       .OrderByDescending(x => x.Id)
                       .Where(x => !string.IsNullOrEmpty(x.ContratoId.ToString()) && (x.PessoaFisica.Name.Contains(pesquisa) ||  x.PessoaJuridica.NomeFantasia.Contains(pesquisa) || x.Fornecedor.NameOrRazaoSocial.Contains(pesquisa) || x.Id.ToString().Contains(pesquisa) || x.ContratoId.ToString().Contains(pesquisa)))
                            .Include(x => x.Contrato)
                            .Include(x => x.PessoaFisica)
                            .Include(x => x.PessoaJuridica)
                            .Include(x => x.Fornecedor)
                            .Include(x => x.Parcelas)
                            .AsNoTracking()
                            .Skip((pageNumber - 1) * 10)
                            .Take(10)
                            .ToList();
                case FiltroFinanceiro.Atrasadas:
                    return _bancoContext.Financeiro
                       .OrderByDescending(x => x.Id)
                       .Where(x => x.Parcelas.Any(x => x.StatusPagamento == SituacaoPagamento.Atrasada) && ((x.PessoaFisica.Name.Contains(pesquisa) || x.PessoaJuridica.NomeFantasia.Contains(pesquisa) || x.Fornecedor.NameOrRazaoSocial.Contains(pesquisa) || x.Id.ToString().Contains(pesquisa) || x.ContratoId.ToString().Contains(pesquisa))))
                            .Include(x => x.Contrato)
                            .Include(x => x.PessoaFisica)
                            .Include(x => x.PessoaJuridica)
                            .Include(x => x.Fornecedor)
                            .Include(x => x.Parcelas)
                            .AsNoTracking()
                            .Skip((pageNumber - 1) * 10)
                            .Take(10)
                            .ToList();
                default:
                    return _bancoContext.Financeiro
                      .OrderByDescending(x => x.Id)
                      .Where(x => x.DespesaReceita == (DespesaReceita)filtro && (x.PessoaFisica.Name.Contains(pesquisa) || x.PessoaJuridica.NomeFantasia.Contains(pesquisa) || x.Fornecedor.NameOrRazaoSocial.Contains(pesquisa) || x.Id.ToString().Contains(pesquisa) || x.ContratoId.ToString().Contains(pesquisa)))
                           .Include(x => x.Contrato)
                           .Include(x => x.PessoaFisica)
                           .Include(x => x.PessoaJuridica)
                           .Include(x => x.Fornecedor)
                           .Include(x => x.Parcelas)
                           .AsNoTracking()
                           .Skip((pageNumber - 1) * 10)
                           .Take(10)
                           .ToList();
            }
        }

        public int ReturnQtPaginas(string pesquisa, FiltroFinanceiro filtro) {

            var qtFinanceiro = 1;
            switch (filtro) {
                case FiltroFinanceiro.Todos:
                    qtFinanceiro = _bancoContext.Financeiro.Count(x => x.PessoaFisica.Name.Contains(pesquisa) || x.PessoaJuridica.NomeFantasia.Contains(pesquisa) || x.Fornecedor.NameOrRazaoSocial.Contains(pesquisa) || x.Id.ToString().Contains(pesquisa) || x.ContratoId.ToString().Contains(pesquisa));
                    break;
                case FiltroFinanceiro.Contrato:
                    qtFinanceiro = _bancoContext.Financeiro.Count(x => !string.IsNullOrEmpty(x.ContratoId.ToString()) && (x.PessoaFisica.Name.Contains(pesquisa) && x.PessoaJuridica.NomeFantasia.Contains(pesquisa) || x.Fornecedor.NameOrRazaoSocial.Contains(pesquisa) || x.Id.ToString().Contains(pesquisa) || x.ContratoId.ToString().Contains(pesquisa)));
                    break;
                case FiltroFinanceiro.Atrasadas:
                    qtFinanceiro = _bancoContext.Financeiro.Count(x => x.Parcelas.Any(x => x.StatusPagamento == SituacaoPagamento.Atrasada) && ((x.PessoaFisica.Name.Contains(pesquisa) || x.PessoaJuridica.NomeFantasia.Contains(pesquisa) || x.Fornecedor.NameOrRazaoSocial.Contains(pesquisa) || x.Id.ToString().Contains(pesquisa) || x.ContratoId.ToString().Contains(pesquisa))));
                    break;
                default:
                    qtFinanceiro = _bancoContext.Financeiro.Count(x => x.DespesaReceita == (DespesaReceita)filtro && (x.PessoaFisica.Name.Contains(pesquisa) && x.PessoaJuridica.NomeFantasia.Contains(pesquisa) || x.Fornecedor.NameOrRazaoSocial.Contains(pesquisa) || x.Id.ToString().Contains(pesquisa) || x.ContratoId.ToString().Contains(pesquisa)));
                    break;
            }

            int qtPage = (int)Math.Ceiling((double)qtFinanceiro / 10);
            return (qtPage == 0) ? 1 : qtPage;
        }

        public List<Parcela> GetPaginationAndFiltroParcelas(int id, int pageNumber, string pesquisa) {
            return _bancoContext.Parcela.Where(x => x.FinanceiroId == id && x.NomeParcela.Contains(pesquisa))
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToList();
        }

        public int ReturnQtPaginasParcelas(int id, string pesquisa) {
            int qtParcelas = _bancoContext.Parcela.Count(x => x.FinanceiroId == id && x.NomeParcela.Contains(pesquisa));
            int qtPaginas = (int)Math.Ceiling((double)qtParcelas / 10);
            return (qtPaginas == 0) ? 1 : qtPaginas;
        }

        public Financeiro listPorIdFinanceiroNoJoinParcelas(int? id) {
            return _bancoContext.Financeiro
                .AsNoTracking().Include(x => x.PessoaFisica)
                .AsNoTracking().Include(x => x.PessoaJuridica)
                .AsNoTracking().Include(x => x.Contrato)
                .AsNoTracking().Include(x => x.Fornecedor)
                .FirstOrDefault(x => x.Id == id) ?? throw new Exception("Nenhum registro encontrado!");
        }

    }
}
