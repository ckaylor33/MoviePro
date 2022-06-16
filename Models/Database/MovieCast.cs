namespace MoviePro.Models.Database
{
    public class MovieCast
    {
        //Primary and foreign keys//
        public int Id { get; set; }
        public int MovieId { get; set; } //links cast member to movie

        //Descriptive properties//
        public int CastID { get; set; } //way to store a CastID assigned by TMDB API - detail ID for
        //cast or crew member
        public string Department { get; set; }
        public string Name { get; set; }
        public string Character { get; set; }
        public string ImageUrl { get; set; } //full path to image online for the cast member

        //NAVIGATIONAL//
        public Movie Movie { get; set; }


    }
}
