using Learning_Net_7.Helper;
using Learning_Net_7.Modal;

namespace Learning_Net_7.Service
{
    public interface ICustomerService
    {
        Task<List<CustomerModal>> GetAll();
        Task<CustomerModal> GetByCode(string code);
        Task<APIResponse> Remove(string code);
        Task<APIResponse> Update(CustomerModal customer, string code);
        Task<APIResponse> Create(CustomerModal customer);
    }
}
