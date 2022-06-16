using MoviePro.Models.Database;
using MoviePro.Models.TMDB;

namespace MoviePro.Services.Interfaces
{
    public interface IDataMappingService
    {
        //methods to map MovieDetail and Actor data, as well as provide the tolling to build links to 
        //image sources for the cast and crew members
        Task<Movie> MapMovieDetailAsync(MovieDetail movie);
        ActorDetail MapActorDetail(ActorDetail actor);
    }
}
