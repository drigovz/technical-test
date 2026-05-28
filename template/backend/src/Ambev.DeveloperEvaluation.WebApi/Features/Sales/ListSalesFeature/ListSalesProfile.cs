using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSalesFeature;

/// <summary>
/// AutoMapper profile for ListSales WebApi feature.
/// </summary>
public class ListSalesProfile : Profile
{
    public ListSalesProfile()
    {
        CreateMap<ListSalesResult, ListSalesResponse>();
        CreateMap<ListSaleItemResult, ListSaleItemResponse>();
    }
}