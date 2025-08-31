using ECommerce.Application.Common;
using ECommerce.Application.Common.Mappings;
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
        var pagedProducts = await _productRepository.GetPagedAsync(request.PaginationDto);
        
        var pagedProductsDto = new PagedResult<GetProductDto>(
            data: pagedProducts.Data.Select(p => p.ToGetProductDto()).ToList(),
            totalRecords: pagedProducts.TotalRecords,
            page: pagedProducts.Page,
            recordsPerPage: pagedProducts.RecordsPerPage
        );

        return Result<PagedResult<GetProductDto>>.Success(pagedProductsDto);
    }

}
