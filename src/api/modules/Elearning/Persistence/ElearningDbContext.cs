using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.Elearning.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FSH.Starter.WebApi.Elearning.Persistence;

public class ElearningDbContext : FshDbContext
{
    public ElearningDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<ElearningDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> elearnings)
        : base(multiTenantContextAccessor, options, publisher, elearnings)
    {
    }

    public DbSet<Quiz> Quizs { get; set; } = null!;
    public DbSet<QuizResult> QuizResults { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElearningDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.Elearning);
    }
}
