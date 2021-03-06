namespace MoviePro.Models.Database
{
    public class MovieCrew
    {
        //Primary and foreign keys//
        public int Id { get; set; }
        public int MovieId { get; set; }

        //Descriptive properties//
        public int CrewID { get; set; }//generated by TMDB - used to get details from crew members
        public string Department { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public string ImageUrl { get; set; }

        //NAVIGATIONAL//
        public Movie Movie { get; set; }
    }
}
