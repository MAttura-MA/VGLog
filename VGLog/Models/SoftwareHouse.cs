namespace VGLog.Models
{
    public class SoftwareHouse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Country { get; set; }
        public int? FoundedYear { get; set; }
        public List<Videogame> Videogames { get; set; } = new();
    }
}
