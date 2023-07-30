using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository.Interfaces {
    public interface IFinanceiroRepository {
        public List<Financeiro> ListFinanceiros();
        public Financeiro ReturnPorId(int id);
        public Financeiro listPorIdFinanceiro(int? id);
        public Parcela ListarFinanceiroPorId(int id);
        public Parcela ContabilizarParcela(int id);
        public Contrato ListarJoinPorId(int id);
        public Financeiro RescisaoContrato(Financeiro financeiro);
        public Financeiro AdicionarLancamento(Financeiro financeiro);
        public Financeiro EditarLancamento(Financeiro financeiro);
        public Financeiro InativarReceitaOrDespesa(Financeiro financeiro);
        public void TaskMonitorParcelas();
        public void TaskMonitorParcelasLancamento();
        public void TaskMonitorPdfRescisao();
        public ClientesContrato ConfirmarImpressaoPdf(ClientesContrato clientesContrato);
        public Financeiro ListFinanceiroPorContratoAndClientesContrato(int? id);
        public List<Financeiro> GetPaginationAndFiltro(int pageNumber, string pesquisa);
        public int ReturnQtPaginas(string pesquisa);
    }
}
