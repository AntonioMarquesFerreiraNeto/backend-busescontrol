using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository.Interfaces {
    public interface IFornecedorRepository {
        public Fornecedor CreateFornecedor(Fornecedor fornecedor);
        public Fornecedor UpdateFornecedor(Fornecedor fornecedor);
        public Fornecedor AtivarFornecedor(int id);
        public Fornecedor InativarFornecedor(int id);
        public Fornecedor GetFornecedorById(int id);
        public List<Fornecedor> GetFornecedoresAtivos(int paginaAtual, bool status);
        public List<Fornecedor> GetFornecedoresInativos(int paginaAtual, bool status);
        public List<Fornecedor> GetAllFornecedoresAutorizados();
        public int GetTotPaginasAtivos();
        public int GetTotPaginasInativos();
    }
}
