using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// AutoMapper profile for UpdateSale feature.
/// </summary>
public class UpdateSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of UpdateSaleProfile.
    /// </summary>
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleCommand, Sale>()
            .ForMember(dest => dest.Items, opt => opt.Ignore()); // We'll handle items separately

        CreateMap<UpdateSaleItemDto, SaleItem>();

        CreateMap<Sale, UpdateSaleResult>();

        CreateMap<SaleItem, UpdateSaleItemResult>();
    }
}