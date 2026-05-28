using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleFeature;

/// <summary>
/// AutoMapper profile for UpdateSale WebApi feature.
/// </summary>
public class UpdateSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of UpdateSaleProfile.
    /// </summary>
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
        CreateMap<UpdateSaleItemRequest, UpdateSaleItemDto>();
        CreateMap<UpdateSaleResult, UpdateSaleResponse>();
        CreateMap<UpdateSaleItemResult, UpdateSaleItemResponse>();
    }
}