using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models.ValidationsModels.Contratos;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace API_BUSESCONTROL.Models {
    public class Contrato {

        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        public int? MotoristaId { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        public int? OnibusId { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [ValidarValorMonetario(ErrorMessage = "Campo inválido!")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValorMonetario { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValorParcelaContrato { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValorTotalPagoContrato { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValorParcelaContratoPorCliente { get; set; }



        [Required(ErrorMessage = "Campo obrigatório!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataEmissao { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataVencimento { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(30, ErrorMessage = "Campo inválido!")]
        public string? Detalhamento { get; set; }

        public int? QtParcelas { get; set; }

        public ModelPagament Pagament { get; set; }

        public ContratoStatus StatusContrato { get; set; }

        public StatusAprovacao Aprovacao { get; set; }

        public Funcionario? Motorista { get; set; }

        public Onibus? Onibus { get; set; }

        public Andamento Andamento { get; set; }

        public virtual List<ClientesContrato>? ClientesContrato { get; set; }

        //public virtual List<Financeiro> Financeiros { get; set; }

        //public virtual List<Rescisao> Rescisoes { get; set; }


        public void SetValoresParcelas(int qtClientes) {
            ValorParcelaContrato = ValorMonetario / QtParcelas;
            ValorParcelaContratoPorCliente = ValorParcelaContrato / qtClientes;
        }

        public bool ValidarValorMonetario() {
            if (ValorMonetario < (decimal)150) {
                return true;
            }
            return false;
        } 

        public bool ValidationDatas() {
            if (DataEmissao >= DataVencimento) {
                return true;
            }
            return false;
        }

        public bool ValidationDataEmissao() {
            if (DataEmissao!.Value.Date < DateTime.Now.Date || DataEmissao.Value.Date > DateTime.Now.Date) {
                return true;
            }
            return false; 
        }

        public bool ValidationDataVencimento() {
            DateTime dataMaxima = DateTime.Now.Date.AddYears(2);
            if (DataVencimento > dataMaxima) {
                return true;
            }
            return false;
        }

        public bool ValidationQtParcelas() {
            DateTime dateVencimento = DataVencimento!.Value;
            DateTime dataEmissao = DataEmissao!.Value;

            float dias = (float)dateVencimento.Subtract(dataEmissao).TotalDays;
            float ano = dias / 365;
            if (Pagament == ModelPagament.Parcelado) {
                bool resultado = (QtParcelas > ano * 12 || QtParcelas < 2 || string.IsNullOrEmpty(QtParcelas.ToString())) ? true : false;
                return resultado;
            }
            else {
                bool resultado = (QtParcelas < 1 || string.IsNullOrEmpty(QtParcelas.ToString())) ? true : false;
                return resultado;
            }
        }
    }
}
