using API_BUSESCONTROL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_BUSESCONTROL.Data.Map {
    public class MapLembrete : IEntityTypeConfiguration<Lembrete> {
        public void Configure(EntityTypeBuilder<Lembrete> builder) {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Funcionario);
        }
    }
}
