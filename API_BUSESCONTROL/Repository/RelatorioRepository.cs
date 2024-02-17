using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using API_BUSESCONTROL.Models.ModelsGraficAnalytics;
using System.Globalization;

namespace API_BUSESCONTROL.Repository {

    public class RelatorioRepository : IRelatorioRepository {

        private readonly BancoContext _bancoContext;

        public RelatorioRepository(BancoContext bancoContext) {
            _bancoContext = bancoContext;
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
            return _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.Aprovado).Sum(x => x.ValorMonetario);
        }
        public decimal? ValorTotEmAnalise() {
            return _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.EmAnalise).Sum(x => x.ValorMonetario);
        }
        public decimal? ValorTotReprovados() {
            return _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.Negado).Sum(x => x.ValorMonetario);
        }
        public decimal? ValorTotPagoContrato() {
            return _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.Aprovado).Sum(x => x.ValorTotalPagoContrato);
        }
        public decimal? ValorTotPagoReceitas() {
            var valorJuros = _bancoContext.Rescisao.Sum(x => x.Multa);
            return _bancoContext.Financeiro
                    .Where(x => x.FinanceiroStatus == FinanceiroStatus.Ativo && x.DespesaReceita == DespesaReceita.Receita)
                    .Sum(x => x.ValorTotalPago) + valorJuros;
        }

        public decimal? ValorTotPagoDespesas() {
            return _bancoContext.Financeiro.Where(x => x.DespesaReceita == DespesaReceita.Despesa).Sum(x => x.ValorTotalPago);
        }

        public decimal? ValorTotReceitas() {
            return _bancoContext
                    .Financeiro
                    .Where(x => x.FinanceiroStatus == FinanceiroStatus.Ativo && x.DespesaReceita == DespesaReceita.Receita)
                    .Sum(x => x.ValorTotDR);
        }

        public decimal? ValorTotDespesas() {
            return _bancoContext
                    .Financeiro
                    .Where(x => x.FinanceiroStatus == FinanceiroStatus.Ativo && x.DespesaReceita == DespesaReceita.Despesa)
                    .Sum(x => x.ValorTotDR);
        }
        public decimal? ValorTotJurosCliente(int? id) {
            return _bancoContext.Parcela.Where(x => x.FinanceiroId == id).Sum(x => x.ValorJuros);
        }

        public decimal? ValorJurosAndMultas() {
            var multas = _bancoContext.Rescisao.Sum(x => x.Multa);
            return _bancoContext
                    .Parcela
                    .Sum(x => x.ValorJuros) + multas;
        }

        public decimal? ValorReceitasComuns() {
            return _bancoContext
                    .Financeiro
                    .Where(x => x.DespesaReceita == DespesaReceita.Receita && x.FinanceiroStatus == FinanceiroStatus.Ativo && x.ContratoId == null)
                    .Sum(x => x.ValorTotDR);
        }

        public int QtContratosEncerrados() {
            return _bancoContext.Contrato.Count(x => x.Andamento == Andamento.Encerrado);
        }
        public int QtContratosAprovados() {
            return _bancoContext.Contrato.Count(x => x.Aprovacao == StatusAprovacao.Aprovado && x.StatusContrato == ContratoStatus.Ativo);
        }
        public int QtContratosEmAnalise() {
            return _bancoContext.Contrato.Count(x => x.Aprovacao == StatusAprovacao.EmAnalise && x.StatusContrato == ContratoStatus.Ativo);
        }
        public int QtContratosNegados() {
            return _bancoContext.Contrato.Count(x => x.Aprovacao == StatusAprovacao.Negado && x.StatusContrato == ContratoStatus.Ativo);
        }
        public int QtContratos() {
            return _bancoContext.Contrato.Count(x => x.StatusContrato == ContratoStatus.Ativo);
        }
        public int QtClientesAdimplentes() {
            int quantidade = _bancoContext.PessoaFisica.Count(x => x.Adimplente == Adimplencia.Adimplente && x.Status == ClienteStatus.Ativo);
            quantidade += _bancoContext.PessoaJuridica.Count(x => x.Adimplente == Adimplencia.Adimplente && x.Status == ClienteStatus.Ativo);
            return quantidade;
        }
        public int QtClientesInadimplentes() {
            int quantidade = _bancoContext.PessoaFisica.Count(x => x.Adimplente == Adimplencia.Inadimplente && x.Status == ClienteStatus.Ativo);
            quantidade += _bancoContext.PessoaJuridica.Count(x => x.Adimplente == Adimplencia.Inadimplente && x.Status == ClienteStatus.Ativo);
            return quantidade;
        }
        public int QtClientesVinculados() {
            int quantidade = _bancoContext.PessoaFisica.Count(x => x.ClientesContrato.Any(x => x.Contrato.Aprovacao == StatusAprovacao.Aprovado));
            quantidade += _bancoContext.PessoaJuridica.Count(x => x.ClientesContrato.Any(x => x.Contrato.Aprovacao == StatusAprovacao.Aprovado));
            return quantidade;
        }
        public int QtClientes() {
            int quantidade = _bancoContext.PessoaFisica.Count(x => x.Status == ClienteStatus.Ativo);
            quantidade += _bancoContext.PessoaJuridica.Count(x => x.Status == ClienteStatus.Ativo);
            return quantidade;
        }

        public SimpleAnalytics ReturnSimpleAnalytics() {
            SimpleAnalytics dataResponse = new SimpleAnalytics();
            DateTime minDate = _bancoContext.Financeiro.Where(x => x.FinanceiroStatus == FinanceiroStatus.Ativo).Select(x => x.DataEmissao).Min()!.Value.Date;
            DateTime maxDate = _bancoContext.Financeiro.Where(x => x.FinanceiroStatus == FinanceiroStatus.Ativo).Select(x => x.DataEmissao).Max()!.Value.Date;
            minDate = new DateTime(minDate.Year, minDate.Month, DateTime.Now.Day);
            maxDate = new DateTime(maxDate.Year, maxDate.Month, DateTime.Now.Day);
            
            while (minDate <= maxDate) {
                decimal valorMensalReceita = _bancoContext.Financeiro
                        .Where(x => x.FinanceiroStatus == FinanceiroStatus.Ativo && x.DespesaReceita == DespesaReceita.Receita && x.DataEmissao.Value.Year == minDate.Year && x.DataEmissao.Value.Month == minDate.Month)
                        .Sum(x => x.ValorTotDR) ?? 0;
                decimal valorMensalDespesa = _bancoContext.Financeiro
                        .Where(x => x.FinanceiroStatus == FinanceiroStatus.Ativo && x.DespesaReceita == DespesaReceita.Despesa && x.DataEmissao.Value.Year == minDate.Year && x.DataEmissao.Value.Month == minDate.Month)
                        .Sum(x => x.ValorTotDR) ?? 0;

                var simpleReceita = new SimpleReceitas { ValTotMothYear = Math.Round(valorMensalReceita, 2), DateMothYear = minDate.ToString("MM/yyyy") };
                var simpleDespesa = new SimpleDespesas { ValTotMothYear = Math.Round(valorMensalDespesa, 2), DateMothYear = minDate.ToString("MM/yyyy") };

                dataResponse.LabelsDate.Add(minDate.ToString("MM/yyyy"));
                dataResponse.SimpleReceitasList?.Add(simpleReceita);
                dataResponse.SimpleDespesasList?.Add(simpleDespesa);
                minDate = minDate.AddMonths(1);
            }
            return dataResponse;
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
