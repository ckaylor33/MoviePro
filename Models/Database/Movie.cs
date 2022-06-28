using MoviePro.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviePro.Models.Database
{
    public class Movie
    {
        //Primary and foreign keys//
        public int Id { get; set; }
        public int MovieId { get; set; } //id within TMDB

        //Descriptive properties//
        public string Title { get; set; }
        public string TagLine { get; set; }
        public string Overview { get; set; }
        public int RunTime { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }

        public MovieRating Rating { get; set; }

        public float VoteAverage { get; set; }

        public byte[] Poster { get; set; }
        public string PosterType { get; set; }

        public byte[] Backdrop { get; set; }
        public string BackdropType { get; set; }

        public string TrailerUrl { get; set; }

        [NotMapped]
        [Display(Name = "Poster Image")]
        public IFormFile PosterFile { get; set; } // helps us communicate a custom user selection when 
                                                  //they create a custom movie and store a poster file - value comes from a form submission

        [NotMapped]
        [Display(Name = "Backdrop Image")]
        public IFormFile BackdropFile { get; set; } // same as above

        //NAVIGATIONAL//
        public ICollection<MovieCollection> Collections { get; set; } = new HashSet<MovieCollection>();
        //movie capable of belonging to any number of MovieCollections
        public ICollection<MovieCast> Cast { get; set; } = new HashSet<MovieCast>();
        public ICollection<MovieCrew> Crew { get; set; } = new HashSet<MovieCrew>();
    }
}
