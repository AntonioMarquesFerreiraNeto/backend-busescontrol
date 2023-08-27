using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository.Interfaces {
    public interface IUsuarioService {
        public Funcionario ValidationCredenciasUser(Login login);
    }
}
