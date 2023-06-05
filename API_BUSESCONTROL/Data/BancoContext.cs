using API_BUSESCONTROL.Models;
using Microsoft.EntityFrameworkCore;

namespace API_BUSESCONTROL.Data {
    public class BancoContext : DbContext {

        public BancoContext(DbContextOptions<BancoContext> options): base(options) {

        }

        public DbSet<Onibus> Onibus { get; set; }
        public DbSet<PaletaCores> PaletaCores { get; set; }
        public DbSet<Funcionario> Funcionario { get; set; }
        public DbSet<PessoaFisica> PessoaFisica { get; set; }
        public DbSet<PessoaJuridica> PessoaJuridica { get; set; }
        //Para o entity não criar duas tabelas (pessoaFisica e pessoaJuridica).
        public DbSet<Cliente> Cliente { get; set; }
    }
}
