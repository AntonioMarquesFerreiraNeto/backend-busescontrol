using System.ComponentModel;

namespace API_BUSESCONTROL.Models.Enums {
    
    public enum StatusAprovacao : int {
        [Description("Em análise")]
        EmAnalise = 0,
        [Description("Reprovado")]
        Negado = 1,
        [Description("Aprovado")]
        Aprovado = 2,
    }
    public enum ModelPagament : int {
        [Description("Parcelado")]
        Parcelado = 0,
        [Description("À vista")]
        Avista = 1,
    }
    public enum ContratoStatus : int {
        [Description("Ativo")]
        Ativo = 0,
        [Description("Inativo")]
        Inativo = 1,
    }
    public enum Andamento : int {
        [Description("Em tramitação")]
        Aguardando = 0,
        [Description("Em andamento")]
        EmAndamento = 1,
        [Description("Encerrado")]
        Encerrado = 2
    }

    //Clientes contrato 
    public enum ProcessRescendir : int {
        NoRescisao = 0,
        PdfBaixado = 1,
    }

    //Filtro contrato
    public enum FiltroContrato : int {
        EmAnalise = 0,
        Negado = 1,
        Aprovado = 2,
        Aguardando = 3,
        EmAndamento = 4,
        Encerrado = 5,
        Todos = 6
    }
}
