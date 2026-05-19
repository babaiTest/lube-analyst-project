using LubeAnalyst.Data;

namespace LubeAnalyst.Repository
{
    public class AddCustomerCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TestRequest> TestRequests { get; set; } = new List<TestRequest>();
    }
}
