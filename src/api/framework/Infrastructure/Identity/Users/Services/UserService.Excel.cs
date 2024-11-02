using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Identity.Users.Dtos;
using FSH.Framework.Core.Identity.Users.Features.ExportUsers;
using FSH.Framework.Core.Specifications;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FSH.Framework.Infrastructure.Identity.Users.Services;

internal partial class UserService
{
    public async Task<byte[]> ExportAsync(UserListFilter filter, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<FshUser>(filter);

        var list = await userManager.Users
            .WithSpecification(spec)
            .ProjectToType<UserExportDto>()
            .ToListAsync(cancellationToken);

        return dataExport.ListToByteArray(list);
    }
}
