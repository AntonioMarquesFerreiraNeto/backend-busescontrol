using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class Login {

        [Required(ErrorMessage = "Obrigatório!")]
        public string Cpf { get; set; }

        [Required (ErrorMessage = "Obrigatório!")]
        public string Senha { get; set; }
    }
}
