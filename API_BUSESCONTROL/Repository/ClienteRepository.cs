using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public PessoaFisica ClienteResponsavel(int id) {
            return _bancoContext.PessoaFisica.FirstOrDefault(x => x.Id == id)!;
        }

        public PessoaFisica UpdateCliente(PessoaFisica cliente) {
            try {
                PessoaFisica clienteDB = GetClienteFisicoById(cliente.Id);
                if (DuplicataEditar(cliente, clienteDB)) throw new Exception("Cliente já se encontra registrado!");
                if (ValClienTResponAlterInvalid(cliente, clienteDB)) throw new Exception("Cliente inadimplente não pode ter vinculação alterada!");
                if (string.IsNullOrEmpty(clienteDB.IdVinculacaoContratual.ToString())) {
                    if (ValidationContratoAndamento(clienteDB.Id, cliente.IdVinculacaoContratual)) throw new Exception("Cliente possui contratos/receitas. Portanto, não pode ser menor de idade!");
                }
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
                if (!string.IsNullOrEmpty(clienteDB.IdVinculacaoContratual.ToString()) && _bancoContext.PessoaFisica.Any(x => x.IdVinculacaoContratual == clienteDB.Id)) {
                    throw new Exception("Cliente possui menores de idade vinculado!");
                }
                _bancoContext.PessoaFisica.Update(clienteDB);
                _bancoContext.SaveChanges();
                return clienteDB;

            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }
        public bool ValidationContratoAndamento(int id, int? vinculacao) {
            if (string.IsNullOrEmpty(vinculacao.ToString())) return false;
            if (_bancoContext.Financeiro.Any(x => x.PessoaFisicaId == id)) {
                return true;
            }
            return false;
        }
        //Método que não deixa cliente inadimplente ter seu responsável alterado. 
        public bool ValClienTResponAlterInvalid(PessoaFisica pessoaFisica, PessoaFisica pessoaFisicaDB) {
            if (pessoaFisicaDB.Adimplente == Adimplencia.Inadimplente && pessoaFisica.IdVinculacaoContratual != pessoaFisicaDB.IdVinculacaoContratual) {
                return true;
            }
            return false;
        }

        public PessoaFisica GetClienteFisicoById(int? id) {
            PessoaFisica cliente = _bancoContext.PessoaFisica
                .Include(x => x.Financeiros)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id) ?? throw new Exception("Desculpe, cliente não encontrado!");
            return cliente;
        }

        public PessoaFisica InativarCliente(int? id) {
            PessoaFisica cliente = GetClienteFisicoById(id);
            if (_bancoContext.PessoaFisica.Any(x => x.IdVinculacaoContratual == cliente.Id)) {
                throw new Exception("Cliente possui vinculo com menor de idade em contratos em andamento!");
            }
            if (ValContratoAndamentoPf(cliente.Id)) throw new Exception("Cliente possui contratos em andamento ou encerrados!");
            if (cliente.Adimplente == Adimplencia.Inadimplente) throw new Exception("Não é possível inativar cliente inadimplente!");
            if (cliente.Financeiros.Any(x => x.FinanceiroStatus == FinanceiroStatus.Ativo)) throw new Exception("Cliente/fornecedor possui financeiro em andamento!");
            DesabilitarClientesVinculados(cliente, null!);
            cliente.Status = ClienteStatus.Inativo;
            _bancoContext.PessoaFisica.Update(cliente);
            _bancoContext.SaveChanges();
            return cliente;
        }
        public bool ValContratoAndamentoPf(int id) {
            bool retorno = _bancoContext.PessoaFisica.Any(x => x.Id == id && x.ClientesContrato.Any(x => x.Contrato!.Andamento != Andamento.Encerrado)) ? true : false;
            return retorno;
        }

        public PessoaFisica AtivarCliente(int? id) {
            PessoaFisica cliente = GetClienteFisicoById(id);
            cliente.Status = ClienteStatus.Ativo;
            _bancoContext.PessoaFisica.Update(cliente);
            _bancoContext.SaveChanges();
            return cliente;
        }

        public List<PessoaFisica> GetClientesAtivos(int paginaAtual, string pesquisa) {
            string pesquisaTel = pesquisa.Replace("-", "");
            if (paginaAtual < 1) throw new Exception("Desculpe, ação inválida!");
            return _bancoContext.PessoaFisica
                    .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.Contrato)
                    .Where(x => x.Status == ClienteStatus.Ativo && (x.Name!.Contains(pesquisa) || x.Telefone!.Contains(pesquisaTel) || x.Cidade!.Contains(pesquisa) || x.Adimplente == SearchByAdimplencia(pesquisa)))
                    .Skip((paginaAtual - 1) * 10).Take(10).ToList();
        }

        public List<PessoaFisica> GetClientesInativos(int paginaAtual, string pesquisa) {
            string pesquisaTel = pesquisa.Replace("-", "");
            if (paginaAtual < 1) throw new Exception("Desculpe, ação inválida!");
            return _bancoContext.PessoaFisica
                .Include(x => x.ClientesContrato).ThenInclude(x => x.Contrato)
                .AsNoTracking()
                .Where(x => x.Status == ClienteStatus.Inativo && (x.Name!.Contains(pesquisa) || x.Telefone!.Contains(pesquisaTel) || x.Cidade!.Contains(pesquisa) || x.Adimplente == SearchByAdimplencia(pesquisa)))
                .Skip((paginaAtual - 1) * 10).Take(10).ToList();
        }

        public int QtPaginasClientesAtivos(string pesquisa) {
            string pesquisaTel = pesquisa.Replace("-", "");
            var totClientes = _bancoContext.PessoaFisica.Count(x => x.Status == ClienteStatus.Ativo && (x.Name!.Contains(pesquisa) || x.Telefone!.Contains(pesquisaTel) || x.Cidade!.Contains(pesquisa) || x.Adimplente == SearchByAdimplencia(pesquisa)));
            int qtPaginas = (int)Math.Ceiling((double)totClientes / 10);
            return (qtPaginas == 0) ? 1 : qtPaginas;
        }

        public int QtPaginasClientesInativos(string pesquisa) {
            string pesquisaTel = pesquisa.Replace("-", "");
            var totClientes = _bancoContext.PessoaFisica.Count(x => x.Status == ClienteStatus.Inativo && (x.Name!.Contains(pesquisa) || x.Telefone!.Contains(pesquisaTel) || x.Cidade!.Contains(pesquisa) || x.Adimplente == SearchByAdimplencia(pesquisa)));
            int qtPaginas = (int)Math.Ceiling((double)totClientes / 10);
            return (qtPaginas == 0) ? 1 : qtPaginas;
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
            return _bancoContext.PessoaFisica.Where(x => string.IsNullOrEmpty(x.IdVinculacaoContratual.ToString()) && x.Status == ClienteStatus.Ativo && x.Adimplente == Adimplencia.Adimplente).ToList();
        }
        public List<PessoaJuridica> GetClientesParaVinculacaoPJ() {
            return _bancoContext.PessoaJuridica.Where(x => x.Status == ClienteStatus.Ativo && x.Adimplente == Adimplencia.Adimplente).ToList();
        }
        public List<PessoaFisica> GetClientesAdimplentes() {
            return _bancoContext.PessoaFisica.Where(x => x.Adimplente == Adimplencia.Adimplente && x.Status == ClienteStatus.Ativo).ToList();
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
            if (_bancoContext.PessoaFisica.Any(x => x.IdVinculacaoContratual == clienteDB.Id)) {
                throw new Exception("Cliente possui vinculo com menor de idade em contratos em andamento!");
            }
            if (ValContratoAndamentoPj(clienteDB.Id)) throw new Exception("Cliente possui contratos em andamento ou encerrados!");
            if (clienteDB.Adimplente == Adimplencia.Inadimplente) throw new Exception("Não é possível inativar cliente inadimplente!");
            if (clienteDB.Financeiros.Any(x => x.FinanceiroStatus == FinanceiroStatus.Ativo)) throw new Exception("Cliente/fornecedor possui financeiro em andamento!");

            DesabilitarClientesVinculados(null!, clienteDB);
            clienteDB.Status = ClienteStatus.Inativo;
            _bancoContext.PessoaJuridica.Update(clienteDB);
            _bancoContext.SaveChanges();
            return clienteDB;
        }
        public bool ValContratoAndamentoPj(int id) {
            bool retorno = _bancoContext.PessoaJuridica.Any(x => x.Id == id && x.ClientesContrato.Any(x => x.Contrato!.Andamento != Andamento.Encerrado)) ? true : false;
            return retorno;
        }

        public PessoaJuridica AtivarClientePJ(int? id) {
            PessoaJuridica clienteDB = GetClienteByIdPJ(id);
            clienteDB.Status = ClienteStatus.Ativo;
            _bancoContext.PessoaJuridica.Update(clienteDB);
            _bancoContext.SaveChanges();
            return clienteDB;
        }

        public PessoaJuridica GetClienteByIdPJ(int? id) {
            return _bancoContext.PessoaJuridica
                .Include(x => x.Financeiros)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id) ?? throw new Exception("Desculpe, cliente não encontrado!");
        }

        public List<PessoaJuridica> GetClientesAtivosPJ(int paginaAtual, string pesquisa) {
            string pesquisaTel = pesquisa.Replace("-", "");
            if (paginaAtual < 1) throw new Exception("Desculpe, ação inválida!");
            return _bancoContext.PessoaJuridica
                   .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.Contrato)
                   .Where(x => x.Status == ClienteStatus.Ativo && (x.RazaoSocial!.Contains(pesquisa) || x.Telefone!.Contains(pesquisaTel) || x.Cidade!.Contains(pesquisa) || x.Adimplente == SearchByAdimplencia(pesquisa)))
                   .Skip((paginaAtual - 1) * 10).Take(10).ToList();
        }

        public List<PessoaJuridica> GetClientesInativosPJ(int paginaAtual, string pesquisa) {
            string pesquisaTel = pesquisa.Replace("-", "");
            if (paginaAtual < 1) throw new Exception("Desculpe, ação inválida!");
            return _bancoContext.PessoaJuridica
                   .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.Contrato)
                   .Where(x => x.Status == ClienteStatus.Inativo && (x.RazaoSocial!.Contains(pesquisa) || x.Telefone!.Contains(pesquisaTel) || x.Cidade!.Contains(pesquisa) || x.Adimplente == SearchByAdimplencia(pesquisa)))
                   .Skip((paginaAtual - 1) * 10).Take(10).ToList();
        }

        public int QtPaginasClientesAtivosPJ(string pesquisa) {
            string pesquisaTel = pesquisa.Replace("-", "");
            int totClientes = _bancoContext.PessoaJuridica.Count(x => x.Status == ClienteStatus.Ativo && (x.RazaoSocial!.Contains(pesquisa) || x.Telefone!.Contains(pesquisaTel) || x.Cidade!.Contains(pesquisa) || x.Adimplente == SearchByAdimplencia(pesquisa)));
            int qtPaginas = (int)Math.Ceiling((double)totClientes / 10);
            return (qtPaginas == 0) ? 1 : qtPaginas;
        }

        public int QtPaginasClientesInativosPJ(string pesquisa) {
            string pesquisaTel = pesquisa.Replace("-", "");
            int totClientes = _bancoContext.PessoaJuridica.Count(x => x.Status == ClienteStatus.Inativo && (x.RazaoSocial!.Contains(pesquisa) || x.Telefone!.Contains(pesquisaTel) || x.Cidade!.Contains(pesquisa) || x.Adimplente == SearchByAdimplencia(pesquisa)));
            int qtPaginas = (int)Math.Ceiling((double)totClientes / 10);
            return (qtPaginas == 0) ? 1 : qtPaginas;
        }

        private Adimplencia? SearchByAdimplencia(string pesquisa) {
            pesquisa = pesquisa.ToLower();
            if ("adimplente".Contains(pesquisa)) {
                return Adimplencia.Adimplente;
            } else if ("inadimplente".Contains(pesquisa)) {
                return Adimplencia.Inadimplente;
            }
            return null;
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

        public void DesabilitarClientesVinculados(PessoaFisica pessoaFisica, PessoaJuridica pessoaJuridica) {
            if (pessoaFisica != null) {
                List<PessoaFisica> clientes = _bancoContext.PessoaFisica.Where(x => x.IdVinculacaoContratual == pessoaFisica.Id).ToList();
                foreach (var model in clientes) {
                    model.Status = ClienteStatus.Inativo;
                    _bancoContext.Update(model);
                    _bancoContext.SaveChanges();
                }
            }
            else if (pessoaJuridica != null) {
                List<PessoaFisica> clientes = _bancoContext.PessoaFisica.Where(x => x.IdVinculacaoContratual == pessoaJuridica.Id).ToList();
                foreach (var model in clientes) {
                    model.Status = ClienteStatus.Inativo;
                    _bancoContext.Update(model);
                    _bancoContext.SaveChanges();
                }
            }
        }
    }
}
