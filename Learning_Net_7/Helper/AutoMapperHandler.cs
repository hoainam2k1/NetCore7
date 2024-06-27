using AutoMapper;
using Learning_Net_7.Modal;
using Learning_Net_7.Repository.Models;

namespace Learning_Net_7.Helper
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler() { 
        CreateMap<Customer, CustomerModal>().ForMember(item => item.StatusName, otp => otp.MapFrom(item => (item.IsActive != null && item.IsActive.Value) ? "Active" : "In active")).ReverseMap();
        }
    }
}
