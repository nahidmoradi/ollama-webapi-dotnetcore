using OllamaSharp;
using OllamaSharp.Models.Chat;
using CommentAnalyzer.Models;
using System.Text;
using System.Text.Json;

namespace CommentAnalyzer.Services;

public class CommentAnalyzerService : ICommentAnalyzerService
{
    private readonly IOllamaApiClient _ollamaClient;
    private readonly ILogger<CommentAnalyzerService> _logger;

    public CommentAnalyzerService(
        IOllamaApiClient ollamaClient,
        ILogger<CommentAnalyzerService> logger)
    {
        _ollamaClient = ollamaClient;
        _logger = logger;
    }

    public async Task<CommentSummary> AnalyzeCommentsAsync(List<Comment> comments)
    {
        if (comments == null || comments.Count == 0)
        {
            return new CommentSummary
            {
                OverallSummary ="There Are No Comments To Analyze.",
                TotalComments = 0,
                AverageRating = 0
            };
        }

        _logger.LogInformation($"cStart Analysis {comments.Count} Comment...");

        var commentsText = PrepareCommentsForAnalysis(comments);

        var prompt = BuildAnalysisPrompt(commentsText);

        try
        {
            var chatRequest = new ChatRequest
            {
                Model = _ollamaClient.SelectedModel,
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = ChatRole.User,
                        Content = prompt
                    }
                }
            };

            var result = new StringBuilder();
            await foreach (var response in _ollamaClient.Chat(chatRequest))
            {
                result.Append(response.Message?.Content ?? "");
            }
            var analysisText = result.ToString();

            _logger.LogInformation("Analysis Completed Successfully");

