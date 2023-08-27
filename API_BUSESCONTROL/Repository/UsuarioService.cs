using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;

namespace API_BUSESCONTROL.Repository {
    public class UsuarioService : IUsuarioService {

        private readonly BancoContext _bancoContext;

        public UsuarioService(BancoContext bancoContext) {
            _bancoContext = bancoContext;
        }

        public Funcionario ValidationCredenciasUser(Login login) {
            Funcionario usuario = _bancoContext.Funcionario.FirstOrDefault(x => x.Cpf == login.Cpf && x.StatusUsuario == UsuarioStatus.Ativo) ?? throw new Exception("Senha ou CPF informados podem estar incorretos!");
            if (!usuario.VerificarSenha(login.Senha)) throw new Exception("Senha ou CPF informados podem estar incorretos!");
            return usuario;
        }
    }
}
