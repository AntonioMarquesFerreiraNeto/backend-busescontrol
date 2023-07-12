using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Migrations;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;

namespace API_BUSESCONTROL.Repository {
    public class FornecedorRepository : IFornecedorRepository {

        private readonly BancoContext _bancoContext;

        public FornecedorRepository(BancoContext bancoContext) {
            _bancoContext = bancoContext;
        }

        public Fornecedor CreateFornecedor(Fornecedor fornecedor) {
            try {
                fornecedor.TrimFornecedor();
                if (ValidarDuplicata(fornecedor)) throw new Exception("Fornecedor já se encontra registrado!");
                _bancoContext.Fornecedor.Add(fornecedor);
                _bancoContext.SaveChanges();
                return fornecedor;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }

        public Fornecedor UpdateFornecedor(Fornecedor fornecedor) {
            try {
                Fornecedor fornecedorDB = GetFornecedorById(fornecedor.Id);
                if (ValidarDuplicataEdit(fornecedor, fornecedorDB)) throw new Exception("Fornecedor já se encontra registrado!");
                fornecedorDB.NameOrRazaoSocial = fornecedor.NameOrRazaoSocial!.Trim();
                fornecedorDB.DataFornecedor = fornecedor.DataFornecedor;
                fornecedorDB.Telefone = fornecedor.Telefone;
                fornecedorDB.Email = fornecedor.Email;
                fornecedorDB.TypePessoa = fornecedor.TypePessoa;
                if (fornecedorDB.TypePessoa == TypePessoa.PessoaFisica) fornecedorDB.Cpf = fornecedor.Cpf;
                else fornecedorDB.Cnpj = fornecedor.Cnpj;
                fornecedorDB.Cep = fornecedor.Cep!.Trim();
                fornecedorDB.Logradouro = fornecedor.Logradouro!.Trim(); ;
                fornecedorDB.NumeroResidencial = fornecedor.NumeroResidencial!.Trim();
                fornecedorDB.Bairro = fornecedor.Bairro!.Trim();
                fornecedorDB.Cidade = fornecedor.Cidade!.Trim();
                fornecedorDB.Estado = fornecedor.Estado!.Trim();
                fornecedorDB.Ddd = fornecedor.Ddd!.Trim();

                _bancoContext.Fornecedor.Update(fornecedorDB);
                _bancoContext.SaveChanges();
                return fornecedorDB;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }


        public Fornecedor AtivarFornecedor(int id) {
            Fornecedor fornecedorDB = GetFornecedorById(id);
            fornecedorDB.Status = FornecedorStatus.Ativo;
            _bancoContext.Update(fornecedorDB);
            _bancoContext.SaveChanges();
            return fornecedorDB;
        }

        public Fornecedor InativarFornecedor(int id) {
            Fornecedor fornecedorDB = GetFornecedorById(id);
            fornecedorDB.Status = FornecedorStatus.Inativo;
            _bancoContext.Update(fornecedorDB);
            _bancoContext.SaveChanges();
            return fornecedorDB;
        }

        public Fornecedor GetFornecedorById(int id) {
            return _bancoContext.Fornecedor.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Nenhum registro encontrado!");
        }

        public List<Fornecedor> GetFornecedoresAtivos(int paginaAtual, bool status) {
            if (status) {
                int indice = (paginaAtual - 1) * 10;
                return _bancoContext.Fornecedor.Where(x => x.Status == FornecedorStatus.Ativo).Skip(indice).Take(10).ToList();
            }
            if (paginaAtual < 2) throw new Exception("Ação inválido!");
            int indiceInicial = (paginaAtual - 2) * 10;
            return _bancoContext.Fornecedor.Where(x => x.Status == FornecedorStatus.Ativo).Skip(indiceInicial).Take(10).ToList();
        }

        public List<Fornecedor> GetFornecedoresInativos(int paginaAtual, bool status) {
            if (status) {
                int indice = (paginaAtual - 1) * 10;
                return _bancoContext.Fornecedor.Where(x => x.Status == FornecedorStatus.Inativo).Skip(indice).Take(10).ToList();
            }
            if (paginaAtual < 2) throw new Exception("Ação inválido!");
            int indiceInicial = (paginaAtual - 2) * 10;
            return _bancoContext.Fornecedor.Where(x => x.Status == FornecedorStatus.Inativo).Skip(indiceInicial).Take(10).ToList();
        }

        public int GetTotPaginasAtivos() {
            int qtItens = _bancoContext.Fornecedor.Count(x => x.Status == FornecedorStatus.Ativo);
            int qtPaginas = (int)Math.Ceiling((double)qtItens /10);
            return qtPaginas;
        }
        public int GetTotPaginasInativos() {
            int qtItens = _bancoContext.Fornecedor.Count(x => x.Status == FornecedorStatus.Inativo);
            int qtPaginas = (int)Math.Ceiling((double)qtItens / 10);
            return qtPaginas;
        }

        public bool ValidarDuplicata(Fornecedor fornecedor) {
            bool existeFornecedor = _bancoContext.Fornecedor.Any(x =>
                (x.NameOrRazaoSocial == fornecedor.NameOrRazaoSocial || x.Email == fornecedor.Email || x.Telefone == fornecedor.Telefone) ||
                (fornecedor.TypePessoa == TypePessoa.PessoaFisica && x.Cpf == fornecedor.Cpf) ||
                (fornecedor.TypePessoa == TypePessoa.PessoaJuridica && x.Cnpj == fornecedor.Cnpj)
            );

            return existeFornecedor;
        }

        public bool ValidarDuplicataEdit(Fornecedor fornecedor, Fornecedor fornecedorDB) {
            bool existeOutroFornecedor = _bancoContext.Fornecedor.Any(x =>
                    ((x.NameOrRazaoSocial == fornecedor.NameOrRazaoSocial && fornecedor.NameOrRazaoSocial != fornecedorDB.NameOrRazaoSocial) ||
                    (x.Email == fornecedor.Email && fornecedor.Email != fornecedorDB.Email) ||
                    (x.Telefone == fornecedor.Telefone && fornecedor.Telefone != fornecedorDB.Telefone) ||
                    (fornecedor.TypePessoa == TypePessoa.PessoaFisica && x.Cpf == fornecedor.Cpf && fornecedor.Cpf != fornecedorDB.Cpf) ||
                    (fornecedor.TypePessoa == TypePessoa.PessoaJuridica && x.Cnpj == fornecedor.Cnpj && fornecedor.Cnpj != fornecedorDB.Cnpj))
            ) ;

            return existeOutroFornecedor;
        }
    }
}
