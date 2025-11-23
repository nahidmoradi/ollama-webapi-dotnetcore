using CommentAnalyzer.Models;

namespace CommentAnalyzer.Services;

public interface ICommentAnalyzerService
{
    Task<CommentSummary> AnalyzeCommentsAsync(List<Comment> comments);
    Task<string> GenerateSimpleSummaryAsync(List<Comment> comments);
    Task<List<DetailedCommentAnalysis>> AnalyzeCommentsDetailedAsync(List<Comment> comments);
}
