using FSH.Starter.WebApi.Setting.Domain;

namespace FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;
public record EntityCodeDto(
    Guid Id,
    int? Order,    
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? Separator,
    int? Value,
    CodeType Type);

public record EntityCodeExpDto(
    Guid Id,
    int? Order,    
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? Separator,
    int? Value,
    CodeType Type,

    Guid CreatedBy,
    DateTimeOffset Created,
    Guid? LastModifiedBy,
    DateTimeOffset? LastModified
    );

public class EntityCodeExportDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Separator { get; set; } = default!;
    public int? Value { get; set; }

    public CodeType Type { get; set; }
    public int TypeId => (int) Type!;
    
    public bool IsActive { get; set; }

    public Guid CreatedBy { get; set; }
    public DateTimeOffset Created { get; set; }
    public Guid LastModifiedBy { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}

public record GetEntityCodeResponse(
    Guid Id,
    int? Order,    
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? Separator,
    int? Value,
    CodeType Type);
