using LubeAnalyst.Data;

namespace LubeAnalyst.Repository
{
    public interface ICustomerQueryRepository
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
    }
}
