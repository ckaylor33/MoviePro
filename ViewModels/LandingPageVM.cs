using System;
using System.Collections.Generic;
using MoviePro.Models.Database;
using MoviePro.Models.TMDB;

namespace MoviePro.ViewModels
{
    public class LandingPageVM
    {
        //aggregate of the Collection Model and MovieSearch Model
        public List<Collection> CustomCollections { get; set; }
        public MovieSearch NowPlaying { get; set; } //display MovieSearch data on landing page
        public MovieSearch Popular { get; set; }
        public MovieSearch TopRated { get; set; }
        public MovieSearch Upcoming { get; set; }

    }
}
