using ECommerce.Application.Common;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Products;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<PagedResult<GetProductDto>>>
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<PagedResult<GetProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Products.GetQueryable();

        var totalRecords = await query.CountAsync();
        var productsDto = await query.Page(paginationDto)
                        .Select(p => p.ToGetProductDto())
                        .ToListAsync();

        var result = PagedResultHelper.Create(productsDto, totalRecords, paginationDto);
        return Result<PagedResult<GetProductDto>>.Success(result);
    }
}
