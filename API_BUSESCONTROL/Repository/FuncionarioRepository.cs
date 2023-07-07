using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Helpers;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace API_BUSESCONTROL.Repository
{
    public class FuncionarioRepository : IFuncionarioRepository {

        private readonly BancoContext _bancoContext;
        private readonly IEmail _email;

        public FuncionarioRepository(BancoContext bancoContext, IEmail email) {
            _bancoContext = bancoContext;
            _email = email;
        }

        public Funcionario CreateFuncionario(Funcionario funcionario) {
            try {
                if (Duplicata(funcionario)) throw new Exception("Funcionário já se encontra registrado!");
                funcionario = TrimFuncionario(funcionario);
                if (funcionario.Cargo != CargoFuncionario.Motorista) {
                    funcionario.Senha = funcionario.GerarSenha();
                    if (!EnviarSenha(funcionario)) throw new Exception("Desculpe, não conseguimos enviar o e-mail com a senha.");
                    funcionario.setPasswordHash();
                    funcionario.StatusUsuario = UsuarioStatus.Ativo;
                }
                _bancoContext.Funcionario.Add(funcionario);
                _bancoContext.SaveChanges();
                return funcionario;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }

        public Funcionario GetFuncionarioById(int? id) {
            Funcionario funcionario = _bancoContext.Funcionario.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Desculpe, funcionário não encontrado!"); ;
            return funcionario;
        }

        public Funcionario UpdateFuncionario(Funcionario funcionario) {
            try {
                Funcionario funcionarioDB = GetFuncionarioById(funcionario.Id);
                if (DuplicataEditar(funcionario, funcionarioDB)) throw new Exception("Funcionário já se encontra registrado!");
                if (ValidarVinculoMotorista(funcionario.Cargo, funcionarioDB.Id)) throw new Exception("Motoristas com contratos não podem ter seus cargos alterados!");
                if (funcionario.Cargo != CargoFuncionario.Motorista && string.IsNullOrEmpty(funcionarioDB.Senha)) {
                    funcionarioDB.Senha = funcionarioDB.GerarSenha();
                    if (!EnviarSenha(funcionarioDB)) throw new Exception("Desculpe, não conseguimos enviar o e-mail com a senha.");
                    funcionarioDB.setPasswordHash();
                    funcionarioDB.StatusUsuario = UsuarioStatus.Ativo;
                }
                if (funcionario.Cargo == CargoFuncionario.Motorista) funcionarioDB.StatusUsuario = UsuarioStatus.Inativo;
                funcionarioDB.Cargo = funcionario.Cargo;
                funcionarioDB.Name = funcionario.Name!.Trim();
                funcionarioDB.DataNascimento = funcionario.DataNascimento;
                funcionarioDB.Cpf = funcionario.Cpf!.Trim();
                funcionarioDB.Telefone = funcionario.Telefone!.Trim();
                funcionarioDB.Email = funcionario.Email!.Trim();
                funcionarioDB.Cep = funcionario.Cep!.Trim();
                funcionarioDB.NumeroResidencial = funcionario.NumeroResidencial!.Trim();
                funcionarioDB.Logradouro = funcionario.Logradouro!.Trim();
                funcionarioDB.ComplementoResidencial = funcionario.ComplementoResidencial!.Trim();
                funcionarioDB.Bairro = funcionario.Bairro!.Trim();
                funcionarioDB.Estado = funcionario.Estado!.Trim();

                _bancoContext.Funcionario.Update(funcionarioDB);
                _bancoContext.SaveChanges();
                return funcionarioDB;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }
        public bool ValidarVinculoMotorista(CargoFuncionario cargoFuncionario, int funcionarioId) {
            if (cargoFuncionario != CargoFuncionario.Motorista) {
                if (_bancoContext.Contrato.Any(x => x.MotoristaId == funcionarioId && x.StatusContrato == ContratoStatus.Ativo)) {
                    return true;
                }
                return false;
            }
            return false;
        }

        public Funcionario AtivarFuncionario(int? id) {
            Funcionario funcionarioDB = GetFuncionarioById(id);
            funcionarioDB.Status = FuncionarioStatus.Ativo;
            _bancoContext.Funcionario.Update(funcionarioDB);
            _bancoContext.SaveChanges();
            return funcionarioDB;
        }

        public Funcionario InativarFuncionario(int? id) {
            Funcionario funcionarioDB = GetFuncionarioById(id);
            if (_bancoContext.Contrato.Any(x => x.MotoristaId == id && x.Andamento == Andamento.EmAndamento)) {
                throw new Exception("Motorista vinculado em contratos em andamento!");
            }
            funcionarioDB.Status = FuncionarioStatus.Inativo;
            if (funcionarioDB.Cargo != CargoFuncionario.Motorista) funcionarioDB.StatusUsuario = UsuarioStatus.Inativo;
            _bancoContext.Funcionario.Update(funcionarioDB);
            _bancoContext.SaveChanges();
            return funcionarioDB;
        }

        public Funcionario InativarUsuario(int? id) {
            Funcionario funcionarioDB = GetFuncionarioById(id);
            funcionarioDB.StatusUsuario = UsuarioStatus.Inativo;
            _bancoContext.Funcionario.Update(funcionarioDB);
            _bancoContext.SaveChanges();
            return funcionarioDB;
        }

        public Funcionario AtivarUsuario(int? id) {
            Funcionario funcionarioDB = GetFuncionarioById(id);
            if (funcionarioDB.Cargo == CargoFuncionario.Motorista) throw new Exception("Desculpe, ação inválida!");
            funcionarioDB.StatusUsuario = UsuarioStatus.Ativo;
            _bancoContext.Funcionario.Update(funcionarioDB);
            _bancoContext.SaveChanges();
            return funcionarioDB;
        }

        public List<Funcionario> PaginateListAtivos(int paginaAtual, bool statusPaginacao) {
            //Se o statusPaginação for igual a true, deve paginar para próxima página, caso contrário, página anterior.
            if (statusPaginacao) {
                int indiceInicial = (paginaAtual - 1) * 10;
                return _bancoContext.Funcionario.Where(x => x.Status == FuncionarioStatus.Ativo).OrderBy(x => x.Cargo == CargoFuncionario.Motorista)
                    .Skip(indiceInicial).Take(10).ToList();
            }
            if (paginaAtual < 2) {
                throw new Exception("Desculpe, ação inválida!");
            }
            int indice = (paginaAtual - 2) * 10;
            return _bancoContext.Funcionario.Where(x => x.Status == FuncionarioStatus.Ativo).OrderBy(x => x.Cargo == CargoFuncionario.Motorista)
                .Skip(indice).Take(10).ToList();
        }

        public List<Funcionario> PaginateListInativos(int paginaAtual, bool statusPaginacao) {
            //Se o statusPaginação for igual a true, deve paginar para próxima página, caso contrário, página anterior.
            if (statusPaginacao) {
                int indiceInicial = (paginaAtual - 1) * 10;
                return _bancoContext.Funcionario.Where(x => x.Status == FuncionarioStatus.Inativo).OrderBy(x => x.Cargo == CargoFuncionario.Motorista)
                    .Skip(indiceInicial).Take(10).ToList();
            }
            if (paginaAtual < 2) {
                throw new Exception("Desculpe, ação inválida!");
            }
            int indice = (paginaAtual - 2) * 10;
            return _bancoContext.Funcionario.Where(x => x.Status == FuncionarioStatus.Inativo).OrderBy(x => x.Cargo == CargoFuncionario.Motorista)
                .Skip(indice).Take(10).ToList();
        }

        public int QtPaginasAtivas() {
            var qtOnibus = _bancoContext.Funcionario.Count(x => x.Status == FuncionarioStatus.Ativo);
            //Arredonda o resultado para cima, caso  o mesmo seja flutuante.
            int qtPaginas = (int)Math.Ceiling((double)qtOnibus / 10);
            return qtPaginas;
        }

        public int QtPaginasInativas() {
            var qtOnibus = _bancoContext.Funcionario.Count(x => x.Status == FuncionarioStatus.Inativo);
            //Arredonda o resultado para cima, caso  o mesmo seja flutuante.
            int qtPaginas = (int)Math.Ceiling((double)qtOnibus / 10);
            return qtPaginas;
        }

        public List<Funcionario> GetAllMotoristas() {
            return _bancoContext.Funcionario.Where(x => x.Cargo == CargoFuncionario.Motorista && x.Status == FuncionarioStatus.Ativo).ToList();
        }

        public Funcionario TrimFuncionario(Funcionario value) {
            value.Name = value.Name!.Trim();
            value.Telefone = value.Telefone!.Trim();
            value.Cep = value.Cep!.Trim();
            value.Logradouro = value.Logradouro!.Trim();
            value.NumeroResidencial = value.NumeroResidencial!.Trim();
            value.ComplementoResidencial = value.ComplementoResidencial!.Trim();
            value.Ddd = value.Ddd!.Trim();
            value.Bairro = value.Bairro!.Trim();
            value.Cidade = value.Cidade!.Trim();
            value.Estado = value.Estado!.Trim();
            return value;
        }
        public bool Duplicata(Funcionario funcionario) {
            if (_bancoContext.Funcionario.Any(x => x.Cpf == funcionario.Cpf || x.Telefone == funcionario.Telefone || x.Email!.ToLower() == funcionario.Email!.ToLower())) {
                return true;
            }
            return false;
        }
        public bool DuplicataEditar(Funcionario funcionario, Funcionario funcionarioDB) {
            if (_bancoContext.Funcionario.Any(x => (x.Cpf == funcionario.Cpf && funcionario.Cpf != funcionarioDB.Cpf)
                || (x.Email!.ToLower() == funcionario.Email!.ToLower() && funcionario.Email!.ToLower() != funcionarioDB.Email!.ToLower())
                || (x.Telefone == funcionario.Telefone && funcionario.Telefone != funcionarioDB.Telefone))) {
                return true;
            }
            return false;
        }

        public bool EnviarSenha(Funcionario funcionario) {
            string tema = "Buses Control — Credencial para autenticação";
            string mensagem = $"Prezado {funcionario.Name}, <br><br>Gostaríamos de informar que uma senha foi gerada exclusivamente para você. " +
                "Por motivos de segurança, recomendamos que você mantenha essa informação confidencial. " +
                $"Segue abaixo a senha gerada:<br>Senha: <strong>{funcionario.Senha}</strong>" +
                "<br><br>Caso necessário, lembre-se de alterar essa senha periodicamente para garantir a proteção dos seus dados pessoais. " +
                "Caso tenha alguma dúvida ou precise de suporte adicional, não hesite em entrar em contato conosco.";
            return _email.EnviarEmail(funcionario.Email, tema, mensagem);
        }
    }
}