            var summary = ParseAnalysisResult(analysisText, comments);

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Parsing Comments");
            throw;
        }
    }

    public async Task<string> GenerateSimpleSummaryAsync(List<Comment> comments)
    {
        if (comments == null || comments.Count == 0)
        {
            return "There Are No Comments To Summarize.";
        }

        var commentsText = string.Join("\n\n", comments.Select(c => 
            $"⭐ {c.Rating}/5 - {c.Author}:\n{c.Text}"));

       var prompt = $@"
        You are a Persian AI assistant tasked with summarizing user comments.

        User Comments:
        {commentsText}

        Please write a short summary (maximum 3 lines) of the user comments that:
        1. is useful and practical
        2. includes the main positive and negative points
        3. is in simple and fluent language

        Summary:";

        try
        {
            var chatRequest = new ChatRequest
            {
                Model = _ollamaClient.SelectedModel,
                Messages = new List<Message>
                {
                    new Message { Role = ChatRole.User, Content = prompt }
                }
            };

            var result = new StringBuilder();
            await foreach (var response in _ollamaClient.Chat(chatRequest))
            {
                result.Append(response.Message?.Content ?? "");
            }
            return result.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Generating Simple Summary");
            return "Error Generating Summary.";
        }
    }

    public async Task<List<DetailedCommentAnalysis>> AnalyzeCommentsDetailedAsync(List<Comment> comments)
    {
        var results = new List<DetailedCommentAnalysis>();

        foreach (var comment in comments)
        {
           var prompt = $@"
            Perform a detailed analysis of this comment:

            Comment: {comment.Text}
            Rating: {comment.Rating}/5

            Please specify the following:
            1. Overall sentiment (positive/negative/neutral)
            2. Important key phrases
            3. Main topics mentioned

            Provide the answer in JSON format:
            {{
            ""sentiment"": ""positive"" or ""negative"" or ""neutral"",
            ""keyPhrases"": [""phrase 1"", ""phrase 2""],
            ""topics"": [""topic 1"", ""topic 2""]
            }}";

            try
            {
                var chatRequest = new ChatRequest
                {
                    Model = _ollamaClient.SelectedModel,
                    Messages = new List<Message>
                    {
                        new Message { Role = ChatRole.User, Content = prompt }
                    }
                };

                var result = new StringBuilder();
                await foreach (var response in _ollamaClient.Chat(chatRequest))
                {
                    result.Append(response.Message?.Content ?? "");
                }
                var jsonResult = result.ToString();
                
                var jsonStart = jsonResult.IndexOf("{");
                var jsonEnd = jsonResult.LastIndexOf("}") + 1;
                
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonOnly = jsonResult.Substring(jsonStart, jsonEnd - jsonStart);
                    var parsed = JsonSerializer.Deserialize<JsonElement>(jsonOnly);

                    results.Add(new DetailedCommentAnalysis
                    {
                        CommentId = comment.Id,
                        Sentiment = parsed.GetProperty("sentiment").GetString() ?? "Neutral",
                        KeyPhrases = parsed.GetProperty("keyPhrases")
                            .EnumerateArray()
                            .Select(e => e.GetString() ?? "")
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToList(),
                        Topics = parsed.GetProperty("topics")
                            .EnumerateArray()
                            .Select(e => e.GetString() ?? "")
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToList()
                    });
                }
            }
            catch (Exception ex)
            {
               _logger.LogWarning(ex, $"Error Parsing Comment {comment.Id}");
            }
        }

        return results;
    }

    private string PrepareCommentsForAnalysis(List<Comment> comments)
    {
        var sb = new StringBuilder();
        foreach (var comment in comments)
        {
            sb.AppendLine($"⭐ Rating: {comment.Rating}/5");
            sb.AppendLine($"👤 Author: {comment.Author}");
            sb.AppendLine($"💬 Text: {comment.Text}");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private string BuildAnalysisPrompt(string commentsText)
    {
     return $@"
        You are a professional user comment analyst. Your task is to analyze the following comments in detail and comprehensively.

        User Comments:
        {commentsText}

        Please provide a complete analysis including the following:

        1. General Summary (2-3 sentences):
        - What is the overall opinion of the users?
        - What is the general level of satisfaction?

        2. Positive Points (at least 3):
        - What were the things that the users approved of?
        - What are the main strengths?

        3. Negative Points (at least 3):
        - What were the problems raised?
        - What are the weaknesses?

        4. Repetitive Topics (at least 2):
        - What did the users talk about the most?
        - What are the most frequent topics?

        5. Sentiment Analysis:
        - What percentage of comments are positive?
        - What percentage of comments are negative?
        - What is the overall sentiment?

        Write the answer in a structured and clear manner.
        Use bullet points.
        The answer should be completely in Persian and fluent.
        ";
        }

    private CommentSummary ParseAnalysisResult(string analysisText, List<Comment> comments)
    {
        var summary = new CommentSummary
        {
            TotalComments = comments.Count,
            AverageRating = comments.Average(c => c.Rating),
            OverallSummary = analysisText
        };

        var positiveSection = ExtractSection(analysisText, "Positive Points", "Negative Points");
        summary.PositivePoints = ExtractBulletPoints(positiveSection);

        var negativeSection = ExtractSection(analysisText, "Negative Points", "Repeating Topics");
        summary.NegativePoints = ExtractBulletPoints(negativeSection);

        var themesSection = ExtractSection(analysisText, "Repeating Topics", "Sentiment Analysis");
        summary.CommonThemes = ExtractBulletPoints(themesSection);

        summary.Sentiment = AnalyzeSentiment(comments);

        return summary;
    }

    private string ExtractSection(string text, string startMarker, string endMarker)
    {
        var startIndex = text.IndexOf(startMarker, StringComparison.OrdinalIgnoreCase);
        if (startIndex < 0) return string.Empty;

        var endIndex = text.IndexOf(endMarker, startIndex, StringComparison.OrdinalIgnoreCase);
        if (endIndex < 0) endIndex = text.Length;

        return text.Substring(startIndex, endIndex - startIndex);
    }

    private List<string> ExtractBulletPoints(string text)
    {
        var points = new List<string>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("-") || trimmed.StartsWith("•") || 
                trimmed.StartsWith("*") || char.IsDigit(trimmed.FirstOrDefault()))
            {
                var point = trimmed.TrimStart('-', '•', '*', ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', ')');
                if (!string.IsNullOrWhiteSpace(point))
                {
                    points.Add(point.Trim());
                }
            }
        }

        return points.Take(5).ToList();
    }

    private SentimentAnalysis AnalyzeSentiment(List<Comment> comments)
    {
        var positive = comments.Count(c => c.Rating >= 4);
        var negative = comments.Count(c => c.Rating <= 2);
        var neutral = comments.Count - positive - negative;

        var total = comments.Count;

        return new SentimentAnalysis
        {
            PositiveCount = positive,
            NegativeCount = negative,
            NeutralCount = neutral,
            PositivePercentage = Math.Round((double)positive / total * 100, 1),
            NegativePercentage = Math.Round((double)negative / total * 100, 1),
            OverallSentiment = positive > negative ? "Positive" : negative > positive ? "Negative" : "Neutral"
        };
    }
}
