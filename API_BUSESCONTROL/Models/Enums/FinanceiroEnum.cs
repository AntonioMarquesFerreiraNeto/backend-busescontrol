using System.ComponentModel;

namespace API_BUSESCONTROL.Models.Enums {

    public enum TypeEfetuacao : int {
        [Description("Crédito")]
        Credito = 0,
        [Description("Espécie")]
        Especie = 1,
        [Description("Débito")]
        Debito = 2
    }
    public enum DespesaReceita : int {
        [Description("Despesa")]
        Despesa = 0,
        [Description("Receita")]
        Receita = 1
    }
    public enum FinanceiroStatus : int {
        [Description("Inativa")]
        Inativo = 0,
        [Description("Ativada")]
        Ativo = 1,
    }

    //Parcela
    public enum SituacaoPagamento : int {
        [Description("Em espera")]
        AguardandoPagamento = 0,
        [Description("Efetuada")]
        PagamentoContabilizado = 1,
        [Description("Atrasada")]
        Atrasada = 2
    }

    //Filtro financeiro
    public enum FiltroFinanceiro : int {
        Atrasadas = 2,
        Despesa = 0,
        Receita = 1,
        Contrato = 3,
        Todos = 4
    }
}
