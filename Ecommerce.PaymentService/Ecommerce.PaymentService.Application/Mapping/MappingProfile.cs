using AutoMapper;
using Ecommerce.PaymentService.Application.Dto;
using Ecommerce.PaymentService.Domain.Model;

namespace Ecommerce.PaymentService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Payment, PaymentDto>().ReverseMap();
        }
    }
}
