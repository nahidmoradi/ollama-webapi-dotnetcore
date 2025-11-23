namespace CommentAnalyzer.Models;

public class Comment
{
    public int Id { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5 stars
    public DateTime CreatedAt { get; set; }
}

public class AnalysisRequest
{
    public List<Comment> Comments { get; set; } = new();
}

public class CommentSummary
{
    public string OverallSummary { get; set; } = string.Empty;
    public List<string> PositivePoints { get; set; } = new();
    public List<string> NegativePoints { get; set; } = new();
    public List<string> CommonThemes { get; set; } = new();
    public SentimentAnalysis Sentiment { get; set; } = new();
    public int TotalComments { get; set; }
    public double AverageRating { get; set; }
}

public class SentimentAnalysis
{
    public int PositiveCount { get; set; }
    public int NegativeCount { get; set; }
    public int NeutralCount { get; set; }
    public string OverallSentiment { get; set; } = string.Empty;
    public double PositivePercentage { get; set; }
    public double NegativePercentage { get; set; }
}

public class DetailedCommentAnalysis
{
    public int CommentId { get; set; }
    public string Sentiment { get; set; } = string.Empty; // Positive/Negative/Neutral
    public List<string> KeyPhrases { get; set; } = new();
    public List<string> Topics { get; set; } = new();
}
