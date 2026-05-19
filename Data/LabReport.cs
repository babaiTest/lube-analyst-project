namespace LubeAnalyst.Data
{
    public class LabReport
    {
        public int LabReportID { get; set; }
        public int TestRequestID { get; set; }
        public string? ReportFilePath { get; set; }
        public string? ReportData { get; set; }
        public DateTime GeneratedAt { get; set; }

        public TestRequest? TestRequest { get; set; } = null!;
    }
}
