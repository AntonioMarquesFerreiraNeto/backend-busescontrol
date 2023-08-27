using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class RedefinirSenha {

        [Required]
        public string ChaveRedefinition { get; set; }

        [Required (ErrorMessage = "Obrigatório!")]
        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "Obrigatório!")]
        public string ConfirmarSenha { get; set; }
    }
}
