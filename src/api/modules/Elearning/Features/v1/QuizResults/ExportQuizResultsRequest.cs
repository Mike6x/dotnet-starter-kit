using Ardalis.Specification;
using FSH.Framework.Core.DataIO;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Elearning.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;

public class ExportQuizResultsRequest : BaseFilter, IRequest<byte[]>
{
    public Guid? QuizId { get; set; }
    public string? UserId { get; set; }
    public bool? IsPass { get; set; }
}

public class ExportQuizResultsHandler(
    [FromKeyedServices("elearning:quizresults")]  IReadRepository<QuizResult> repository, IDataExport dataExport)
    : IRequestHandler<ExportQuizResultsRequest, byte[]>
{
    public async Task<byte[]> Handle(ExportQuizResultsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var spec = new ExportQuizResultsSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        
        return dataExport.ListToByteArray(items);
    }
}

public sealed class ExportQuizResultsSpecs : EntitiesByBaseFilterSpec<QuizResult, QuizResultExportDto>
{
    public ExportQuizResultsSpecs(ExportQuizResultsRequest request)
        : base(request) =>
        Query
            .Include(e => e.Quiz)
                .Where(e => e.QuizId.Equals(request.QuizId!.Value), request.QuizId.HasValue)
                .Where(e => e.SId != null && e.SId.Equals(request.UserId), !string.IsNullOrEmpty(request.UserId))
                .Where(e => e.IsPass.Equals(request.IsPass!), request.IsPass.HasValue)
                    .OrderByDescending(e => e.Created);
}
