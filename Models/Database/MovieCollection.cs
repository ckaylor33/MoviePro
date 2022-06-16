namespace MoviePro.Models.Database
{
    public class MovieCollection
    {
        //Primary and foreign keys//
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public int MovieId { get; set; }

        //Descriptive properties//
        public int Order { get; set; } //order or prioristise movies within collections - movies might
        //appear top of the list in one collection and bottom in another

        //NAVIGATIONAL PROPERTIES//
        //align closely to foreign keys above - store entire record pointed to by either the 
        //CollectionId or MovieId foreign keys - if CollectionId has a value of 2, then the Collection
        //property below would be the entire record that has the primary key of 2 in the Collection table
        public Collection Collection { get; set; }
        public Movie Movie { get; set; }
    }
}
