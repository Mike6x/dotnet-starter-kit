using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Catalog.Application.Products.Get.v1;
using FSH.Starter.WebApi.Catalog.Application.Products.Search.v1;
using MediatR;

namespace FSH.Starter.WebApi.Catalog.Application.Products.GetList.v1;

public record GetProductsRequest(BaseFilter Filter) : IRequest<List<ProductDto>>;