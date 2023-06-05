using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using System.Linq;

namespace API_BUSESCONTROL.Repository {
    public class ClienteRepository : IClienteRepository {

        private readonly BancoContext _bancoContext;

        public ClienteRepository(BancoContext bancoContext) {
            _bancoContext = bancoContext;
        }

        public PessoaFisica CreateCliente(PessoaFisica cliente) {
            try {
                cliente = TrimPessoaFisica(cliente);
                if (Duplicata(cliente)) throw new Exception("Cliente já se encontra registrado!");
                _bancoContext.PessoaFisica.Add(cliente);
                _bancoContext.SaveChanges();
                return cliente;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }

        public PessoaFisica UpdateCliente(PessoaFisica cliente) {
            try {
                PessoaFisica clienteDB = GetClienteFisicoById(cliente.Id);
                if (DuplicataEditar(cliente, clienteDB)) throw new Exception("Cliente já se encontra registrado!");
                if (_bancoContext.PessoaFisica.Any(x => x.IdVinculacaoContratual == clienteDB.Id)) throw new Exception("Cliente responsável por outro cliente não pode ser menor de idade!");
                clienteDB.Name = cliente.Name!.Trim();
                clienteDB.DataNascimento = cliente.DataNascimento;
                clienteDB.Cpf = cliente.Cpf!.Trim();
                clienteDB.Rg = cliente.Rg!.Trim();
                clienteDB.Email = cliente.Email!.Trim();
                clienteDB.Telefone = cliente.Telefone!.Trim();
                clienteDB.NameMae = cliente.NameMae!.Trim();
                clienteDB.IdVinculacaoContratual = cliente.IdVinculacaoContratual;
                clienteDB.Cep = cliente.Cep!.Trim();
                clienteDB.ComplementoResidencial = cliente.ComplementoResidencial!.Trim();
                clienteDB.NumeroResidencial = cliente.NumeroResidencial!.Trim();
                clienteDB.Logradouro = cliente.Logradouro!.Trim();
                clienteDB.Ddd = cliente.Ddd!.Trim();
                clienteDB.Bairro = cliente.Bairro!.Trim();
                clienteDB.Estado = cliente.Estado!.Trim();
                clienteDB.Cidade = cliente.Cidade!.Trim();
                _bancoContext.PessoaFisica.Update(clienteDB);
                _bancoContext.SaveChanges();
                return clienteDB;
                
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }

        public PessoaFisica GetClienteFisicoById(int? id) {
            PessoaFisica cliente = _bancoContext.PessoaFisica.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Desculpe, cliente encontrado!");
            return cliente;
        }

        public PessoaFisica InativarCliente(int? id) {
            PessoaFisica cliente = GetClienteFisicoById(id);
            cliente.Status = ClienteStatus.Inativo;
            _bancoContext.PessoaFisica.Update(cliente);
            _bancoContext.SaveChanges();
            return cliente;
        }

        public PessoaFisica AtivarCliente(int? id) {
            PessoaFisica cliente = GetClienteFisicoById(id);
            cliente.Status = ClienteStatus.Ativo;
            _bancoContext.PessoaFisica.Update(cliente);
            _bancoContext.SaveChanges();
            return cliente;
        }

        public List<PessoaFisica> GetClientesAtivos(int paginaAtual, bool statusPaginate) {
            if (statusPaginate == true) {
                int indiceInicial = (paginaAtual - 1) * 10;
                return _bancoContext.PessoaFisica.Where(x => x.Status == ClienteStatus.Ativo).Skip(indiceInicial).Take(10).ToList();
            }
            if (paginaAtual < 2) throw new Exception("Desculpe, ação inválida!");
            int indice = (paginaAtual - 2) * 10;
            return _bancoContext.PessoaFisica.Where(x => x.Status == ClienteStatus.Ativo).Skip(indice).Take(10).ToList();
        }

        public List<PessoaFisica> GetClientesInativos(int paginaAtual, bool statusPaginate) {
            if (statusPaginate == true) {
                int indiceInicial = (paginaAtual - 1) * 10;
                return _bancoContext.PessoaFisica.Where(x => x.Status == ClienteStatus.Inativo).Skip(indiceInicial).Take(10).ToList();
            }
            if (paginaAtual < 2) throw new Exception("Desculpe, ação inválida");
            int indice = (paginaAtual - 2) * 10;
            return _bancoContext.PessoaFisica.Where(x => x.Status == ClienteStatus.Inativo).Skip(indice).Take(10).ToList();
        }

        public int QtPaginasClientesAtivos() {
            var totClientes = _bancoContext.PessoaFisica.Count(x => x.Status == ClienteStatus.Ativo);
            int qtPaginas = (int)Math.Ceiling((double)totClientes / 10);
            return qtPaginas;
        }

        public int QtPaginasClientesInativos() {
            var totClientes = _bancoContext.PessoaFisica.Count(x => x.Status == ClienteStatus.Inativo);
            int qtPaginas = (int)Math.Ceiling((double)totClientes / 10);
            return qtPaginas;
        }

        public bool Duplicata(PessoaFisica cliente) {
            if (_bancoContext.PessoaFisica.Any(x => x.Cpf == cliente.Cpf || x.Telefone == cliente.Telefone || x.Email!.ToLower() == cliente.Email!.ToLower())) {
                return true;
            }
            return false;
        }

        public bool DuplicataEditar(PessoaFisica cliente, PessoaFisica clienteDB) {
            if (_bancoContext.PessoaFisica.Any(x => (x.Cpf == cliente.Cpf && cliente.Cpf != clienteDB.Cpf)
                || (x.Email!.ToLower() == cliente.Email!.ToLower() && cliente.Email!.ToLower() != clienteDB.Email!.ToLower())
                || (x.Telefone == cliente.Telefone && cliente.Telefone != clienteDB.Telefone))) {
                return true;
            }
            return false;
        }

        public PessoaFisica TrimPessoaFisica(PessoaFisica value) {
            value.Name = value.Name!.Trim();
            value.Rg = value.Rg!.Trim();
            value.Telefone = value.Telefone!.Trim();
            value.NameMae = value.NameMae!.Trim();
            value.Cep = value.Cep!.Trim();
            value.ComplementoResidencial = value.ComplementoResidencial!.Trim();
            value.Logradouro = value.Logradouro!.Trim();
            value.NumeroResidencial = value.NumeroResidencial!.Trim();
            value.Ddd = value.Ddd!.Trim();
            value.Bairro = value.Bairro!.Trim();
            value.Cidade = value.Cidade!.Trim();
            value.Estado = value.Estado!.Trim();

            return value;
        }

        public List<PessoaFisica> GetClientesParaVinculacao() {
            return _bancoContext.PessoaFisica.Where(x => string.IsNullOrEmpty(x.IdVinculacaoContratual.ToString()) && x.Status == ClienteStatus.Ativo).ToList();
        }

        public PessoaJuridica CreateClientePJ(PessoaJuridica cliente) {
            try {
                cliente = PessoaJuridicaTrim(cliente);
                if (DuplicataJuridica(cliente)) throw new Exception("Cliente já se encontra registrado!");
                _bancoContext.PessoaJuridica.Add(cliente);
                _bancoContext.SaveChanges();
                return cliente;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }

        public PessoaJuridica UpdateClientePJ(PessoaJuridica cliente) {
            try {
                PessoaJuridica clienteDB = GetClienteByIdPJ(cliente.Id);
                if (DuplicataEditarJuridica(cliente, clienteDB)) throw new Exception("Cliente já se encontra registrado!");
                clienteDB.NomeFantasia = cliente.NomeFantasia!.Trim();
                clienteDB.RazaoSocial = cliente.RazaoSocial!.Trim();
                clienteDB.InscricaoMunicipal = cliente.InscricaoMunicipal!.Trim();
                clienteDB.InscricaoEstadual = cliente.InscricaoEstadual!.Trim();
                clienteDB.Cnpj = cliente.Cnpj;
                clienteDB.Email = cliente.Email!.Trim();
                clienteDB.Telefone = cliente.Telefone!.Trim();
                clienteDB.Cep = cliente.Cep!.Trim();
                clienteDB.ComplementoResidencial = cliente.ComplementoResidencial!.Trim();
                clienteDB.NumeroResidencial = cliente.NumeroResidencial!.Trim();
                clienteDB.Logradouro = cliente.Logradouro!.Trim();
                clienteDB.Ddd = cliente.Ddd!.Trim();
                clienteDB.Bairro = cliente.Bairro!.Trim();
                clienteDB.Estado = cliente.Estado!.Trim();
                clienteDB.Cidade = cliente.Cidade!.Trim();

                _bancoContext.PessoaJuridica.Update(clienteDB);
                _bancoContext.SaveChanges();
                return clienteDB;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }

        public PessoaJuridica InativarClientePJ(int? id) {
            PessoaJuridica clienteDB = GetClienteByIdPJ(id);
            clienteDB.Status = ClienteStatus.Inativo;
            _bancoContext.PessoaJuridica.Update(clienteDB);
            _bancoContext.SaveChanges();
            return clienteDB;
        }

        public PessoaJuridica AtivarClientePJ(int? id) {
            PessoaJuridica clienteDB = GetClienteByIdPJ(id);
            clienteDB.Status = ClienteStatus.Ativo;
            _bancoContext.PessoaJuridica.Update(clienteDB);
            _bancoContext.SaveChanges();
            return clienteDB;
        }

        public PessoaJuridica GetClienteByIdPJ(int? id) {
            return _bancoContext.PessoaJuridica.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Desculpe, cliente não encontrado!");
        }

        public List<PessoaJuridica> GetClientesAtivosPJ(int paginaAtual, bool statusPagina) {
            if (statusPagina) {
                int indiceInicial = (paginaAtual - 1) * 10;
                return _bancoContext.PessoaJuridica.Where(x => x.Status == ClienteStatus.Ativo).Skip(indiceInicial).Take(10).ToList();
            }
            if (paginaAtual < 2) throw new Exception("Desculpe, ação inválida!");
            int indice = (paginaAtual - 2) * 10;
            return _bancoContext.PessoaJuridica.Where(x => x.Status == ClienteStatus.Ativo).Skip(indice).Take(10).ToList();
        }

        public List<PessoaJuridica> GetClientesInativosPJ(int paginaAtual, bool statusPagina) {
            if (statusPagina) {
                int indiceInicial = (paginaAtual - 1) * 10;
                return _bancoContext.PessoaJuridica.Where(x => x.Status == ClienteStatus.Inativo).Skip(indiceInicial).Take(10).ToList();
            }
            if (paginaAtual < 2) throw new Exception("Desculpe, ação inválida!");
            int indice = (paginaAtual - 2) * 10;
            return _bancoContext.PessoaJuridica.Where(x => x.Status == ClienteStatus.Inativo).Skip(indice).Take(10).ToList();
        }

        public int QtPaginasClientesAtivosPJ() {
            int totClientes = _bancoContext.PessoaJuridica.Count(x => x.Status == ClienteStatus.Ativo);
            int qtPaginas = (int)Math.Ceiling((double)totClientes / 10);
            return qtPaginas;
        }

        public int QtPaginasClientesInativosPJ() {
            int totClientes = _bancoContext.PessoaJuridica.Count(x => x.Status == ClienteStatus.Inativo);
            int qtPaginas = (int)Math.Ceiling((double)totClientes / 10);
            return qtPaginas;
        }

        public PessoaJuridica PessoaJuridicaTrim(PessoaJuridica value) {
            value.NomeFantasia = value.NomeFantasia!.Trim();
            value.RazaoSocial = value.RazaoSocial!.Trim();
            value.InscricaoMunicipal = value.InscricaoMunicipal!.Trim();
            value.InscricaoEstadual = value.InscricaoEstadual!.Trim();
            value.Email = value.Email!.Trim();
            value.Telefone = value.Telefone!.Trim();
            value.Cep = value.Cep!.Trim();
            value.ComplementoResidencial = value.ComplementoResidencial!.Trim();
            value.NumeroResidencial = value.NumeroResidencial!.Trim();
            value.Logradouro = value.Logradouro!.Trim();
            value.Ddd = value.Ddd!.Trim();
            value.Bairro = value.Bairro!.Trim();
            value.Estado = value.Estado!.Trim();
            value.Cidade = value.Cidade!.Trim();
            return value;
        }

        public bool DuplicataJuridica(PessoaJuridica cliente) {
            if (_bancoContext.PessoaJuridica.Any(x => x.Cnpj == cliente.Cnpj || x.RazaoSocial == cliente.RazaoSocial
                || cliente.NomeFantasia == x.NomeFantasia || x.Telefone == cliente.Telefone || x.Email == cliente.Email
                || cliente.InscricaoEstadual == x.InscricaoEstadual)) {
                return true;
            }
            return false;
        }

        public bool DuplicataEditarJuridica(PessoaJuridica cliente, PessoaJuridica clienteDB) {
            if (_bancoContext.PessoaJuridica.Any(x => (x.Cnpj == cliente.Cnpj && cliente.Cnpj != clienteDB.Cnpj)
                || (x.Email == cliente.Email && cliente.Email != clienteDB.Email)
                || (x.Telefone == cliente.Telefone && cliente.Telefone != clienteDB.Telefone)
                || (cliente.InscricaoEstadual == x.InscricaoEstadual && cliente.InscricaoEstadual != clienteDB.InscricaoEstadual)
                || (cliente.RazaoSocial == x.RazaoSocial && cliente.RazaoSocial != clienteDB.RazaoSocial)
                || (cliente.NomeFantasia == x.NomeFantasia && cliente.NomeFantasia != clienteDB.NomeFantasia))) {
                return true;
            }
            return false;
        }

        public List<PessoaJuridica> GetClientesParaVinculacaoPJ() {
            return _bancoContext.PessoaJuridica.Where(x => x.Status == ClienteStatus.Ativo).ToList();
        }
    }
}
