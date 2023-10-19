using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class SubContratoOnibus {
        public int Id { get; set; }
        [Required]
        public int? ContratoId { get; set; }
        [Required]
        public int? OnibusId { get; set; }
        [Required]
        public DateTime? DataInicial { get; set; }
        [Required]
        public DateTime? DataFinal { get; set; }

        public Contrato? Contrato { get; set; }
        public Onibus? Onibus { get; set; }

        public bool ValidarDates() {
            return (DataInicial!.Value.Date > DataFinal!.Value.Date) ? true : false;
        }
    }
}
