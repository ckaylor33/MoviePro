﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoviePro.Data;
using MoviePro.Enums;
using MoviePro.Models;
using MoviePro.Models.Settings;
using MoviePro.Services.Interfaces;
using MoviePro.ViewModels;
using System.Diagnostics;

namespace MoviePro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRemoteMovieService _tmdbMovieService;
        private readonly AppSettings _appSettings;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IRemoteMovieService tmdbMovieService, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _context = context;
            _tmdbMovieService = tmdbMovieService;
            _appSettings = appSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            const int count = 16;
            var data = new LandingPageVM()
            {
                CustomCollections = await _context.Collection
                                                   .Include(c => c.MovieCollections)
                                                   .ThenInclude(mc => mc.Movie)
                                                   .ToListAsync(),
                NowPlaying = await _tmdbMovieService.SearchMovieAsync(MovieCategory.now_playing, count),
                Popular = await _tmdbMovieService.SearchMovieAsync(MovieCategory.popular, count),
                TopRated = await _tmdbMovieService.SearchMovieAsync(MovieCategory.top_rated, count),
                Upcoming = await _tmdbMovieService.SearchMovieAsync(MovieCategory.upcoming, count)


            };
            ViewData["api_key"] = _appSettings.MovieProSettings.TmDbApiKey;
            ViewData["Title"] = "Home";
            ViewBag.MovieCount = count;

            return View(data);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}