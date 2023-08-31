using API_BUSESCONTROL.Models.ValidationsModels.Pessoas;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace API_BUSESCONTROL.Models {
    public class AlterarSenha {

        [Required(ErrorMessage = "Obrigatório!")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Obrigatório!")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Obrigatório!")]
        [ValidationSenha(ErrorMessage = "Senha fraca!")]
        public string NewSenha { get; set; }

        [Required(ErrorMessage = "Obrigatório!")]
        public string ConfirmSenha { get; set; }
    }
}
