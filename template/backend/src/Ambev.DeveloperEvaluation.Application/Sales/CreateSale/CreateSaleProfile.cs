using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// AutoMapper profile for CreateSale feature.
/// </summary>
public class CreateSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of CreateSaleProfile.
    /// </summary>
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(dest => dest.Items, opt => opt.Ignore()); // We'll handle items separately

        CreateMap<CreateSaleItemDto, SaleItem>();

        CreateMap<Sale, CreateSaleResult>();

        CreateMap<SaleItem, CreateSaleItemResult>();
    }
}