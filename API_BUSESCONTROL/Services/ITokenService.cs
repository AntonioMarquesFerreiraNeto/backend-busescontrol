using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Services {
    public interface ITokenService {
        public string GenerateToken(Funcionario funcionario);
    }
}
