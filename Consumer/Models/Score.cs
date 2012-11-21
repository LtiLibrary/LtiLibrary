using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consumer.Models
{
    public class Score
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ScoreId { get; set; }

        public int AssignmentId { get; set; }
        public decimal DecimalValue { get; set; }
        public int UserId { get; set; }
    }
}