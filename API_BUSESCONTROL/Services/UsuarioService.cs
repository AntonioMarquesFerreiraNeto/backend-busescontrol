using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Helpers;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;

namespace API_BUSESCONTROL.Services
{
    public class UsuarioService : IUsuarioService
    {

        private readonly BancoContext _bancoContext;
        private readonly IEmail _email;

        public UsuarioService(BancoContext bancoContext, IEmail email)
        {
            _bancoContext = bancoContext;
            _email = email;
        }

        public Funcionario ValidationCredenciasUser(Login login)
        {
            Funcionario usuario = _bancoContext.Funcionario.FirstOrDefault(x => x.Cpf == login.Cpf && x.StatusUsuario == UsuarioStatus.Ativo) ?? throw new Exception("Senha ou CPF informados podem estar incorretos!");
            if (!usuario.VerificarSenha(login.Senha)) throw new Exception("Senha ou CPF informados podem estar incorretos!");
            return usuario;
        }

        public void EsqueceuSenha(EsqueceuSenha esqueceuSenha)
        {
            Funcionario funcionario = _bancoContext.Funcionario.FirstOrDefault(x => x.Cpf == esqueceuSenha.Cpf && x.Email == esqueceuSenha.Email && x.DataNascimento!.Value.Date == esqueceuSenha.DataNascimento!.Value.Date && x.StatusUsuario == UsuarioStatus.Ativo) ?? throw new Exception("CPF, e-mail ou data de nascimento podem estar inválidos!");
            if (!RecuperarSenhaMail(funcionario)) throw new Exception("Desculpe, não conseguimos enviar o e-mail. Consulte nosso suporte para mais informações.");
        }

        public bool RecuperarSenhaMail(Funcionario funcionario)
        {
            string tema = "Buses Control - Recuperação de senha";
            string urlRecuperacao = $"<a href='https://buscontrol.netlify.app/redefinirSenha/{funcionario.ChaveRedefinition}'>Clique aqui para recuperar sua conta</a>";
            string conteudo = $"<p>Olá, {funcionario.Name}. Você acessou nosso serviço de recuperação de conta recentemente. Portanto, acesse o link a seguir para redefinir sua senha: {urlRecuperacao}</p>";
            return _email.EnviarEmail(funcionario.Email, tema, conteudo);
        }

        public void ConsulteChaveRedefinition(string chaveSecreta)
        {
            Funcionario funcionario = _bancoContext.Funcionario.FirstOrDefault(x => x.ChaveRedefinition == chaveSecreta && x.StatusUsuario == UsuarioStatus.Ativo) ?? throw new Exception("O acesso a esta URL está estritamente reservado para usuários autorizados. Tentativas de acesso não autorizado são registrados!");
        }

        public void RedefinirSenha(RedefinirSenha redefinirSenha)
        {
            Funcionario funcionario = _bancoContext.Funcionario.FirstOrDefault(x => x.ChaveRedefinition == redefinirSenha.ChaveRedefinition && x.StatusUsuario == UsuarioStatus.Ativo) ?? throw new Exception("O acesso a esta URL está estritamente reservado para usuários autorizados. Tentativas de acesso não autorizado são registrados!");
            funcionario.SetNewPasswordHash(redefinirSenha.NovaSenha);
            _bancoContext.Funcionario.Update(funcionario);
            _bancoContext.SaveChanges();
        }

        public void AlterarSenha(AlterarSenha alterarSenha)
        {
            if (alterarSenha.SenhaAtual == alterarSenha.NewSenha) throw new Exception("A nova senha não pode ser igual a atual!");
            else if (alterarSenha.NewSenha != alterarSenha.ConfirmSenha) throw new Exception("Nova senha diferente de confirmar senha!");
            Funcionario funcionario = _bancoContext.Funcionario.FirstOrDefault(x => x.Id == alterarSenha.UsuarioId) ?? throw new Exception("Desculpe, registro não encontrado!");
            if (alterarSenha.SenhaAtual.GerarHash() != funcionario.Senha) throw new Exception("Senha atual fornecida não corresponde à senha em nossa base de dados!");
            funcionario.SetNewPasswordHash(alterarSenha.NewSenha);
            _bancoContext.Funcionario.Update(funcionario);
            _bancoContext.SaveChanges();
        }
    }
}
