using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MoviePro.Enums;
using MoviePro.Models.Settings;
using MoviePro.Models.TMDB;
using MoviePro.Services.Interfaces;
using System.Runtime.Serialization.Json;

namespace MoviePro.Services
{
    public class TMDBMovieService : IRemoteMovieService
    {
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClient; //creates http client and will execute request

        //CONSTRUCTOR//
        public TMDBMovieService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClient)
        {
            _appSettings = appSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<ActorDetail> ActorDetailAsync(int id)
        {
            //Step 1: Setup a default instance of ActorDetail
            ActorDetail actorDetail = new();

            //Step 2: Assemble the full request uri string out to TMDB API endpoint
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/person/{id}";

            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.TmDbApiKey },
                {"language", _appSettings.TMDBSettings.QueryOptions.Language }
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 3: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri); //assembles request message
            var response = await client.SendAsync(request); //sends request and stores response

            //Step 4: Return the ActorDetail object - deserialise JSON
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();//read content of response
                var dcjs = new DataContractJsonSerializer(typeof(ActorDetail));
                actorDetail = (ActorDetail)dcjs.ReadObject(responseStream);//update value of movieSearch

            }

            return actorDetail;
        }

        public async Task<MovieDetail> MovieDetailAsync(int id)
        {
            //Step 1: Setup a default instance of MovieDetail
            MovieDetail movieDetail = new();

            //Step 2: Assemble the full request uri string out to TMDB API endpoint
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/movie/{id}";

            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmDbApiKey },
                {"language", _appSettings.TMDBSettings.QueryOptions.Language },
                {"append_to_response", _appSettings.TMDBSettings.QueryOptions.AppendToResponse }
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 3: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri); //assembles request message
            var response = await client.SendAsync(request); //sends request and stores response

            //Step 4: Return the MovieDetail object - deserialise JSON
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();//read content of response
                var dcjs = new DataContractJsonSerializer(typeof(MovieDetail));
                movieDetail = dcjs.ReadObject(responseStream) as MovieDetail;//update value of movieSearch

            }

            return movieDetail;
        }

        public async Task<MovieSearch> SearchMovieAsync(MovieCategory category, int count)
        {
            //Step 1: Setup a default instance of MovieSearch
            MovieSearch movieSearch = new();

            //Step 2: Assemble the full request uri string out to TMDB API endpoint
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/movie/{category}";

            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmDbApiKey },
                {"language", _appSettings.TMDBSettings.QueryOptions.Language },
                {"page", _appSettings.TMDBSettings.QueryOptions.Page }
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 3: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri); //assembles request message
            var response = await client.SendAsync(request); //sends request and stores response

            //Step 4: Return the MovieSearch object - deserialise JSON
            if (response.IsSuccessStatusCode)
            {
                var dcjs = new DataContractJsonSerializer(typeof(MovieSearch));
                using var responseStream = await response.Content.ReadAsStreamAsync();//read content of response
                movieSearch = (MovieSearch)dcjs.ReadObject(responseStream);//update value of movieSearch
                movieSearch.results = movieSearch.results.Take(count).ToArray();
                movieSearch.results.ToList().ForEach(r => r.poster_path = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{r.poster_path}");
            }

            return movieSearch;
        }
    }
}
