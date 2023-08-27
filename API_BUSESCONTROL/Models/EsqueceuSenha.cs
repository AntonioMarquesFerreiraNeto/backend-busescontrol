using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class EsqueceuSenha {

        [Required(ErrorMessage = "Obrigatório!")]
        public string? Cpf { get; set; }

        [Required(ErrorMessage = "Obrigatório!")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Obrigatório!")]
        public DateTime? DataNascimento { get; set; }
    }
}
