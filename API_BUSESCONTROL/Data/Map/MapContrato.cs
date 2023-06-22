using API_BUSESCONTROL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_BUSESCONTROL.Data.Map {
    public class MapContrato : IEntityTypeConfiguration<Contrato> {
        public void Configure(EntityTypeBuilder<Contrato> builder) {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Onibus);
            builder.HasOne(x => x.Motorista);
        }
    }
}
