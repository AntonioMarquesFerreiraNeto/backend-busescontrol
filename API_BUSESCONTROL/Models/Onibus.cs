using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models.ValidationsModels.Frota;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace API_BUSESCONTROL.Models {
    public class Onibus {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(3, ErrorMessage = "Campo inválido.")]
        public string? Marca { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(3, ErrorMessage = "Campo inválido.")]
        public string? NameBus { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [ValidarAnoFab(ErrorMessage = "Campo inválido!")]
        [MinLength(4, ErrorMessage = "Campo inválido!")]
        public string? DataFabricacao { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(8, ErrorMessage = "Campo inválido.")]
        public string? Renavam { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(7, ErrorMessage = "Campo inválido.")]
        public string? Placa { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(17, ErrorMessage = "Campo inválido.")]
        public string? Chassi { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [ValidarAssentos(ErrorMessage = "Campo inválido!")]
        public string? Assentos { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        public string? CorBus { get; set; }

        public StatusFrota StatusOnibus { get; set; }

        public Disponibilidade Disponibilidade { get; set; }

        public virtual List<Contrato>? Contratos { get; set; }

        public virtual List<SubContratoOnibus>? SubContratoOnibus { get; set; }  

        public string ReturnPlacaFormatada() {
            var letras = Regex.Replace(Placa, "[0-9]", "");
            var numbers = Regex.Replace(Placa, "[^0-9]", "");
            return $"{letras}-{numbers}";
        }
    }
}
