using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class AlterarSenha {

        [Required(ErrorMessage = "Obrigatório!")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Obrigatório!")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Obrigatório!")]
        public string NewSenha { get; set; }

        [Required(ErrorMessage = "Obrigatório!")]
        public string ConfirmSenha { get; set; }
    }
}
