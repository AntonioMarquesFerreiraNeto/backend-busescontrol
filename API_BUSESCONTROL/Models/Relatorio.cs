using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models {
    public class Relatorio {

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValTotAprovados { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValTotEmAnalise { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValTotContratos { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValTotPago { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValTotPendente { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValTotReceitas { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValTotDespesas { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValTotEfetuadoReceita { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValTotEfetuadoDespesa { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValTotReprovados { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValorReceitasComuns { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValorJurosAndMultas { get; set;}

        public int? QtContratosEncerrados { get; set; }
        public int? QtContratosAprovados { get; set; }
        public int? QtContratosEmAnalise { get; set; }
        public int? QtContratosNegados { get; set; }
        public int? QtContratos { get; set; }
        public int? QtClientesAdimplente { get; set; }
        public int? QtClientesInadimplente { get; set; }
        public int? QtClientesVinculados { get; set; }
        public int? QtClientes { get; set; }
        public int? QtMotorista { get; set; }
        public int? QtMotoristaVinculado { get; set; }
        public int? QtOnibus { get; set; }
        public int? QtOnibusVinculado { get; set; }
        
        public SimpleAnalytics? SimpleAnalytics { get; set; }

        public decimal? ReturnValorPendente() {
            return ValTotContratos - ValTotPago;
        }
    }
}
