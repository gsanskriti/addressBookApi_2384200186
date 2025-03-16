using AutoMapper;
using ModelLayer.model;

namespace BusinessLayer.mapping
{
    public class AddressBookProfile : Profile
    {
        public AddressBookProfile()
        {
            CreateMap<RequestModel, ResponseModel>();
            CreateMap<ResponseModel, RequestModel>();
        }
    }
}
