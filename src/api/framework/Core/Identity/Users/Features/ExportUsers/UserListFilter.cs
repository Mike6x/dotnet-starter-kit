using FSH.Framework.Core.Paging;

namespace FSH.Framework.Core.Identity.Users.Features.ExportUsers;

public class UserListFilter : PaginationFilter
{
    public bool? IsActive { get; set; }
}
