using Azure.Messaging.ServiceBus;
using LubeAnalyst.Data;

namespace LubeAnalyst.Repository
{
    public class CustomerCommandRepository : ICustomerCommandRepository
    {
        private readonly LabDbContext _context;
        //private readonly ServiceBusClient _serviceBusClient;
        public CustomerCommandRepository(LabDbContext context)
        {
            _context = context;
            //_serviceBusClient = serviceBusClient;
        }
        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }       
    }
}
