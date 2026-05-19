using LubeAnalyst.Data;

namespace LubeAnalyst.Repository
{
    public interface ICustomerCommandRepository
    {
        Task<Customer> AddCustomerAsync(Customer customer);
        
    }
}
