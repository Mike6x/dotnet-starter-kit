using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Identity.Users.Dtos;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FSH.Framework.Infrastructure.Identity.Users.Services
{

    internal partial class UserService
    {
    
        #region My Customize

        public async Task<UserDetail> GetByNameAsync(string userName, CancellationToken cancellationToken)
        {
            var user = await userManager.Users
                .AsNoTracking()
                .Where(u => u.UserName == userName)
                    .FirstOrDefaultAsync(cancellationToken);

             _ = user ?? throw new NotFoundException($"User with username: {userName} not found!");

            return user.Adapt<UserDetail>();
        }

        public async Task<UserDetail> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await userManager.Users
                .AsNoTracking()
                .Where(u => u.Email == email)
                    .FirstOrDefaultAsync(cancellationToken);

            _ = user ?? throw new NotFoundException($"User with email address: {email} not found!");

            return user.Adapt<UserDetail>();
        }

        public async Task<UserDetail> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            var user = await userManager.Users
                .AsNoTracking()
                .Where(u => u.PhoneNumber == phoneNumber)
                    .FirstOrDefaultAsync(cancellationToken);

            _ = user ?? throw new NotFoundException($"User with phone number: {phoneNumber} not found!");

            return user.Adapt<UserDetail>();
        }

        #endregion

    }
}