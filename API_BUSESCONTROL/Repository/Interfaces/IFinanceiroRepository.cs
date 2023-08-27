using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;

namespace API_BUSESCONTROL.Repository.Interfaces {
    public interface IFinanceiroRepository {
        public List<Financeiro> ListFinanceiros();
        public Financeiro ReturnPorId(int id);
        public Financeiro listPorIdFinanceiro(int? id);
        public Parcela ListarFinanceiroPorId(int id);
        public Parcela ContabilizarParcela(int id);
        public Contrato ListarJoinPorId(int id);
        public Financeiro RescisaoContrato(int contratoId, int clienteId);
        public Financeiro AdicionarLancamento(Financeiro financeiro);
        public Financeiro EditarLancamento(Financeiro financeiro);
        public Financeiro InativarReceitaOrDespesa(int id);
        public void TaskMonitorParcelas();
        public void TaskMonitorParcelasLancamento();
        public void TaskMonitorPdfRescisao();
        public ClientesContrato ConfirmarImpressaoPdfRescisao(ClientesContrato clientesContrato);
        public Financeiro ListFinanceiroPorContratoAndClientesContrato(int? id);
        public List<Financeiro> GetPaginationAndFiltro(int pageNumber, string pesquisa, FiltroFinanceiro filtro);
        public int ReturnQtPaginas(string pesquisa, FiltroFinanceiro filtro);
        public List<Parcela> GetPaginationAndFiltroParcelas(int id, int pageNumber, string pesquisa);
        public int ReturnQtPaginasParcelas(int id, string pesquisa);
        public Financeiro listPorIdFinanceiroNoJoinParcelas(int? id);
    }
}
