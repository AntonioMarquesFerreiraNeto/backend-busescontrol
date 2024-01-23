using API_BUSESCONTROL.Data.Map;
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
        public DbSet<Fornecedor> Fornecedor { get; set; }

        //Para o entity não criar duas tabelas (pessoaFisica e pessoaJuridica).
        public DbSet<Cliente> Cliente { get; set; }

        public DbSet<Contrato> Contrato { get; set; }
        public DbSet<Financeiro> Financeiro { get; set; }
        public DbSet<Parcela> Parcela { get; set; }
        public DbSet<Rescisao> Rescisao { get; set; }
        public DbSet<ClientesContrato> ClientesContrato { get; set; }
        public DbSet<SubContratoMotorista> SubContratoMotorista { get; set; }
        public DbSet<SubContratoOnibus> SubContratoOnibus { get; set; }
        public DbSet<Lembrete> Lembrete { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            //Adicionando os mapeamento do sistema no contexto. 
            modelBuilder.ApplyConfiguration(new MapContrato());
            modelBuilder.ApplyConfiguration(new MapClientesContrato());
            modelBuilder.ApplyConfiguration(new MapFinanceiro());
            modelBuilder.ApplyConfiguration(new MapRescisao());

            //Mapeamento dos relacionamentos de lembretes.
            modelBuilder.Entity<Funcionario>()
               .HasMany(funci => funci.Lembretes)
               .WithOne(lembrete => lembrete.Funcionario)
               .HasForeignKey(lembrete => lembrete.FuncionarioId);
            modelBuilder.Entity<Funcionario>()
                .HasMany(funci => funci.LembretesEnviados)
                .WithOne(x => x.Remetente)
                .HasForeignKey(x => x.RemetenteId);

            base.OnModelCreating(modelBuilder);            
        }
    }
}
