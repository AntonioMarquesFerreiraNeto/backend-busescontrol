using API_BUSESCONTROL.Helpers;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models.ValidationsModels;
using API_BUSESCONTROL.Models.ValidationsModels.Pessoas;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API_BUSESCONTROL.Models {
    public class Funcionario {

        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(5, ErrorMessage = "Campo inválido.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [CpfValidation(ErrorMessage = "Campo inválido!")]
        public string? Cpf { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [DataType(DataType.Date, ErrorMessage = "Campo inválido!")]
        [ValidarDataFuncionario(ErrorMessage = "Data de nascimento inválida!")]
        [Display(Name = "Data de nascimento")]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [ValidationEmail(ErrorMessage = "Campo inválido!")]
        [MinLength(5, ErrorMessage = "Campo inválido!")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Campo inválido!")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(8, ErrorMessage = "Campo inválido!")]
        [MaxLength(9, ErrorMessage = "Campo inválido!")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(8, ErrorMessage = "Campo inválido!")]
        [MaxLength(8, ErrorMessage = "Campo inválido!")]
        public string? Cep { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        public string? NumeroResidencial { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(3, ErrorMessage = "Campo inválido!")]
        public string? Logradouro { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(2, ErrorMessage = "Campo inválido!")]
        public string? ComplementoResidencial { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(2, ErrorMessage = "Campo inválido!")]
        public string? Bairro { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(3, ErrorMessage = "Campo inválido!")]
        public string? Cidade { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(2, ErrorMessage = "Campo inválido!")]
        public string? Estado { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(2, ErrorMessage = "Campo inválido!")]
        public string? Ddd { get; set; }

        public string? Apelido { get; set; }

        public string? Senha { get;  set; }

        public string? ChaveRedefinition { get; set; }

        public FuncionarioStatus Status { get; set; }

        public CargoFuncionario Cargo { get; set; }

        public UsuarioStatus StatusUsuario { get; set; }

        public virtual List<Contrato>? Contratos { get; set; }

        public bool ValidarSenha(string senha) {
            if (senha.GerarHash() == Senha) {
                return true;
            }
            else {
                return false;
            }
        }
        public bool ValidarDuplicataSenha(string newSenha) {
            bool result = (Senha == newSenha) ? true : false;
            return result;
        }
        public string GerarSenha() {

            Random random = new Random();

            int rdn = random.Next(2);
            int tamanhoSenha = (rdn == 0) ? 14 : 16;

            string caixaCaracteres = "ABCDEFGHIJKLNOPQIWYZK" + "ABCDEFGHIJKLNOPQIWYZK".ToLower() + "@#$%&*!" + "123456789";
            StringBuilder senhaUser = new StringBuilder();

            for (int cont = 0; cont < tamanhoSenha; cont++) {
                int indiceCaracter = random.Next(0, caixaCaracteres.Length - 1);
                senhaUser.Append(caixaCaracteres[indiceCaracter]);
            }
            return Convert.ToString(senhaUser)!;
        }

        public void setPasswordHash() {
            Senha = Senha.GerarHash();
        }

        public void SetNewPasswordHash(string senha) {
            Senha = senha.GerarHash();
        }

        public string ReturnTelefoneFuncionario() {
            return $"{long.Parse(Telefone).ToString(@"00000-0000")}";
        }
        public string ReturnCpfFuncionario() {
            return $"{Convert.ToUInt64(Cpf):000\\.000\\.000\\-00}";
        }

        public bool VerificarSenha(string senha) {
            return (senha.GerarHash() == Senha) ? true : false;
        }

        public void GerarChaveRedefinition() {
            Random random = new Random();
            string caixaChar = "5BWwaZ6YyB6lPG48pY411DWjgQzkfDcLbNZWOI385iwEYoFD6kLFqLl3ggh3j7olD4b";
            string chave = "";
            for (int c = 0; c < 60; c++) {
                int indiceChar = random.Next(0, caixaChar.Length - 1);
                chave += caixaChar[indiceChar];
            }
            ChaveRedefinition = chave;
        }
    }
}
