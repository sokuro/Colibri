using AutoMapper;
using Colibri.Models;
using Colibri.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Data
{
    public class ColibriMappingProfile : Profile
    {
        public ColibriMappingProfile()
        {
            // create Maps: try to match Property -> Property, Type -> Type
            // Building an Exception: ForMember: customize Configuration for individual Member
            // ReverseMap: use the Information about the Create and create in opposite Order
            CreateMap<Order, OrderViewModel>()
                .ForMember(o => o.OrderId, ex => ex.MapFrom(o => o.OrderId))
                .ReverseMap();

            // new Map OrderItem -> OrderItemViewModel
            CreateMap<OrderItem, OrderItemViewModel>()
                .ReverseMap();
        }
    }
}
