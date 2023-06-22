using API_BUSESCONTROL.Models.Enums;

namespace API_BUSESCONTROL.Models {
    public class ClientesContrato {
        public int Id { get; set; }
        public int? ContratoId { get; set; }
        public int? PessoaJuridicaId { get; set; }
        public int? PessoaFisicaId { get; set; }
        public virtual PessoaFisica? PessoaFisica { get; set; }
        public virtual PessoaJuridica? PessoaJuridica { get; set; }
        public virtual Contrato? Contrato { get; set; }

        public DateTime? DataEmissaoPdfRescisao { get; set; }
        public ProcessRescendir ProcessRescisao { get; set; }

        public ClientesContrato() { }
    }
}
