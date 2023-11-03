namespace API_BUSESCONTROL.Models.Enums {
    
    //Ônibus
    public enum StatusFrota : int {
        Ativo = 0,
        Inativo = 1
    }
    public enum Disponibilidade : int {
        Disponivel = 0,
        Indisponivel = 1
    }

    //Funcionário
    public enum FuncionarioStatus : int {
        Ativo = 0,
        Inativo = 1
    }
    public enum UsuarioStatus : int {
        Inativo = 0,
        Ativo = 1
    }
    public enum CargoFuncionario : int {
        Motorista = 0,
        Assistente = 1,
        Administrador = 2
    }
    
    //Cliente
    public enum ClienteStatus : int {
        Ativo = 0,
        Inativo = 1
    }
    public enum Adimplencia : int {
        Adimplente = 0,
        Inadimplente = 1,
    }

    //Contrato
    public enum StatusAprovacao : int {
        EmAnalise = 0,
        Negado = 1,
        Aprovado = 2,
    }
    public enum ModelPagament : int {
        Parcelado = 0,
        Avista = 1,
    }
    public enum ContratoStatus : int {
        Ativo = 0,
        Inativo = 1,
    }
    public enum Andamento : int {
        Aguardando = 0,
        EmAndamento = 1,
        Encerrado = 2
    }

    //Clientes contrato 
    public enum ProcessRescendir : int {
        NoRescisao = 0,
        PdfBaixado = 1,
    }

    //Fornecedor 
    public enum TypePessoa : int {
        PessoaFisica = 0,
        PessoaJuridica = 1
    }
    public enum FornecedorStatus : int {
        Ativo = 0,
        Inativo = 1
    }

    //Financeiro
    public enum TypeEfetuacao : int {
        Credito = 0,
        Especie = 1,
        Debito = 2
    }
    public enum DespesaReceita : int {
        Despesa = 0,
        Receita = 1
    }
    public enum FinanceiroStatus : int {
        Inativo = 0,
        Ativo = 1,
    }

    //Parcela
    public enum SituacaoPagamento : int {
        AguardandoPagamento = 0,
        PagamentoContabilizado = 1,
        Atrasada = 2
    }

    public enum FiltroFinanceiro : int {
        Atrasadas = 2, 
        Despesa = 0,
        Receita = 1,
        Contrato = 3,
        Todos = 4
    }

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
