using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;

namespace API_BUSESCONTROL.Repository.Interfaces
{
    public interface IContratoRepository
    {
        public Contrato CreateContrato(Contrato contrato, List<ClientesContrato> lista);
        public Contrato UpdateContrato(Contrato contrato, List<ClientesContrato> lista);
        public Contrato InativarContrato(int id);
        public Contrato AtivarContrato(int id);
        public Contrato AprovarContrato(int id);
        public Contrato RevogarContrato(int id);
        public Contrato GetContratoById(int id);
        public List<Contrato> ListContratosAprovados();
        public List<Contrato> ListContratosEmAnalise();
        public List<Contrato> GetContratosAtivos(int paginaAtual, FiltroContrato filtro, int pageSize, string pesquisa);
        public List<Contrato> GetContratosInativos(int paginaAtual, string pesquisa);
        public List<Contrato> GetAllContratosAtivos();
        public List<Contrato> GetAllContratosInativos();
        public ClientesContrato GetClientesContratoById(int id);
        public int ReturnQtPaginasAtivos(FiltroContrato filtro, int pageSize, string pesquisa);
        public int ReturnQtPaginasInativos(string pesquisa);
        public Contrato CreateMotoristaSubstituto(SubContratoMotorista subContratoMotorista);
        public Contrato UpdateMotoristaSubstituto(SubContratoMotorista subContratoMotorista);
        public Contrato DeleteMotoristaSubstituto(int id);
        public Contrato CreateOnibusSubstituto(SubContratoOnibus subContratoOnibus);
        public Contrato UpdateOnibusSubstituto(SubContratoOnibus subContratoOnibus);
        public Contrato DeleteOnibusSubstituto(int id);
    }
}
