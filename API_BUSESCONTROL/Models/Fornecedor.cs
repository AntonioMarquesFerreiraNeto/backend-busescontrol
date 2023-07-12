using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models.ValidationsModels.Pessoas;
using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class Fornecedor : Endereco {

        public int Id { get; set; }

        [ValidationEmail(ErrorMessage = "Campo inválido!")]
        [MinLength(5, ErrorMessage = "Campo inválido!")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(9, ErrorMessage = "Campo inválido!")]
        [MaxLength(9, ErrorMessage = "Campo inválido!")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(5, ErrorMessage = "Campo inválido.")]
        public string? NameOrRazaoSocial { get; set; }

        public string? Cpf { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        public DateTime? DataFornecedor { get; set; }

        public string? Cnpj { get; set; }

        public TypePessoa TypePessoa { get; set; }

        public FornecedorStatus Status { get; set; }

        public bool ValidationDate() {

            DateTime dataAtual = DateTime.Now.Date;
            long dias = (int)dataAtual.Subtract(DataFornecedor!.Value).TotalDays;
            int totAnos = (int)dias / 365;
            if (TypePessoa == TypePessoa.PessoaJuridica) {
                if (totAnos <= 0 || totAnos > 250) {
                    return true;
                }
                return false;
            }
            else {
                if (totAnos < 18 || totAnos > 132) {
                    return true;
                }
                return false;
            }
        }

        public void TrimFornecedor() {
            NameOrRazaoSocial = NameOrRazaoSocial!.Trim();
            Cep = Cep!.Trim();
            Logradouro = Logradouro!.Trim();
            NumeroResidencial = NumeroResidencial!.Trim();
            Logradouro = Logradouro!.Trim();
            Bairro = Bairro!.Trim();
            Cidade = Cidade!.Trim();
            Estado = Estado!.Trim();
        }

        public string ReturnTelefoneFornecedor() {
            string tel = Telefone;
            return $"{long.Parse(tel).ToString(@"00000-0000")}";
        }

        public bool ValidarCpf(string value) {

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            value = value.Trim();
            value = value.Replace(".", "").Replace("-", "");
            if (value.Length != 11)
                return false;
            tempCpf = value.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return value.EndsWith(digito);
        }

        public bool ValidaCNPJ(string cnpj) {
           
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14) {
                return false;
            }
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++) {
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            }
            resto = (soma % 11);
            if (resto < 2) {
                resto = 0;
            }
            else {
                resto = 11 - resto;
            }
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++) {
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            }
            resto = (soma % 11);
            if (resto < 2) {
                resto = 0;
            }
            else {
                resto = 11 - resto;
            }
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }
    }
}
