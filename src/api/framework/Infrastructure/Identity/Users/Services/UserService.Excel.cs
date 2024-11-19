using System.Collections.ObjectModel;
using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.DataIO;
using FSH.Framework.Core.Identity.Users.Dtos;
using FSH.Framework.Core.Identity.Users.Features.ExportUsers;
using FSH.Framework.Core.Mail;
using FSH.Framework.Core.Specifications;
using FSH.Framework.Core.Storage.File;
using FSH.Framework.Core.Storage.File.Features;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization;

namespace FSH.Framework.Infrastructure.Identity.Users.Services;

internal partial class UserService
{
    public async Task<byte[]> ExportAsync(ExportUsersRequest request, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByBaseFilterSpec<FshUser>(request);

        var list = await userManager.Users
            .WithSpecification(spec)
            .ProjectToType<UserExportDto>()
            .ToListAsync(cancellationToken);

        return dataExport.ListToByteArray(list);
    }

    public async Task<ImportResponse> ImportAsync(FileUploadCommand uploadFile, bool isUpdate, string origin, CancellationToken cancellationToken)
    {
        var items = await dataImport.ToListAsync<FshUser>(uploadFile, FileType.Excel);

        ImportResponse response = new()
        {
            TotalRecords = items.Count, 
            Message = ""
        };

        if (response.TotalRecords <= 0)
        {
            response.Message = "File is empty or Invalid format";
            return response;
        }

        int count = 0;
        try
        {
            if (isUpdate)
            {
                foreach (var item in items)
                {
                    var user = await userManager.FindByIdAsync(item.Id);
                    if (user != null)
                    {
                        user.FirstName = item.FirstName;
                        user.LastName = item.LastName;
                        user.UserName = item.UserName;
                        user.PhoneNumber = item.PhoneNumber;
                        user.IsActive = item.IsActive;
                        user.EmailConfirmed = item.IsActive && item.EmailConfirmed;

                        await userManager.UpdateAsync(user);
                        count++;
                    }                   
                }

                response.Message = $"Updated {count} Users successfully";
            }
            else
            {
                foreach (var item in items)
                {
                    var result = await userManager.CreateAsync(item, item.UserName!);
                    if (result.Succeeded)
                    {
                        count++;
                        // add basic role
                        await userManager.AddToRoleAsync(item, FshRoles.Basic);

                        // send confirmation mail
                        if (!string.IsNullOrEmpty(item.Email))
                        {
                            string emailVerificationUri = await GetEmailVerificationUriAsync(item, origin);
                            var mailRequest = new MailRequest(
                                new Collection<string> { item.Email },
                                "Confirm Registration",
                                emailVerificationUri);
                            jobService.Enqueue("email", () => mailService.SendAsync(mailRequest, CancellationToken.None));
                        }
                    }               
                }

                response.Message = $"Imported {count} Users successfully";
            }
        }
        catch (Exception)
        {
            response.Message = $"Internal error with {count} items!";
            return response;
        }

        return response;
    }
}
