using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository.Interfaces {
    public interface IUsuarioService {
        public Funcionario ValidationCredenciasUser(Login login);
        public void EsqueceuSenha(EsqueceuSenha esqueceuSenha);
        public void ConsulteChaveRedefinition(string chaveSecreta);
        public void RedefinirSenha(RedefinirSenha redefinirSenha);
        public void AlterarSenha(AlterarSenha alterarSenha);
    }
}
