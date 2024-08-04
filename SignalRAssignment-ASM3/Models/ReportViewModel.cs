namespace SignalRAssignment_ASM3.Models
{
    public class ReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PostCount { get; set; }
        public List<Post> Posts { get; set; }
        public AppUser TopUser { get; set; }
        public int TopUserPostCount { get; set; }
    }


}
