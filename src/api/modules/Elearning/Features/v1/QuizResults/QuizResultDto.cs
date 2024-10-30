namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;

public record QuizResultDto(
    Guid Id,
    DateTime StartTime,
    DateTime EndTime,
    
    string? SId,
    decimal Sp,
    decimal Ut,
    string Fut, 
    
    string Qt, 
    decimal Tp,
    decimal Ps,
    decimal Psp,
    decimal Tl,

    string V, 
    string T, 
    
    bool IsPass,
    decimal? Rating,

    Guid QuizId,
    string QuizCode,
    string QuizName
);

public record QuizResultExportDto(
    Guid Id,
    DateTime StartTime,
    DateTime EndTime,
    
    string? SId,
    decimal Sp,
    decimal Ut,
    string Fut, 
    
    string Qt, 
    decimal Tp,
    decimal Ps,
    decimal Psp,
    decimal Tl,

    string V, 
    string T, 

    bool IsPass,
    decimal? Rating,
    
    Guid QuizId,
    string QuizCode,
    string QuizName,
    
    Guid CreatedBy,
    DateTimeOffset Created,
    Guid? LastModifiedBy,
    DateTimeOffset? LastModified
);


public record GetQuizResultResponse(
    Guid Id,
    DateTime StartTime,
    DateTime EndTime,
    
    string? SId,
    decimal Sp,
    decimal Ut,
    string Fut, 
    
    string Qt, 
    decimal Tp,
    decimal Ps,
    decimal Psp,
    decimal Tl,

    string V, 
    string T, 

    bool IsPass,
    decimal? Rating,
    Guid QuizId
);
