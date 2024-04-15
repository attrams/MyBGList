using CsvHelper.Configuration.Attributes;

namespace MyBGList.Models.Csv
{
    public class BggRecord
    {
        [Name(name: "ID")]
        public int? ID { get; set; }

        public string? Name { get; set; }

        [Name(name: "Year Published")]
        public int? YearPublished { get; set; }

        [Name(name: "Min Players")]
        public int? MinPlayers { get; set; }

        [Name(name: "Max Players")]
        public int? MaxPlayers { get; set; }

        [Name(name: "Play Time")]
        public int? PlayTime { get; set; }

        [Name(name: "Min Age")]
        public int? MinAge { get; set; }

        [Name(name: "Users Rated")]
        public int? UsersRated { get; set; }

        [Name(name: "Rating Average")]
        public decimal? RatingAverage { get; set; }

        [Name(name: "BGG Rank")]
        public int? BGGRank { get; set; }

        [Name(name: "Complexity Average")]
        public decimal? ComplexityAverage { get; set; }

        [Name(name: "Owned Users")]
        public int? OwnedUsers { get; set; }

        public string? Mechanics { get; set; }

        public string? Domains { get; set; }
    }
}