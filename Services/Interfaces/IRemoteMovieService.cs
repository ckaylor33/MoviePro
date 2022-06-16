using MoviePro.Enums;
using MoviePro.Models.TMDB;

namespace MoviePro.Services.Interfaces
{
    public interface IRemoteMovieService
    {
        Task<MovieDetail> MovieDetailAsync(int id); //Any class implementing this interface, must
                                                    //have a function named MovieDetailAsync that takes in an
                                                    //int id and returns a task of type MovieDetail
        Task<MovieSearch> SearchMovieAsync(MovieCategory category, int count);

        Task<ActorDetail> ActorDetailAsync(int id);

        //Why use an interface? Because at the moment the app uses the TMDB API, but possible in the
        //future we may want to point to other APIs - when that happens we can create other sets of 
        //classes that also implement this interface, but are geared towards that other API. Having seperation
        //between the interface and concrete class that implements the interface is known as loose coupling.
        //This is likely to be present in any well architected software.
    }
}
