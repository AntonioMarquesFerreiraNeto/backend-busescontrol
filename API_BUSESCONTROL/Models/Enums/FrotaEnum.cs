using System.ComponentModel;

namespace API_BUSESCONTROL.Models.Enums {
    
    public enum StatusFrota : int {
        [Description("Ativo")]
        Ativo = 0,
        [Description("Inativo")]
        Inativo = 1
    }
    public enum Disponibilidade : int {
        [Description("Disponível")]
        Disponivel = 0,
        [Description("Indisponível")]
        Indisponivel = 1
    }
}
