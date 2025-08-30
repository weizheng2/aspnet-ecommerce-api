using ECommerce.Application.Common;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Products;

public record GetAllProductsQuery(int Page, int PagesPerRecord) : IRequest<Result<PagedResult<GetProductDto>>>;
