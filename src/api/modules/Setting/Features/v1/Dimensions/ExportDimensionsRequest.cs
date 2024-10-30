using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.DataIO;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Setting.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Setting.Features.v1.Dimensions;

public class ExportDimensionsRequest : BaseFilter, IRequest<byte[]>
{
    public string? Type { get;  set; }
    public Guid? FatherId { get; set; }
    public bool? IsActive { get; set; }
}

public class ExportDimensionsHandler(
    [FromKeyedServices("setting:dimension")]  IReadRepository<Dimension> repository, IDataExport dataExport)
    : IRequestHandler<ExportDimensionsRequest, byte[]>
{
    public async Task<byte[]> Handle(ExportDimensionsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var spec = new ExportDimensionsSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        
        return dataExport.ListToByteArray(items);
    }
}

public sealed class ExportDimensionsSpecs : EntitiesByBaseFilterSpec<Dimension, DimensionExportDto>
{
    public ExportDimensionsSpecs(ExportDimensionsRequest request)
        : base(request) =>
        Query
            .Where(e => e.IsActive.Equals(request.IsActive!), request.IsActive.HasValue)
            .Where(e => string.IsNullOrEmpty(request.Type) || e.Type.Equals(request.Type, StringComparison.Ordinal))
            .Where(e => e.FatherId.Equals(request.FatherId!.Value), request.FatherId.HasValue)
            .OrderBy(e => e.Order);
}
