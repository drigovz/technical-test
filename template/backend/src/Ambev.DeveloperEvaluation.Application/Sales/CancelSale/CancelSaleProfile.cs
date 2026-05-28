using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// AutoMapper profile for CancelSale feature.
/// </summary>
public class CancelSaleProfile : Profile
{
    public CancelSaleProfile()
    {
        CreateMap<CancelSaleCommand, CancelSaleResult>();
    }
}