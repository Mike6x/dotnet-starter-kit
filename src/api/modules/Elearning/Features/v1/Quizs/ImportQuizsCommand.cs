using FSH.Framework.Core.DataIO;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Storage.File;
using FSH.Framework.Core.Storage.File.Features;
using FSH.Starter.WebApi.Elearning.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;

public record ImportQuizsCommand(FileUploadCommand UploadFile, bool IsUpdate ) : IRequest<ImportResponse>;


// public class ImportQuizsHandler(
//     [FromKeyedServices("elearning:quiz")]  IRepository<Quiz> repository, IDataImport dataImport)
//     : IRequestHandler<ImportQuizsCommand, ImportResponse>
// {
//     public async Task<ImportResponse> Handle(ImportQuizsCommand request, CancellationToken cancellationToken)
//     {
//         ArgumentNullException.ThrowIfNull(request);
//         
//         var items = await dataImport.ToListAsync<Quiz>(request.UploadFile, FileType.Excel);
//         
//         ImportResponse response = new()
//         {
//             TotalRecords = items.Count, 
//             Message = ""
//     
//         };
//
//         if (response.TotalRecords <= 0)
//         {
//             response.Message = "File is empty or Invalid format";
//             return response;
//         }
//             
//         try
//         {
//             if (request.IsUpdate)
//             {
//                 await repository.UpdateRangeAsync(items, cancellationToken);
//                 response.Message = " Updated successful";
//             }
//             else
//             {
//                 await repository.AddRangeAsync (items, cancellationToken);
//                 response.Message = "Added successful";
//             }
//         }
//         catch (Exception)
//         {
//             response.Message = "Internal error!";
//             // throw new CustomException("Internal error!")
//         }
//
//         return response;
//     }
// }
//
