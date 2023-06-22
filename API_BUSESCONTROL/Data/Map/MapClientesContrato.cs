using API_BUSESCONTROL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_BUSESCONTROL.Data.Map {
    public class MapClientesContrato : IEntityTypeConfiguration<ClientesContrato> {
        public void Configure(EntityTypeBuilder<ClientesContrato> builder) {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.PessoaFisica)
                .WithMany(x => x.ClientesContrato) 
                .HasForeignKey(x => x.PessoaFisicaId);

            builder.HasOne(x => x.PessoaJuridica)
                .WithMany(x => x.ClientesContrato)
                .HasForeignKey(x => x.PessoaJuridicaId);

            builder.HasOne(x => x.Contrato)
                .WithMany(x => x.ClientesContrato) 
                .HasForeignKey(x => x.ContratoId);
        }
    }
}
