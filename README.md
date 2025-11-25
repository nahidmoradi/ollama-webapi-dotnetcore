# 🧠 Comment Analyzer with Ollama + Llama3.1

## Analyze and summarize user comments like Digikala (No Cloud Required!)

This project provides a complete API for intelligent user comment analysis using:
- ✅ Ollama - Run LLM locally
- ✅ Llama3.1 - Powerful Meta model
- ✅ Microsoft.Extensions.AI - Official Microsoft package
- ✅ Semantic Kernel - Microsoft AI framework
- ✅ .NET 9 - Latest version

---

## Installation & Setup

### Step 1: Install Ollama

Windows:
```powershell
choco install ollama
```

Linux/Mac:
```bash
curl -fsSL https://ollama.ai/install.sh | sh
```

### Step 2: Download Llama3.1 Model

```bash
ollama pull llama3.1:latest
```

### Step 3: Test Ollama

```bash
ollama run llama3
>>> Hello! Can you create a summary of user comments?
```

### Step 4: Run .NET Project

```powershell
cd backend/ollama
dotnet restore
dotnet run
```

API will be available on port 5100:
- Swagger UI: http://localhost:5100/swagger
- API Base: http://localhost:5100/api/comment

---

## API Endpoints

## 📡 API Endpoints

### 1️⃣ Full analysis 
```http
POST /api/comment/analyze
Content-Type: application/json

{
  "comments": [
    {
      "id": 1,
      "author": "علی احمدی",
      "text": "محصول عالی بود!",
      "rating": 5,
      "createdAt": "2025-11-23T10:00:00"
    }
  ]
}
```

**پاسخ:**
```json
{
  "overallSummary": "کاربران از کیفیت محصول راضی هستند...",
  "positivePoints": [
    "کیفیت ساخت عالی",
    "قیمت مناسب",
    "ارسال سریع"
  ],
  "negativePoints": [
    "بسته‌بندی ضعیف",
    "راهنمای استفاده ناقص"
  ],
  "commonThemes": [
    "کیفیت محصول",
    "قیمت"
  ],
  "sentiment": {
    "positiveCount": 4,
    "negativeCount": 1,
    "neutralCount": 1,
    "overallSentiment": "مثبت",
    "positivePercentage": 66.7,
    "negativePercentage": 16.7
  },
  "totalComments": 6,
  "averageRating": 4.2
}
```

### 2️⃣ Simple summary
```http
POST /api/comment/summary
Content-Type: application/json

{
  "comments": [...]
}
```

### 3️⃣ Detailed analysis of each comment
```http
POST /api/comment/detailed
Content-Type: application/json

{
  "comments": [...]
}
```

### 4️⃣ Get a sample comment
```http
GET /api/comment/sample
```

### 5️⃣ System health check
```http
GET /api/comment/health
```

---


---

## How It Works

1. Receive comments from user.
2. Prepare a structured prompt for Llama3.1.
3. Send prompt to Ollama using Microsoft.Extensions.AI.
4. Parse response to structured JSON.
5. Return result to user.

---

## Troubleshooting

- Connection refused: `ollama list` & `ollama serve`
- Model not found: `ollama pull llama3`
- Slow response: use smaller model, reduce prompt length, or GPU.

---

## Local vs Cloud APIs

| Feature | Ollama (Local) | OpenAI API |
|---------|----------------|------------|
| Cost | Free ✅ | Paid ❌ |
| Privacy | Full ✅ | Limited ⚠️ |
| Speed | Medium ⚠️ | Fast ✅ |
| Offline | Yes ✅ | No ❌ |
| Limitations | Hardware ⚠️ | Quota 💰 |

---

## Frontend Integration (Angular)

```typescript
@Injectable({ providedIn: 'root' })
export class CommentAnalyzerService {
  private apiUrl = 'http://localhost:5100/api/comment';
  analyzeComments(comments: Comment[]): Observable<CommentSummary> {
    return this.http.post<CommentSummary>(`${this.apiUrl}/analyze`, { comments });
  }
}
```

HTML example:
```html
<div class="comment-summary">
  <h3>User Comments Summary</h3>
  <p>{{ summary.overallSummary }}</p>
  <div class="positive-points">
    <h4>Positive Points:</h4>
    <ul>
      <li *ngFor="let point of summary.positivePoints">{{ point }}</li>
    </ul>
  </div>
  <div class="negative-points">
    <h4>Negative Points:</h4>
    <ul>
      <li *ngFor="let point of summary.negativePoints">{{ point }}</li>
    </ul>
  </div>
</div>
```

---

## Notes

- Make sure Ollama is running before starting the project.
- Llama3.1 is ~4.9GB.
- Send at least 5 comments for better results.
- Persian supported; English more accurate.

---

## Enjoy!

Analyze user comments with AI like Digikala—without cloud costs! 🚀

## 👨‍💻 Developer

- GitHub: [nahidmoradi](https://github.com/nahidmoradi)
- Email: n.morady@gmail.com

## Linkedin
 https://www.linkedin.com/in/nahid-moradi-84959a65/
