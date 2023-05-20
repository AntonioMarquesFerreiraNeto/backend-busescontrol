using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class PaletaCores {

        public int id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        public string? Cor { get; set; }
    }
}
