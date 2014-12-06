using System.ComponentModel.DataAnnotations;

namespace Provider.Models
{
    public class Consumer
    {
        public int ConsumerId { get; set; }

        [Required, Display(Name = "Consumer Name")]
        public string Name { get; set; }

        [Required]
        public string Key { get; set; }
        
        [Required]
        public string Secret { get; set; }
    }
}