using LubeAnalyst.Data;
using System;

namespace LubeAnalyst.Repository
{
    public class AddCustomerHandler
    {
        private readonly ICustomerCommandRepository _repository;

        public AddCustomerHandler(ICustomerCommandRepository repository)
        {
            _repository = repository;
        }

        public async Task<Customer> Handle(AddCustomerCommand command)
        {
            var customer = new Customer
            {
                Name = command.Name,
                Email = command.Email,
                Phone = command.Phone,
                CompanyName = command.CompanyName               
            };

            await _repository.AddCustomerAsync(customer);            
            return customer;
        }
    }
}
