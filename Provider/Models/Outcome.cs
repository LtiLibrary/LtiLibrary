namespace Provider.Models
{
    public class Outcome
    {
        public int OutcomeId { get; set; }
        public int ConsumerId { get; set; }
        public string ContextTitle { get; set; }
        public string LisResultSourcedId { get; set; }
        public string ServiceUrl { get; set; }
    }
}