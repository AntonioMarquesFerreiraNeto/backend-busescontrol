using API_BUSESCONTROL.Models;
using Microsoft.EntityFrameworkCore;

namespace API_BUSESCONTROL.Data {
    public class BancoContext : DbContext {

        public BancoContext(DbContextOptions<BancoContext> options): base(options) {

        }

        public DbSet<Onibus> Onibus { get; set; }
        public DbSet<PaletaCores> PaletaCores { get; set; }
        public DbSet<Funcionario> Funcionario { get; set; }
    }
}
