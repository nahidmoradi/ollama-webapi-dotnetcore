using Microsoft.AspNetCore.Mvc;
using CommentAnalyzer.Models;
using CommentAnalyzer.Services;

namespace CommentAnalyzer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentAnalyzerService _analyzerService;
    private readonly ILogger<CommentController> _logger;

    public CommentController(
        ICommentAnalyzerService analyzerService,
        ILogger<CommentController> logger)
    {
        _analyzerService = analyzerService;
        _logger = logger;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<CommentSummary>> AnalyzeComments([FromBody] AnalysisRequest request)
    {
        try
        {
            _logger.LogInformation($" Request Analysis {request.Comments.Count} Comment Received");

            var summary = await _analyzerService.AnalyzeCommentsAsync(request.Comments);

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Parsing Comments");
            return StatusCode(500, new { error = "Error Parsing Comments", detail = ex.Message });
        }
    }

    [HttpPost("summary")]
    public async Task<ActionResult<string>> GenerateSummary([FromBody] AnalysisRequest request)
    {
        try
        {
            var summary = await _analyzerService.GenerateSimpleSummaryAsync(request.Comments);
            return Ok(new { summary });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Generating Summary");
            return StatusCode(500, new { error = "Error Generating Summary", detail = ex.Message });
        }
    }

    [HttpPost("detailed")]
    public async Task<ActionResult<List<DetailedCommentAnalysis>>> AnalyzeDetailed([FromBody] AnalysisRequest request)
    {
        try
        {
            var results = await _analyzerService.AnalyzeCommentsDetailedAsync(request.Comments);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error In Partial Analysis");
            return StatusCode(500, new { error = "Error In Partial Analysis", detail = ex.Message });
        }
    }

    [HttpGet("sample")]
    public ActionResult<List<Comment>> GetSampleComments()
    {
        var sampleComments = new List<Comment>
        {
            new Comment
            {
                Id = 1,
                Author = "علی احمدی",
                Text = "محصول فوق‌العاده‌ای بود. کیفیت ساخت عالی و قیمت مناسب. پیشنهاد می‌کنم حتماً بخرید.",
                Rating = 5,
                CreatedAt = DateTime.Now.AddDays(-5)
            },
            new Comment
            {
                Id = 2,
                Author = "مریم رضایی",
                Text = "خوب بود اما بسته‌بندی ضعیف بود و محصول کمی آسیب دیده رسید. در کل راضی هستم.",
                Rating = 4,
                CreatedAt = DateTime.Now.AddDays(-4)
            },
            new Comment
            {
                Id = 3,
                Author = "حسین کریمی",
                Text = "اصلا راضی نیستم. کیفیت پایین و قیمت بالا. ارزش خرید نداره.",
                Rating = 2,
                CreatedAt = DateTime.Now.AddDays(-3)
            },
            new Comment
            {
                Id = 4,
                Author = "فاطمه محمدی",
                Text = "محصول خوبی هست. مطابق با توضیحات. ارسال سریع بود و راضی هستم.",
                Rating = 5,
                CreatedAt = DateTime.Now.AddDays(-2)
            },
            new Comment
            {
                Id = 5,
                Author = "رضا نوری",
                Text = "قیمت نسبت به کیفیت مناسب نیست. انتظار بیشتری داشتم.",
                Rating = 3,
                CreatedAt = DateTime.Now.AddDays(-1)
            },
            new Comment
            {
                Id = 6,
                Author = "سارا حسینی",
                Text = "عالی! بهترین خرید امسالم. کیفیت فوق‌العاده و قیمت مناسب. ممنون از فروشنده.",
                Rating = 5,
                CreatedAt = DateTime.Now
            }
        };

        return Ok(sampleComments);
    }

    [HttpGet("health")]
    public ActionResult CheckHealth()
    {
        return Ok(new
        {
            status = "healthy",
            message = "API آماده است",
            timestamp = DateTime.Now,
            ollama = "connected"
        });
    }
}
