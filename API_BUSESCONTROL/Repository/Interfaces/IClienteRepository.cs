using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository.Interfaces
{
    public interface IClienteRepository
    {
        //Services dos clientes PF.
        public PessoaFisica CreateCliente(PessoaFisica cliente);
        public PessoaFisica UpdateCliente(PessoaFisica cliente);
        public PessoaFisica InativarCliente(int? id);
        public PessoaFisica AtivarCliente(int? id);
        public PessoaFisica GetClienteFisicoById(int? id);
        public List<PessoaFisica> GetClientesAtivos(int paginaAtual, bool statusPagina);
        public List<PessoaFisica> GetClientesInativos(int paginaAtual, bool statusPagina);
        public List<PessoaFisica> GetClientesParaVinculacao();
        public List<PessoaFisica> GetClientesAdimplentes();
        public int QtPaginasClientesAtivos();
        public int QtPaginasClientesInativos();

        //Services dos clientes PJ.
        public PessoaJuridica CreateClientePJ(PessoaJuridica cliente);
        public PessoaJuridica UpdateClientePJ(PessoaJuridica cliente);
        public PessoaJuridica InativarClientePJ(int? id);
        public PessoaJuridica AtivarClientePJ(int? id);
        public PessoaJuridica GetClienteByIdPJ(int? id);
        public List<PessoaJuridica> GetClientesAtivosPJ(int paginaAtual, bool statusPagina);
        public List<PessoaJuridica> GetClientesInativosPJ(int paginaAtual, bool statusPagina);
        public List<PessoaJuridica> GetClientesParaVinculacaoPJ();
        public int QtPaginasClientesAtivosPJ();
        public int QtPaginasClientesInativosPJ();
    }
}
