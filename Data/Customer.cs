namespace LubeAnalyst.Data
{
    public class Customer
    {
        public int? CustomerID { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? CompanyName { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<TestRequest> TestRequests { get; set; } = new List<TestRequest>();
    }
}
