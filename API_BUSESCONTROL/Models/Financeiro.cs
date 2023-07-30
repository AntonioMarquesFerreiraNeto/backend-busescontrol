using API_BUSESCONTROL.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace API_BUSESCONTROL.Models {
    public class Financeiro {
        public int Id { get; set; }

        public int? ContratoId { get; set; }

        public int? PessoaJuridicaId { get; set; }

        public int? PessoaFisicaId { get; set; }

        public int? FornecedorId { get; set; }

        public virtual PessoaFisica? PessoaFisica { get; set; }

        public virtual PessoaJuridica? PessoaJuridica { get; set; }

        public virtual Fornecedor? Fornecedor { get; set; }

        public virtual List<Parcela>? Parcelas { get; set; }

        public virtual Contrato? Contrato { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataVencimento { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValorParcelaDR { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValorTotDR { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValorTotalPago { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? ValorTotTaxaJurosPaga { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataEmissao { get; set; }

        public int? QtParcelas { get; set; }

        public TypeEfetuacao TypeEfetuacao { get; set; }

        public DespesaReceita DespesaReceita { get; set; }

        public ModelPagament Pagament { get; set; }

        public FinanceiroStatus FinanceiroStatus { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(30, ErrorMessage = "Campo inválido!")]
        public string? Detalhamento { get; set; }


        public bool ValidarValorMonetario() {
            if (ValorTotDR < (decimal)150) {
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
            DateTime dataMaxima = DateTime.Now.Date.AddYears(4);
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

        public string ReturnValorMultaRescisao() {
            decimal? valorTotCliente = Contrato!.ValorParcelaContratoPorCliente * Contrato.QtParcelas;
            decimal valorMulta = (valorTotCliente!.Value * 3) / 100;
            return $"{valorMulta.ToString("C2")}";
        }

        public string ReturnTypePagament() {
            string msgPagament = (Pagament == ModelPagament.Avista) ? "À vista" : "Parcelado";
            return msgPagament;
        }

        public string ReturnNameClienteOrCredor() {
            if (!string.IsNullOrEmpty(ContratoId.ToString())) {
                return (!string.IsNullOrEmpty(PessoaFisicaId.ToString())) ? $"{PessoaFisica.Name}" : $"{PessoaJuridica.RazaoSocial}";
            }
            else {
                return $"{Fornecedor!.NameOrRazaoSocial}";
            }
        }

        public string ReturnTypeFinanceiro() {
            string type = (DespesaReceita == DespesaReceita.Receita) ? "Receita" : "Despesa";
            return type;
        }
        public string ReturnTypeEfetuacao() {
            if (TypeEfetuacao == TypeEfetuacao.Debito) {
                return $"Débito";
            }
            else if (TypeEfetuacao == TypeEfetuacao.Credito) {
                return $"Crédito";
            }
            else {
                return "Em espécie";
            }
        }
        public string ReturnStatusFinanceiro() {
            if (FinanceiroStatus == FinanceiroStatus.Ativo) return "Ativado";
            return "Inativo";
        }
        public string ReturnValorTot() {
            return $"{ValorTotDR!.Value.ToString("C2")}";
        }
        public string ReturnValorParcela() {
            return $"{ValorParcelaDR!.Value.ToString("C2")}";
        }
        public string ReturnValorTotEfetuado() {
            if (!string.IsNullOrEmpty(ValorTotalPago.ToString())) {
                return $"{ValorTotalPago!.Value.ToString("C2")}";
            }
            return "R$ 0,00";
        }
    }
}
