using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Identity.Users.Dtos;
using FSH.Framework.Core.Identity.Users.Features.ExportUsers;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FSH.Framework.Infrastructure.Identity.Users.Services
{
    internal partial class UserService
    {
        #region My Customize
        public async Task<PagedList<UserDetail>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken)
        {
            var spec = new EntitiesByPaginationFilterSpec<FshUser>(filter);

            var users = await userManager.Users
                .WithSpecification(spec)
                .ProjectToType<UserDetail>()
                .ToListAsync(cancellationToken);
                
            int count = await userManager.Users
                .CountAsync(cancellationToken);

            return new PagedList<UserDetail>(users, filter.PageNumber, filter.PageSize, count);
        }

        #endregion
    }
}