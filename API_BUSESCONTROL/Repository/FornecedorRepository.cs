using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;

namespace API_BUSESCONTROL.Repository {
    public class FornecedorRepository : IFornecedorRepository {

        private readonly BancoContext _bancoContext;

        public FornecedorRepository(BancoContext bancoContext) {
            _bancoContext = bancoContext;
        }

        public Fornecedor CreateFornecedor(Fornecedor fornecedor) {
            try {
                _bancoContext.Fornecedor.Add(fornecedor);
                _bancoContext.SaveChanges();
                return fornecedor;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }

        public Fornecedor UpdateFornecedor(Fornecedor fornecedor) {
            throw new NotImplementedException();
        }


        public Fornecedor AtivarFornecedor(int id) {
            throw new NotImplementedException();
        }

        public Fornecedor InativarFornecedor(int id) {
            throw new NotImplementedException();
        }

        public Fornecedor GetFornecedorById(int id) {
            throw new NotImplementedException();
        }

        public List<Fornecedor> GetFornecedoresAtivos() {
            throw new NotImplementedException();
        }

        public List<Fornecedor> GetFornecedoresInativos() {
            throw new NotImplementedException();
        }
    }
}
