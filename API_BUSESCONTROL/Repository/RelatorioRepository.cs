using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_BUSESCONTROL.Repository {
    public class RelatorioRepository : IRelatorioRepository {
        private readonly BancoContext _bancoContext;
        private readonly IContratoRepository _contratoRepositorio;
        private readonly IFinanceiroRepository _financeiroRepositorio;

        public RelatorioRepository(BancoContext bancoContext, IContratoRepository contratoRepositorio, IFinanceiroRepository financeiroRepositorio) {
            _bancoContext = bancoContext;
            _contratoRepositorio = contratoRepositorio;
            _financeiroRepositorio = financeiroRepositorio;
        }

        public Contrato GetContratoById(int id) {
            return _bancoContext.Contrato
                .AsNoTracking().Include("Motorista")
                .AsNoTracking().Include("Onibus")
                .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                .AsNoTracking().Include(x => x.Financeiros).ThenInclude(x => x.Parcelas)
                .AsNoTracking().Include(x => x.Financeiros).ThenInclude(x => x.PessoaFisica)
                .AsNoTracking().Include(x => x.Financeiros).ThenInclude(x => x.PessoaJuridica)
                .AsNoTracking().Include(x => x.Rescisoes).ThenInclude(x => x.PessoaFisica)
                .AsNoTracking().Include(x => x.Rescisoes).ThenInclude(x => x.PessoaJuridica)
                .FirstOrDefault(x => x.Id == id && x.Aprovacao == StatusAprovacao.Aprovado);
        }

        public decimal? ValorTotAprovados() {
            List<Contrato> ListContrato = _contratoRepositorio.ListContratosAprovados();
            decimal? valorTotalContrato = 0;
            foreach (Contrato contrato in ListContrato) {
                valorTotalContrato += contrato.ValorMonetario;
            }
            return valorTotalContrato;
        }
        public decimal? ValorTotEmAnalise() {
            List<Contrato> ListContrato = _contratoRepositorio.ListContratosEmAnalise();
            decimal? valorTotalContrato = 0;
            foreach (Contrato contrato in ListContrato) {
                valorTotalContrato += contrato.ValorMonetario;
            }
            return valorTotalContrato;
        }
        public decimal? ValorTotReprovados() {
            List<Contrato> ListContrato = _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.Negado).ToList();
            decimal? valorTotalContrato = 0;
            foreach (Contrato contrato in ListContrato) {
                valorTotalContrato += contrato.ValorMonetario;
            }
            return valorTotalContrato;
        }
        public decimal? ValorTotContratos() {
            List<Contrato> ListContrato = _contratoRepositorio.ListContratosAprovados();
            ListContrato.AddRange(_contratoRepositorio.ListContratosEmAnalise());
            decimal? valorTot = 0;
            foreach (Contrato contrato in ListContrato) {
                valorTot += contrato.ValorMonetario;
            }
            return valorTot;
        }
        public decimal? ValorTotPagoContrato() {
            List<Contrato> contratos = _contratoRepositorio.ListContratosAprovados();
            decimal? valorPago = 0;
            foreach (var item in contratos) {
                if (!string.IsNullOrEmpty(item.ValorTotalPagoContrato.ToString())) {
                    valorPago += item.ValorTotalPagoContrato;
                }
                foreach (var rescisao in item.Rescisoes) {
                    if (!string.IsNullOrEmpty(rescisao.Multa.ToString())) {
                        valorPago += rescisao.Multa;
                    }
                }
            }
            return valorPago;
        }
        public decimal? ValorTotPendenteContrato() {
            List<Contrato> contratos = _contratoRepositorio.ListContratosAprovados();
            decimal? valorPago = 0;
            decimal? valorTotal = 0;
            foreach (var item in contratos) {
                if (!string.IsNullOrEmpty(item.ValorTotalPagoContrato.ToString())) {
                    valorPago += item.ValorTotalPagoContrato;
                }
                valorTotal += item.ValorMonetario;
            }
            decimal? valorPedente = valorTotal - valorPago;
            return valorPedente;
        }
        public decimal? ValorTotPagoReceitas() {
            List<Financeiro> financeiros = _financeiroRepositorio.ListFinanceiros();
            decimal? valorPago = 0;
            foreach (var financeiro in financeiros) {
                if (financeiro.FinanceiroStatus == FinanceiroStatus.Ativo && financeiro.DespesaReceita == DespesaReceita.Receita) {
                    if (!string.IsNullOrEmpty(financeiro.ValorTotalPago.ToString())) {
                        valorPago += financeiro.ValorTotalPago;
                    }
                }
            }
            List<Rescisao> rescisoes = _bancoContext.Rescisao.Where(x => !string.IsNullOrEmpty(x.Multa.ToString())).ToList();
            foreach (var rescisao in rescisoes) {
                valorPago += rescisao.Multa;
            }
            return valorPago;
        }

        public decimal? ValorTotPagoDespesas() {
            List<Financeiro> financeiros = _financeiroRepositorio.ListFinanceiros();
            decimal? valorPago = 0;
            foreach (var financeiro in financeiros) {
                if (financeiro.FinanceiroStatus == FinanceiroStatus.Ativo && financeiro.DespesaReceita == DespesaReceita.Despesa) {
                    if (!string.IsNullOrEmpty(financeiro.ValorTotalPago.ToString())) {
                        valorPago += financeiro.ValorTotalPago;
                    }
                }
            }
            return valorPago;
        }

        public decimal? ValorTotReceitas() {
            List<Financeiro> financeiros = _financeiroRepositorio.ListFinanceiros();
            decimal? valorPago = 0;
            foreach (var financeiro in financeiros) {
                if (financeiro.FinanceiroStatus == FinanceiroStatus.Ativo && financeiro.DespesaReceita == DespesaReceita.Receita) {
                    valorPago += financeiro.ValorTotDR;
                }
            }
            return valorPago;
        }

        public decimal? ValorTotDespesas() {
            List<Financeiro> financeiros = _financeiroRepositorio.ListFinanceiros();
            decimal? valorPago = 0;
            foreach (var financeiro in financeiros) {
                if (financeiro.FinanceiroStatus == FinanceiroStatus.Ativo && financeiro.DespesaReceita == DespesaReceita.Despesa) {
                    valorPago += financeiro.ValorTotDR;
                }
            }
            return valorPago;
        }
        public decimal? ValorTotJurosCliente(int? id) {
            List<Parcela> financeiros = _bancoContext.Parcela.Where(x => x.FinanceiroId == id).ToList();
            decimal? totValorJuros = 0;
            foreach (var item in financeiros) {
                if (!string.IsNullOrEmpty(item.ValorJuros.ToString())) {
                    totValorJuros += item.ValorJuros;
                }
            }
            return totValorJuros;
        }

        public int QtContratosAprovados() {
            int quantidade = _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.Aprovado && x.StatusContrato == ContratoStatus.Ativo).ToList().Count;
            return quantidade;
        }
        public int QtContratosEmAnalise() {
            int quantidade = _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.EmAnalise && x.StatusContrato == ContratoStatus.Ativo).ToList().Count;
            return quantidade;
        }
        public int QtContratosNegados() {
            int quantidade = _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.Negado && x.StatusContrato == ContratoStatus.Ativo).ToList().Count;
            return quantidade;
        }
        public int QtContratos() {
            int quantidade = _bancoContext.Contrato.Where(x => x.StatusContrato == ContratoStatus.Ativo).ToList().Count;
            return quantidade;
        }
        public int QtClientesAdimplentes() {
            int quantidade = _bancoContext.PessoaFisica.Where(x => x.Adimplente == Adimplencia.Adimplente && x.Status == ClienteStatus.Ativo).ToList().Count;
            quantidade += _bancoContext.PessoaJuridica.Where(x => x.Adimplente == Adimplencia.Adimplente && x.Status == ClienteStatus.Ativo).ToList().Count;
            return quantidade;
        }
        public int QtClientesInadimplentes() {
            int quantidade = _bancoContext.PessoaFisica.Where(x => x.Adimplente == Adimplencia.Inadimplente && x.Status == ClienteStatus.Ativo).ToList().Count;
            quantidade += _bancoContext.PessoaJuridica.Where(x => x.Adimplente == Adimplencia.Inadimplente && x.Status == ClienteStatus.Ativo).ToList().Count;
            return quantidade;
        }
        public int QtClientesVinculados() {
            int quantidade = _bancoContext.PessoaFisica.Count(x => x.ClientesContrato.Any(x => x.Contrato.Aprovacao == StatusAprovacao.Aprovado));
            quantidade += _bancoContext.PessoaJuridica.Count(x => x.ClientesContrato.Any(x => x.Contrato.Aprovacao == StatusAprovacao.Aprovado));
            return quantidade;
        }
        public int QtClientes() {
            int quantidade = _bancoContext.PessoaFisica.Where(x => x.Status == ClienteStatus.Ativo).ToList().Count;
            quantidade += _bancoContext.PessoaJuridica.Where(x => x.Status == ClienteStatus.Ativo).ToList().Count;
            return quantidade;
        }
        public int QtMotoristas() {
            int quantidade = _bancoContext.Funcionario
                  .Where(x => x.Status == FuncionarioStatus.Ativo && x.Cargo == CargoFuncionario.Motorista).ToList().Count;
            return quantidade;
        }
        public int QtMotoristasVinculados() {
            int quantidade = _bancoContext.Funcionario
                .Where(x => x.Status == FuncionarioStatus.Ativo && x.Cargo == CargoFuncionario.Motorista && x.Contratos.Any(x => x.Aprovacao == StatusAprovacao.Aprovado)).ToList().Count;
            return quantidade;
        }
        public int QtOnibus() {
            int quantidade = _bancoContext.Onibus
                .Where(x => x.StatusOnibus == StatusFrota.Ativo).ToList().Count;
            return quantidade;
        }
        public int QtOnibusVinculados() {
            int quantidade = _bancoContext.Onibus
                .Where(x => x.StatusOnibus == StatusFrota.Ativo && x.Contratos.Any(x => x.Aprovacao == StatusAprovacao.Aprovado)).ToList().Count;
            return quantidade;
        }

        public List<Contrato> ListContratosAprovados(string pesquisa, int statusAndamento) {
            if (statusAndamento == 0) {
                return _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.Aprovado && x.Id.ToString().Contains(pesquisa))
                                   .AsNoTracking().Include("Motorista")
                                   .AsNoTracking().Include("Onibus")
                                   .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                                   .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                                   .AsNoTracking().Include(x => x.Financeiros).ThenInclude(x => x.Parcelas)
                                   .ToList();
            }
            else {
                return _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.Aprovado && x.Andamento == (Andamento)statusAndamento && x.Id.ToString().Contains(pesquisa))
                                   .AsNoTracking().Include("Motorista")
                                   .AsNoTracking().Include("Onibus")
                                   .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                                   .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                                   .AsNoTracking().Include(x => x.Financeiros).ThenInclude(x => x.Parcelas)
                                   .ToList();
            }
        }
    }
}
