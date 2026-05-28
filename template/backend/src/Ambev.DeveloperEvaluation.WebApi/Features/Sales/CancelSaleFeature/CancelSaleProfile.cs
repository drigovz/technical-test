using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleFeature;

/// <summary>
/// AutoMapper profile for CancelSale WebApi feature.
/// </summary>
public class CancelSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of CancelSaleProfile.
    /// </summary>
    public CancelSaleProfile()
    {
        CreateMap<CancelSaleRequest, CancelSaleCommand>();
        CreateMap<CancelSaleResult, CancelSaleResponse>();
    }
}