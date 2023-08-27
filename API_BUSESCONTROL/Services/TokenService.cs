using API_BUSESCONTROL.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_BUSESCONTROL.Services {
    public class TokenService : ITokenService {

        public string GenerateToken(Funcionario funcionario) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secrect);
            var tokenDescricao = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, funcionario.Id.ToString()),
                    new Claim(ClaimTypes.Name, funcionario.Name),
                    new Claim(ClaimTypes.Role, funcionario.Cargo.ToString()),
                    new Claim(ClaimTypes.Email, funcionario.Email),
                    new Claim(ClaimTypes.DateOfBirth, funcionario.DataNascimento!.Value.ToString("dd/MM/yyyy"))
                }),
                Expires = DateTime.UtcNow.AddSeconds(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescricao);
            return tokenHandler.WriteToken(token);
        }
    }
}
