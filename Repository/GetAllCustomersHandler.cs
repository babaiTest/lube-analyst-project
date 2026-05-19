using LubeAnalyst.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace LubeAnalyst.Repository
{
    public class GetAllCustomersHandler
    {
        private readonly LabDbContext _context;

        public GetAllCustomersHandler(LabDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> Handle(GetAllCustomersQuery query)
        {
            return await _context.Customers.AsNoTracking().ToListAsync();
        }
    }
}
