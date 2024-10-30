﻿namespace FSH.Starter.WebApi.Setting.Features.v1.Dimensions;
public record DimensionDto(
    Guid Id,
    int? Order,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? FullName,
    string? NativeName,
    string? FullNativeName,
    int? Value,
    string Type,
    Guid? FatherId,
    string? FatherName);

public record DimensionExportDto(
    Guid Id,
    int? Order,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? FullName,
    string? NativeName,
    string? FullNativeName,
    int? Value,
    string Type,
    Guid? FatherId,
    string? FatherName,
    
    Guid CreatedBy,
    DateTimeOffset Created,
    Guid? LastModifiedBy,
    DateTimeOffset? LastModified
    );

public record GetDimensionResponse(
    Guid Id,
    int? Order,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? FullName,
    string? NativeName,
    string? FullNativeName,
    int? Value,
    string Type,
    Guid? FatherId);