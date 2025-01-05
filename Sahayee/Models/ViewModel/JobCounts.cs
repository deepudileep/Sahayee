namespace Sahayee.Models.ViewModel
{
    public class JobCounts
    {
        public List<CountType> Category { get; set; }
        public List<CountType> Hospital { get; set; }
        public List<CountType> Country { get; set; }
    }
    public class CountType
    {
        public int Count { get; set; }
        public string Type { get; set; }
    }
}
