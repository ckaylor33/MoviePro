using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviePro.Data;
using MoviePro.Models.Database;

namespace MoviePro.Controllers
{
    public class MovieCollectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieCollectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            //if given no id, it defaults to the All collection id
            id ??= (await _context.Collection.FirstOrDefaultAsync(c => c.Name.ToUpper() == "All")).Id;

            //will fill in collection dropdown and set default selected value
            ViewData["CollectionId"] = new SelectList(_context.Collection, "Id", "Name", id);

            var allMovieIds = await _context.Movie.Select(m => m.Id).ToListAsync();

            //all movie ids in specified collection and order according to order specified by the user in view
            var movieIdsInCollection = await _context.MovieCollection
                                                      .Where(m => m.CollectionId == id)
                                                      .OrderBy(m => m.Order)
                                                      .Select(m => m.MovieId)
                                                      .ToListAsync();

            //list of movie ids not in collection
            var movieIdsNotInCollection = allMovieIds.Except(movieIdsInCollection);

            var moviesInCollection = new List<Movie>();
            //each record given the name movieId, allows me to say for each Id in the collection
            //I'll use it to find the record and add that record to the moviesInCollection
            moviesInCollection.ForEach(movieId => moviesInCollection.Add(_context.Movie.Find(movieId)));

            //user can change movies that appear inside collection
            ViewData["IdsInCollection"] = new MultiSelectList(moviesInCollection, "Id", "Title");

            //goes out to Movie table and gathers records not trapped by entity framework
            var moviesNotInCollection = await _context.Movie.AsNoTracking().Where(m => movieIdsNotInCollection.Contains(m.Id)).ToListAsync();
            ViewData["IdsNotInCollection"] = new MultiSelectList(moviesNotInCollection, "Id", "Title");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, List<int> idsInCollection)
        {
            var oldRecords = _context.MovieCollection.Where(c => c.CollectionId == id);
            _context.MovieCollection.RemoveRange(oldRecords);
            await _context.SaveChangesAsync();

            if (idsInCollection != null)
            {
                int index = 1;
                idsInCollection.ForEach(movieId =>
                {
                    _context.Add(new MovieCollection()
                    {
                        CollectionId = id,
                        MovieId = movieId,
                        Order = index++
                    });
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { id });
        }
    }
}
