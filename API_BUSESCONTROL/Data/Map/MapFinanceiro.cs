using API_BUSESCONTROL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_BUSESCONTROL.Data.Map {
    public class MapFinanceiro : IEntityTypeConfiguration<Parcela> {
        public void Configure(EntityTypeBuilder<Parcela> builder) {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Financeiro);
        }
    }
}
