namespace LubeAnalyst.Data
{
    public class TestRequest
    {
        public int TestRequestID { get; set; }
        public int CustomerID { get; set; }
        public Guid SampleID { get; set; }
        public string TestType { get; set; } = null!;
        public DateTime SubmissionTimestamp { get; set; }
        public string Status { get; set; } = "Pending";

        public Customer? Customer { get; set; } = null!;
        public ICollection<LabReport> LabReports { get; set; } = new List<LabReport>();
    }
}
