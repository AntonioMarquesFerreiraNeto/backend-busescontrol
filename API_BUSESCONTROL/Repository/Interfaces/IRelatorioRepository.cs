using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models.ModelsGraficAnalytics;

namespace API_BUSESCONTROL.Repository.Interfaces {
    public interface IRelatorioRepository {
        public decimal? ValorTotAprovados();
        public decimal? ValorTotEmAnalise();
        public decimal? ValorTotReprovados();
        public decimal? ValorTotPagoContrato();
        public decimal? ValorTotJurosCliente(int? id);
        public decimal? ValorTotPagoReceitas();
        public decimal? ValorTotPagoDespesas();
        public decimal? ValorTotReceitas();
        public decimal? ValorTotDespesas();
        public decimal? ValorJurosAndMultas();
        public decimal? ValorReceitasComuns();
        public int QtContratosEncerrados();
        public int QtContratosAprovados();
        public int QtContratosEmAnalise();
        public int QtContratosNegados();
        public int QtContratos();
        public int QtClientesAdimplentes();
        public int QtClientesInadimplentes();
        public int QtClientesVinculados();
        public int QtClientes();
        public List<Contrato> ListContratosAprovados(string pesquisa, int statusAndamento);
        public Contrato GetContratoById(int id);
        public SimpleAnalytics ReturnSimpleAnalytics();
    }
}
