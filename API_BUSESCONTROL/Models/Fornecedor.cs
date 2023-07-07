using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models.ValidationsModels.Pessoas;
using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class Fornecedor : Endereco {

        public int Id { get; set; }

        [ValidationEmail(ErrorMessage = "Campo inválido!")]
        [MinLength(5, ErrorMessage = "Campo inválido!")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Campo inválido!")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(9, ErrorMessage = "Campo inválido!")]
        [MaxLength(9, ErrorMessage = "Campo inválido!")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(5, ErrorMessage = "Campo inválido.")]
        public string? NameOrRazaoSocial { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [CpfValidation(ErrorMessage = "Campo inválido!")]
        public string? Cpf { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        public DateTime? DataFornecedor { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [CnpjValidation(ErrorMessage = "Campo inválido!")]
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
    }
}
