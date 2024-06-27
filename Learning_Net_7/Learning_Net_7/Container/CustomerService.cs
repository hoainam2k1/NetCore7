using AutoMapper;
using Learning_Net_7.Helper;
using Learning_Net_7.Modal;
using Learning_Net_7.Repository;
using Learning_Net_7.Repository.Models;
using Learning_Net_7.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Learning_Net_7.Container
{
    public class CustomerService : ICustomerService
    {
        private readonly LearndataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;
        public CustomerService(LearndataContext context, IMapper mapper, ILogger<CustomerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<APIResponse> Create(CustomerModal customer)
        {
            APIResponse response = new APIResponse();
            try
            {
                _logger.LogInformation("Create Begins");
                Customer _customer = _mapper.Map<CustomerModal,Customer>(customer);
                await _context.Customers.AddAsync(_customer);
                await _context.SaveChangesAsync();
                response.ResponseCode = 201;
                response.Result = customer.Code;
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex.Message, ex);
            }
            return response;
        }

        public async Task<List<CustomerModal>> GetAll()
        {
            List<CustomerModal> response = new List<CustomerModal>();
            var listCustomer = await _context.Customers.ToListAsync();
            if(listCustomer.Count > 0)
            {
                response = _mapper.Map<List<Customer>, List<CustomerModal>>(listCustomer);
            }
            return response;
        }

        public async Task<CustomerModal> GetByCode(string code)
        {
            CustomerModal response = new CustomerModal();
            var customer = await _context.Customers.FirstOrDefaultAsync(x =>x.Code == code);
            if (customer != null)
            {
                response = _mapper.Map<Customer, CustomerModal>(customer);
            }
            return response;
        }

        public async Task<APIResponse> Remove(string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                var _customer = await _context.Customers.FirstOrDefaultAsync(x => x.Code == code);
                if (_customer != null)
                {
                    _context.Customers.Remove(_customer);
                    await _context.SaveChangesAsync();
                    response.ResponseCode = 201;
                    response.Result = code;
                }
                else
                {
                    response.ResponseCode = 404;
                    response.ErrorMessage = "Data not found";
                }
                
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        public async Task<APIResponse> Update(CustomerModal customer, string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                var _customer = await _context.Customers.FirstOrDefaultAsync(x => x.Code == code);
                if (_customer != null)
                {
                    _customer.Name = customer.Name;
                    _customer.Email = customer.Email;
                    _customer.Phone = customer.Phone;
                    _customer.IsActive = customer.IsActive;
                    _customer.Creditlimit = customer.Creditlimit;
                    
                    await _context.SaveChangesAsync();
                    response.ResponseCode = 201;
                    response.Result = code;
                }
                else
                {
                    response.ResponseCode = 404;
                    response.ErrorMessage = "Data not found";
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
