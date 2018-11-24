using AutoMapper;
using Colibri.Models;
using Colibri.ViewModels;

namespace Colibri.Data
{
    /*
     * AutoMapper Helper Class
     */
    public class ColibriMappingProfile : Profile
    {
        public ColibriMappingProfile()
        {
            // create Maps: try to match Property -> Property, Type -> Type
            // Building an Exception: ForMember: customize Configuration for individual Member
            // ReverseMap: use the Information about the Create and create in opposite Order

            //CreateMap<Order, OrderViewModel>()
            //    .ForMember(o => o.OrderId, ex => ex.MapFrom(o => o.OrderId))
            //    .ReverseMap();


            // Map: Product, ProductViewModel
            //CreateMap<Product, ProductViewModel>()
            //    .ReverseMap();

            // Map: Products, AdvertisementViewModel
            CreateMap<Products, AdvertisementViewModel>()
                .ReverseMap();

            // Map: ApplicationUser, AdvertisementViewModel
            CreateMap<ApplicationUser, AdvertisementViewModel>()
                .ReverseMap();
        }
    }
}
