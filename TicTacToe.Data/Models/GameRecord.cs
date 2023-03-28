using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Data.Models
{
    public class GameRecord
    {
        public int Id { get; set; }

        public long PlayedAt { get; set; }

        [Required]
        [StringLength(50)]
        public string PlayerX { get; set; } = null!;
        [Required]
        public string PlayerXType { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string PlayerO { get; set; } = null!;
        [Required]
        public string PlayerOType { get; set; } = null!;

        public int State { get; set; }

        [Required]
        public string Board { get; set; } = null!;

        public int WinningLine { get; set; }
    }
}