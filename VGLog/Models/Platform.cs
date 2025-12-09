namespace VGLog.Models
{
    public class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Videogame> Videogames { get; set; } = new();
    }
}
