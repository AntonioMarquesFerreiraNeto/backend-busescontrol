using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class Rescisao {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? Multa { get; set; }

        [DisplayFormat(DataFormatString = "0:N2", ApplyFormatInEditMode = true)]
        public decimal? ValorPagoContrato { get; set; }

        public int? ContratoId { get; set; }

        public int? PessoaFisicaId { get; set; }

        public int? PessoaJuridicaId { get; set; }

        public Contrato? Contrato { get; set; }

        public PessoaFisica? PessoaFisica { get; set; }

        public PessoaJuridica? PessoaJuridica { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataRescisao { get; set; }

        public void CalcularMultaContrato() {
            decimal? valorPorCliente = Contrato!.ValorParcelaContratoPorCliente * Contrato.QtParcelas;
            Multa = (valorPorCliente * 3) / 100;
        }
    }
}
