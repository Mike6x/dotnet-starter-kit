using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Elearning.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Elearning.Persistence.Configurations;
internal sealed class QuizResultConfiguration : IEntityTypeConfiguration<QuizResult>
{
    public void Configure(EntityTypeBuilder<QuizResult> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        
        // builder.Property(x => x.Code).HasMaxLength(100)
        // builder.Property(x => x.Name).HasMaxLength(100)
    }
}
