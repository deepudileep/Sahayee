namespace Sahayee.Models.ViewModel
{
    public class AdminDashCount
    {
        public List<DashItems> DashItems { get; set; }
        public List<LatestItems> LatestItems { get; set; }

    }

    public class DashItems
    {
        public string Title { get; set; }
        public long ActiveCount { get; set; }
        public long InActiveCount { get; set; }
    }

    public class LatestItems
    {
        public string Title { get; set; }
    }
}
