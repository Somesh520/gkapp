using SQLite;

namespace GKFashionApp.Models
{
    public class FashionItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
        
        // Ye naya field hai Photo ke liye
        public string ImageUrl { get; set; } 
        public string Description { get; set; }
    }
}