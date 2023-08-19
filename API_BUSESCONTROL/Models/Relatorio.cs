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

        public string ReturnPercentualContrato(int? qtPercentual) {
            float percentual = float.Parse(qtPercentual.ToString());
            float result = (percentual / (int)QtContratos) * 100;
            return $"{result.ToString("F2")}%";
        }
        public string ReturnPercentualValorContrato(decimal? value) {
            if (ValTotAprovados != 0) {
                decimal? result = value / ValTotAprovados * 100;
                return $"{result.Value.ToString("F2")}%";
            }
            return $"";
        }
        public string ReturnPercentualCliente(int? qtPercentual) {
            float percentual = float.Parse(qtPercentual.ToString());
            float result = (percentual / (int)QtClientes) * 100;
            return $"{result.ToString("F2")}%";
        }
        public string ReturnPercentualMotorista(int? qtPercentual) {
            float percentual = float.Parse(qtPercentual.ToString());
            float result = (percentual / (int)QtMotorista) * 100;
            return $"{result.ToString("F2")}%";
        }
        public string ReturnPercentualOnibus(int? qtPercentual) {
            float percentual = float.Parse(qtPercentual.ToString());
            float result = (percentual / (int)QtOnibus) * 100;
            return $"{result.ToString("F2")}%";
        }

    }
}
