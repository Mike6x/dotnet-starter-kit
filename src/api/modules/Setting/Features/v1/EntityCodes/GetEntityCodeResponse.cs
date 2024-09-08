﻿using FSH.Starter.WebApi.Setting.Domain;

namespace FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;
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