namespace WebApplication1.Models
{
    public class Store
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public float FriendlinessLevel { get; set; }
        public List<Supplier> Suppliers { get; set; }
    }
}
