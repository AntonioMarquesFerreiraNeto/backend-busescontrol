using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository.Interfaces {
    public interface IFornecedorRepository {
        public Fornecedor CreateFornecedor(Fornecedor fornecedor);
        public Fornecedor UpdateFornecedor(Fornecedor fornecedor);
        public Fornecedor InativarFornecedor(int id);
        public Fornecedor AtivarFornecedor(int id);
        public Fornecedor GetFornecedorById(int id);
        public List<Fornecedor> GetFornecedoresAtivos();
        public List<Fornecedor> GetFornecedoresInativos();
    }
}
