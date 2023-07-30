using API_BUSESCONTROL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_BUSESCONTROL.Data.Map {
    public class MapRescisao : IEntityTypeConfiguration<Rescisao> {
        public void Configure(EntityTypeBuilder<Rescisao> builder) {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Contrato);
            builder.HasOne(x => x.PessoaFisica);
            builder.HasOne(x => x.PessoaJuridica);
        }
    }
}
