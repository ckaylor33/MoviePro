using Microsoft.Extensions.Options;
using MoviePro.Enums;
using MoviePro.Models.Database;
using MoviePro.Models.Settings;
using MoviePro.Models.TMDB;
using MoviePro.Services.Interfaces;

namespace MoviePro.Services
{
    public class TMDBMappingService : IDataMappingService
    {
        private readonly AppSettings _appSettings;
        private readonly IImageService _imageService;

        public TMDBMappingService(IOptions<AppSettings> appSettings, IImageService imageService)
        {
            _appSettings = appSettings.Value;
            _imageService = imageService;
        }

        //Looks at property values to determine whether they can be used as is or they need
        //a default value instead
        public ActorDetail MapActorDetail(ActorDetail actor)
        {
            //Image
            actor.profile_path = BuildCastImage(actor.profile_path);

            //Bio
            if (string.IsNullOrEmpty(actor.biography))
            {
                actor.biography = "Not Available";
            }

            //Place of birth
            if (string.IsNullOrEmpty(actor.place_of_birth))
            {
                actor.place_of_birth = "Not available";
            }

            //Birthday
            if (string.IsNullOrEmpty(actor.birthday))
            {
                actor.birthday = "Not available";
            }
            else
            {
                actor.birthday = DateTime.Parse(actor.birthday).ToString("dd MMM, yyyy");
            }
            return actor;
        }

        //Takes in MovieDetail object, returns a movie instance with it's associated MovieCast 
        //& MovieCrew data - these are also grouped by their cast ids and ordered by popularity
        //with data limited to the top 20 records
        public async Task<Movie> MapMovieDetailAsync(MovieDetail movie)
        {
            Movie newMovie = null;

            try
            {
                newMovie = new Movie()
                {
                    MovieId = movie.id,
                    Title = movie.title,
                    TagLine = movie.tagline,
                    Overview = movie.overview,
                    RunTime = movie.runtime,
                    VoteAverage = movie.vote_average,
                    ReleaseDate = DateTime.Parse(movie.release_date),
                    TrailerUrl = BuildTrailerPath(movie.videos),
                    Backdrop = await EncodeBackdropImageAsync(movie.backdrop_path),
                    BackdropType = BuildImageType(movie.backdrop_path),
                    Poster = await EncodePosterImageAsync(movie.poster_path),
                    PosterType = BuildImageType(movie.poster_path),
                    Rating = GetRating(movie.release_dates)
                };

                var castMembers = movie.credits.cast.OrderByDescending(c => c.popularity)
                                                    .GroupBy(c => c.cast_id)
                                                    .Select(g => g.FirstOrDefault())
                                                    .Take(20)
                                                    .ToList();
                //iterate over cast members
                castMembers.ForEach(member =>
                {
                    //on each iteration, create a new MovieCast instance from cast member data
                    //then add to cast property
                    newMovie.Cast.Add(new MovieCast()
                    {
                        CastID = member.id,
                        Department = member.known_for_department,
                        Name = member.name,
                        Character = member.character,
                        ImageUrl = BuildCastImage(member.profile_path),
                    });
                });

                var crewMembers = movie.credits.crew.OrderByDescending(c => c.popularity)
                                        .GroupBy(c => c.id)
                                        .Select(g => g.First())
                                        .Take(20)
                                        .ToList();
                //iterate over crew members
                crewMembers.ForEach(member =>
                {
                    //on each iteration, create a new MovieCast instance from cast member data
                    //then add to cast property
                    newMovie.Crew.Add(new MovieCrew()
                    {
                        CrewID = member.id,
                        Department = member.department,
                        Name = member.name,
                        Job = member.job,
                        ImageUrl = BuildCastImage(member.profile_path),
                    });
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in MapMovieDetailAsync: {ex.Message}");
            }

            return newMovie;
        }

        //returns path to valid image or path to default image if not available
        private string BuildCastImage(string profilePath)
        {
            if (string.IsNullOrEmpty(profilePath))
            {
                return _appSettings.MovieProSettings.DefaultCastImage;
            }

            return $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{profilePath}";
        }

        private MovieRating GetRating(Release_Dates dates)
        {
            var movieRating = MovieRating.NR;
            //goes through GB release dates
            var certification = dates.results.FirstOrDefault(r => r.iso_3166_1 == "US");

            if (certification is not null)
            {
                var apiRating = certification.release_dates.FirstOrDefault(c => c.certification != "")?.certification.Replace("-", "");
                if (!string.IsNullOrEmpty(apiRating))
                {
                    //looks to turn a valid certification into a MovieRating Enum
                    movieRating = (MovieRating)Enum.Parse(typeof(MovieRating), apiRating, true);
                }
            }
            //if nothing found NR is returned as the rating
            return movieRating;
        }

        private async Task<byte[]> EncodePosterImageAsync(string path)
        {
            var posterPath = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{path}";
            return await _imageService.EncodeImageURLAsync(posterPath);
        }

        private string BuildImageType(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            return $"image/{Path.GetExtension(path).TrimStart('.')}";
        }

        private string BuildTrailerPath(Videos videos)
        {
            var videoKey = videos.results.FirstOrDefault(r => r.type.ToLower().Trim() == "trailer" && r.key != "")?.key;
            return string.IsNullOrEmpty(videoKey) ? videoKey : $"{_appSettings.TMDBSettings.BaseYouTubePath}{videoKey}";
        }

        private async Task<byte[]> EncodeBackdropImageAsync(string path)
        {
            var backdropPath = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultBackdropSize}/{path}";
            return await _imageService.EncodeImageURLAsync(backdropPath);
        }
    }
}