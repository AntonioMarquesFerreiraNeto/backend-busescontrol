using System.ComponentModel;

namespace API_BUSESCONTROL.Models.Enums {
    
    //Funcionário
    public enum FuncionarioStatus : int {
        [Description("Ativo")]
        Ativo = 0,
        [Description("Inativo")]
        Inativo = 1
    }
    public enum UsuarioStatus : int {
        [Description("Inativo")]
        Inativo = 0,
        [Description("Ativo")]
        Ativo = 1
    }
    public enum CargoFuncionario : int {
        [Description("Motorista")]
        Motorista = 0,
        [Description("Assistente")]
        Assistente = 1,
        [Description("Administrador")]
        Administrador = 2
    }

    //Cliente
    public enum ClienteStatus : int {
        [Description("Ativo")]
        Ativo = 0,
        [Description("Inativo")]
        Inativo = 1
    }
    public enum Adimplencia : int {
        [Description("Adimplente")]
        Adimplente = 0,
        [Description("Inadimplente")]
        Inadimplente = 1,
    }

    //Fornecedor 
    public enum TypePessoa : int {
        [Description("Pessoa física")]
        PessoaFisica = 0,
        [Description("Pessoa jurídica")]
        PessoaJuridica = 1
    }
    public enum FornecedorStatus : int {
        [Description("Ativo")]
        Ativo = 0,
        [Description("Inativo")]
        Inativo = 1
    }
}
