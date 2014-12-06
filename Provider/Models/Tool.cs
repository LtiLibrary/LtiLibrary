using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Provider.Models
{
    public class Tool
    {
        public Tool()
        {
            Outcomes = new HashSet<Outcome>();
        }

        public int ToolId { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.Html)]
        public string Description { get; set; }
        [DataType(DataType.Html)]
        public string Content { get; set; }
        public virtual ICollection<Outcome> Outcomes { get; set; }
    }

    public class ToolUser
    {
        public string ConsumerName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ReturnUrl { get; set; }
        public string Roles { get; set; }
    }

    public class PostScoreModel
    {
        public string ConsumerName { get; set; }
        public string ContextTitle { get; set; }
        public int OutcomeId { get; set; }
        public double? Score { get; set; }
        public int ToolId { get; set; }
    }
}