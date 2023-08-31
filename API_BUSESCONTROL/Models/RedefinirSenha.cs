using API_BUSESCONTROL.Models.ValidationsModels.Pessoas;
using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class RedefinirSenha {

        [Required]
        public string ChaveRedefinition { get; set; }

        [Required (ErrorMessage = "Obrigatório!")]
        [ValidationSenha(ErrorMessage = "Senha fraca!")]
        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "Obrigatório!")]
        public string ConfirmarSenha { get; set; }
    }
}
