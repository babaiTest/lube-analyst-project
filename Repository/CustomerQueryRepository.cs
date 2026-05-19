using LubeAnalyst.Data;
using Microsoft.EntityFrameworkCore;

namespace LubeAnalyst.Repository
{
    public class CustomerQueryRepository : ICustomerQueryRepository
    {
        private readonly LabDbContext _context;
        public CustomerQueryRepository(LabDbContext context)
        {
            _context = context;
        }
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.AsNoTracking().ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
