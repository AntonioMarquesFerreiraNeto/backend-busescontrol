using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository {
    public interface IContratoRepository {
        public Contrato CreateContrato(Contrato contrato, List<ClientesContrato> lista);
        public Contrato UpdateContrato(Contrato contrato);
        public Contrato InativarContrato(int id);
        public Contrato AtivarContrato(int id);
        public Contrato GetContratoById(int id);
        public List<Contrato> GetContratosAtivos(int paginaAtual, bool statusPag);
        public List<Contrato> GetContratosInativos(int paginaAtual, bool statusPag);
        public int ReturnQtPaginasAtivos();
        public int ReturnQtPaginasInativos();
    }
}
