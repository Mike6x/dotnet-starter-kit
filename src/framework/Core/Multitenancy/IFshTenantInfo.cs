namespace FSH.Framework.Core.Multitenancy;
public interface IFshTenantInfo
{
    string? ConnectionString { get; set; }
}