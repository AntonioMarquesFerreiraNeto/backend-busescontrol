using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models.ValidationsModels.Pessoas;
using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class PessoaJuridica : Cliente {

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(2, ErrorMessage = "Campo inválido!")]
        public string? NomeFantasia { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [CnpjValidation(ErrorMessage = "Campo inválido!")]
        public string? Cnpj { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(3, ErrorMessage = "Campo inválido!")]
        public string? RazaoSocial { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(9, ErrorMessage = "Campo inválido!")]
        public string? InscricaoEstadual { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(11, ErrorMessage = "Campo inválido!")]
        public string? InscricaoMunicipal { get; set; }

        //public virtual List<Rescisao> Rescisoes { get; set; }

        public ClienteStatus Status { get; set; }

        public string ReturnCnpjCliente() {
            return $"{Convert.ToUInt64(Cnpj): 00\\.000\\.000\\/0000-00}";
        }
    }
}
