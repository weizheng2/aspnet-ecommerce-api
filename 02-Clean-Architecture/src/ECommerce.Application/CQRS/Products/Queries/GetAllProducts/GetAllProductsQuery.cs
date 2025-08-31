using ECommerce.Application.Common;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Products;

public record GetAllProductsQuery(PaginationDto PaginationDto) : IRequest<Result<PagedResult<GetProductDto>>>;
