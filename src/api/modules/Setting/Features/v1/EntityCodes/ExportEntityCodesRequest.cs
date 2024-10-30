using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Setting.Domain;
using MediatR;
using FSH.Framework.Core.DataIO;
using FSH.Framework.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using FSH.Framework.Core.Specifications;
using Ardalis.Specification;

namespace FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;

public class ExportEntityCodesRequest : BaseFilter, IRequest<byte[]>
{
    public CodeType? Type { get; set; }
}


public class ExportEntityCodesHandler(
    [FromKeyedServices("setting:EntityCode")]  IReadRepository<EntityCode> repository, IDataExport dataExport)
    : IRequestHandler<ExportEntityCodesRequest, byte[]>
{
    public async Task<byte[]> Handle(ExportEntityCodesRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var spec = new ExportEntityCodesSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        
        return dataExport.ListToByteArray(items);
    }
}


public sealed class ExportEntityCodesSpecs : EntitiesByBaseFilterSpec<EntityCode, EntityCodeExportDto>
{
    public ExportEntityCodesSpecs(ExportEntityCodesRequest request)
        : base(request) =>
            Query
                .Where(e => e.Type.Equals(request.Type!.Value), request.Type.HasValue)
                    .OrderBy(e => e.Order);
}
