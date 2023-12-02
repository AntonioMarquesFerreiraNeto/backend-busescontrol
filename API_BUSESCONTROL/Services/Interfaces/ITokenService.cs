using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Services.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(Funcionario funcionario);
    }
}
