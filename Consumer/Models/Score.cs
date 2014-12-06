namespace Consumer.Models
{
    public class Score
    {
        public int ScoreId { get; set; }
        public int AssignmentId { get; set; }
        public double DoubleValue { get; set; }
        public string UserId { get; set; }
    }

    public class ReadScoreModel
    {
        public Score Score { get; set; }
        public bool IsValid { get; set; }
    }
}