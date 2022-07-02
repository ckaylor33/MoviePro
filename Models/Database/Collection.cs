namespace MoviePro.Models.Database
{
    public class Collection
    {
        //Primary and foreign keys//
        public int Id { get; set; }

        //Descriptive properties//
        public string? Name { get; set; }
        public string? Description { get; set; }

        //NAVIGATIONAL//
        public ICollection<MovieCollection> MovieCollections { get; set; } = new HashSet<MovieCollection>();
    }
}
