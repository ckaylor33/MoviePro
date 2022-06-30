using Microsoft.AspNetCore.Mvc;
using MoviePro.Services.Interfaces;

namespace MoviePro.Controllers
{
    public class ActorsController : Controller
    {
        //provides user with the ability to request ActorDetail - need access to IRemoteMovieService
        //and IDataMappingService
        private readonly IRemoteMovieService _tmdbMovieService;
        private readonly IDataMappingService _mappingService;

        public ActorsController(IRemoteMovieService tmdbMovieService, IDataMappingService mappingService)
        {
            _tmdbMovieService = tmdbMovieService;
            _mappingService = mappingService;
        }

        public async Task<IActionResult> Details(int id)
        {
            //get actor details through _tmdbMovieService
            var actor = await _tmdbMovieService.ActorDetailAsync(id);

            //map through actor details with _mappingService
            actor = _mappingService.MapActorDetail(actor);


            return View(actor);
        }
    }
}
