namespace P03_FootballBetting.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, Column(TypeName = ("VARCHAR(MAX)"))]
        public string LogoUrl { get; set; }

        [Required, MaxLength(3)]
        public string Initials { get; set; }

        [Required]
        public decimal Budget { get; set; }

        [Required, ForeignKey("Color")]
        public int PrimaryKitColorId { get; set; }

        public Color PrimaryKitColor { get; set; }

        [Required, ForeignKey("Color")]
        public int SecondaryKitColorId { get; set; }

        public Color SecondaryKitColor { get; set; }

        [ForeignKey("Town")]
        public int TownId { get; set; }

        public Town Town { get; set; }

        public ICollection<Player> Players { get; set; } = new HashSet<Player>();

        [InverseProperty("HomeTeam")]
        public ICollection<Game> HomeGames { get; set; } = new HashSet<Game>();

        [InverseProperty("AwayTeam")]
        public ICollection<Game> AwayGames { get; set; } = new HashSet<Game>();
    }
}
