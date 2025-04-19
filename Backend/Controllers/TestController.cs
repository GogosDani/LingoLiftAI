using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.DTOs.AIDTOs;
using Backend.Exceptions.AIExceptions;
using Backend.Services.AIServices;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Backend.Controllers;

[Route("api/test")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly IUserLanguageRepository _repository;
    private readonly IAIClient _aiClient;
    private readonly string[] levels = {"beginner", "elementary", "intermediate", "upper intermediate", "advanced", "proficient"};
    
    public TestController(IUserLanguageRepository repository, IAIClient aiClient)
    {
        _repository = repository;
        _aiClient = aiClient;
    }
    
    [HttpGet("check-test-status")]
    public async Task<ActionResult> CheckTestStatus()
    {
        try
        {
            var userId = GetUserId();
            if (!await _repository.UserHasLevel(userId)) return Ok(new { TestAvailable = false });
            return Ok(new { TestAvailable = true });
        } 
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpPost("writing")]
    public async Task<ActionResult> WritingTest(WritingTestRequest request)
    {
        try
        {
            var languageEntity = await _repository.GetLanguageById(request.LanguageId);
            var language = languageEntity.LanguageName;
            var aiAnswer = await _aiClient.GetAiAnswer(
                $"Give me exactly three questions to test {language} level. Each question should require a few sentences to answer. Format your response with exactly three questions separated by semicolons (;). Do not include any additional text.");
            
            var questions = aiAnswer.Split(';')
                .Select(q => q.Trim())
                .ToArray();
            if (questions.Length != 3)
            {
                return StatusCode(500, new { message = "AI did not return exactly three questions" });
            }

            var userId = GetUserId();
            var id = await _repository.AddWritingQuestions(questions, userId);
            
            return Ok(new {questions = questions, id = id} );
        }
        catch (NullGeminiUrlException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
        catch (NullAnswerException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
    [HttpPost("writing-result")]
    public async Task<ActionResult> WritingTestResult(WritingResultRequest request)
    {
        try
        {
            var language = await _repository.GetLanguageById(request.LanguageId);
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var questions = await _repository.GetWritingQuestions(request.QuestionSetId);
            var formattedQA = string.Join("\n\n", 
                questions.Zip(request.Answers, (q, a) => $"{q}\n{a}"));
            var languageLevel = await _aiClient.GetAiAnswer(
                $"Evaluate the user's {language.LanguageName} language level based on these writing samples:\n\n{formattedQA}\n\n" +
                "Please only rate the answers, not the questions because the question are automatically generated." +
                "Use these criteria:\n" +
                "- Beginner: Basic vocabulary, simple sentences, frequent grammatical errors\n" +
                "- Elementary: Simple sentences, basic tenses, common vocabulary\n" +
                "- Intermediate: Some complex sentences, good vocabulary, occasional errors\n" +
                "- Upper Intermediate: Variety of structures, good control of grammar, few errors\n" +
                "- Advanced: Complex structures, rich vocabulary, natural expression\n" +
                "- Proficient: Near-native fluency, sophisticated language use\n\n" +
                "RESPOND WITH EXACTLY ONE WORD from the list above. No explanations or additional text.");
            if(!levels.Contains(languageLevel.ToLower().Trim()))  return StatusCode(500, new { message = "AI did not return a proper level" });
            await _repository.AddUserLanguageLevel(userId, request.LanguageId, languageLevel);
            return Ok(languageLevel);
        }
        catch (NullGeminiUrlException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
        catch (NullAnswerException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
    
    
    [HttpPost("reading")]
    public async Task<ActionResult> ReadingTest(ReadingTestRequest request)
    {
        try
        {
            var languageEntity = await _repository.GetLanguageById(request.LanguageId);
            var language = languageEntity.LanguageName;
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var level = await _repository.GetUserLanguageLevel(userId, request.LanguageId);
            var prompt =
                $"Create a reading comprehension exercise in {language} language for a student at {level} level.\n\n" +
                $"Requirements:\n" +
                $"- Text length: 200-300 words\n" +
                $"- Include vocabulary and grammar typical of {level} level\n" +
                $"- Include exactly 2 vocabulary items from the next level up\n" +
                $"- Include 1 grammatical structure from the next level up\n" +
                $"- Create exactly 4 open-ended questions about the text\n\n" +
                $"Return ONLY valid JSON in this exact format:\n" +
                $"{{\n  \"text\": \"Your reading passage here\",\n  \"questions\": [\n    \"Question 1\",\n    \"Question 2\",\n    \"Question 3\",\n    \"Question 4\"\n  ]\n}}";
            var aiAnswer = await _aiClient.GetAiAnswer(prompt);
            string cleanedResponse = CleanMarkdownResponse(aiAnswer);
            var responseData = JsonSerializer.Deserialize<ReadingTestResponse>(cleanedResponse);
            if (responseData == null)  return StatusCode(500, new { message = "Invalid response format from AI" });
            var id = await _repository.AddReadingTest(userId, responseData.Text, responseData.Questions);
            return Ok(new { 
                text = responseData.Text, 
                questions = responseData.Questions,
                id = id
            });
        }
        catch (NullGeminiUrlException ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = ex.Message});
        }
        catch (NullAnswerException ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = ex.Message});
        }
    }

    [HttpPost("reading-result")]
    public async Task<IActionResult> ReadingResult(ReadingResultRequest request)
    {
        try
        {
            var language = await _repository.GetLanguageById(request.LanguageId);
            var userId = GetUserId();
            var currentLevel = await _repository.GetUserLanguageLevel(userId, request.LanguageId);
            var nextLevel = await _repository.GetNextLevel(currentLevel);
            var prevLevel = await _repository.GetPreviousLevel(currentLevel);
            var test = await _repository.GetReadingTest(request.ReadingTestId);
            var questions = test.Questions.Select(x => x.QuestionText);
            var prompt =  $"Evaluate the user's reading comprehension in {language.LanguageName}.\n\n" +
                          $"Text: {test.Story}\n\n" +
                          $"Question-Answer pairs:\n" +
                          $"{string.Join("\n", questions.Select(q => q).Zip(request.Answers, (q, a) => $"Q: {q}\nA: {a}"))}\n\n" +
                          $"Current level: {currentLevel}\n" +
                          $"Based on the quality, accuracy, and depth of the answers, should the user:\n" +
                          $"1. Move down to {prevLevel} (if answers show significant gaps in understanding)\n" +
                          $"2. Stay at {currentLevel} (if answers show adequate understanding)\n" +
                          $"3. Move up to {nextLevel} (if answers show excellent understanding)\n\n" +
                          $"RESPOND WITH EXACTLY ONE OF THESE THREE LEVELS: {prevLevel}, {currentLevel}, or {nextLevel}. No explanation.";
            var newLevel = await _aiClient.GetAiAnswer(prompt);
            if (newLevel == currentLevel) return Ok();
            await _repository.ChangeLanguageLevel(userId, request.LanguageId, newLevel);
            return Ok(newLevel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
    
    [HttpPost("blinded")]
    public async Task<IActionResult> BlindedTest(BlindedTestRequest request)
    {
        try
        {
            var userId = GetUserId();
            var languageEntity = await _repository.GetLanguageById(request.languageId);
            var language = languageEntity.LanguageName;
            var level = await _repository.GetUserLanguageLevel(userId, request.languageId);
            var prompt =$"Create a fill-in-the-blank exercise in {language} for a {level} level student.\n\n" +
                        $"Requirements:\n" +
                        $"- Write a coherent passage (150-250 words)\n" +
                        $"- Replace exactly 10 words with blanks (represented as '_____')\n" +
                        $"- 7 blanks should be appropriate for {level} level\n" +
                        $"- 3 blanks should use vocabulary from the next level up\n" +
                        $"- Provide a list of 15 possible words (the 10 correct answers plus 5 distractors)\n\n" +
                        $"Return ONLY valid JSON in this exact format:\n" +
                        $"{{\n  \"story\": \"Your text with _____ blanks here\",\n  " +
                        $"\"words\": [\"word1\", \"word2\", ... , \"word15\"],\n  " +
                        $"\"answers\": [\"correct1\", \"correct2\", ... , \"correct10\"]\n}}";
            var aiAnswer = await _aiClient.GetAiAnswer(prompt);
            string cleanedResponse = CleanMarkdownResponse(aiAnswer);
            var responseData = JsonSerializer.Deserialize<BlindedStoryResponse>(cleanedResponse);
            if (responseData == null)  return StatusCode(500, new { message = "Invalid response format from AI" });
            if (responseData.Words.Length < 15)  return StatusCode(500, new { message = "Failed to generate enough words" });
            var id = await _repository.AddBlindedTest(userId, responseData.Story, responseData.Words, responseData.Answers);
            return Ok(new { 
                story = responseData.Story, 
                words = responseData.Words,
                id = id
            });
        }
        catch (NullGeminiUrlException ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = "Couldn't find Gemini Url!" });
        }
        catch (NullAnswerException ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = "AI answer is null!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    
      [HttpPost("blinded-result")]
    public async Task<IActionResult> BlindedTestResult(BlindedResultRequest request)
    {
        try
        {
            var test = await _repository.GetBlindedTest(request.BlindedTestId);
            var userId = GetUserId();
            int correctStandardLevel = 0;
            int correctHigherLevel = 0;
            var correctWords = test.Corrects.Select(c => c.Correct.ToLower()).ToList();
            var higherLevelWords = correctWords.Take(3).ToList();

            for (int i = 0; i < test.Corrects.Count; i++)
            {
                if (test.Corrects[i].Correct.ToLower() == request.UserAnswers[i].ToLower())
                {
                    if (higherLevelWords.Contains(test.Corrects[i].Correct.ToLower()))
                        correctHigherLevel++;
                    else
                        correctStandardLevel++;
                }
            }

            var level = await _repository.GetUserLanguageLevel(userId, request.LanguageId);
            var next = await _repository.GetNextLevel(level);
            var prev = await _repository.GetPreviousLevel(level);

            if (correctStandardLevel >= 6 && correctHigherLevel >= 2)
            {
                await _repository.ChangeLanguageLevel(userId, request.LanguageId, next);
            }
            else if (correctStandardLevel < 4 && correctHigherLevel == 0)
            {
                await _repository.ChangeLanguageLevel(userId, request.LanguageId, prev);
            }

            return Ok(new {standardCorrect = correctStandardLevel, higherCorrect = correctHigherLevel});
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
    [HttpPost("correction")]
    public async Task<IActionResult> CorrectionTest(BlindedTestRequest request)
    {
        try
        {
            var userId = GetUserId();
            var languageEntity = await _repository.GetLanguageById(request.languageId);
            var language = languageEntity.LanguageName;
            var level = await _repository.GetUserLanguageLevel(userId, request.languageId);
            var prompt = $"Create 5 sentences in {language} with grammatical or vocabulary errors for a {level} level student to correct.\n\n" +
                         $"Requirements:\n" +
                         $"- 3 sentences should contain errors appropriate for {level} level students\n" +
                         $"- 2 sentences should contain more complex errors typical of the next level up\n" +
                         $"- Include a mix of error types: grammar, vocabulary usage, word order, etc.\n" +
                         $"- Each sentence should contain exactly one error\n\n" +
                         $"Also provide the correct version of each sentence for evaluation purposes.\n\n" +
                         $"Return ONLY valid JSON in this exact format:\n" +
                         $"{{\n  \"sentences\": [\"incorrect1\", \"incorrect2\", \"incorrect3\", \"incorrect4\", \"incorrect5\"],\n" +
                         $"  \"corrections\": [\"correct1\", \"correct2\", \"correct3\", \"correct4\", \"correct5\"]\n}}";
            var aiAnswer = await _aiClient.GetAiAnswer(prompt);
            string cleanedResponse = CleanMarkdownResponse(aiAnswer);
            var responseData = JsonSerializer.Deserialize<CorrectionResponse>(cleanedResponse);
            var id = await _repository.AddCorrection(responseData.Sentences, userId);
            return Ok(new
            {
                sentences = responseData.Sentences,
                id = id
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
    [HttpPost("correction-result")]
    public async Task<IActionResult> CorrectionResult(CorrectionResultRequest request)
    {
        try
        {
            var sentences = await _repository.GetCorrectionTest(request.CorrectionId);
            var answers = request.Answers;
            var pairs = sentences.Sentences.Zip(answers, (sentence, answer) => $"Incorrect: {sentence}\nUser's answer: {answer}");
            var prompt = $"How many answers are correct? The first sentence is always incorrect, and the second is the user's answer. Return only a number (0-5) based on correct sentences.\n\n" +
                         string.Join("\n\n", pairs) + "only return a number (0-5)";

            var points = await _aiClient.GetAiAnswer(prompt);
            if (points == "5")
            {
                var userId = GetUserId();
                var level = await _repository.GetUserLanguageLevel(userId, request.LanguageId);
                var next = await _repository.GetNextLevel(level);
                await _repository.ChangeLanguageLevel(userId, request.LanguageId, next);
            }

            return Ok(points);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private string CleanMarkdownResponse(string response)
    {
        if (response.StartsWith("```json") || response.StartsWith("```"))
        {
            response = response.Substring(response.IndexOf('\n') + 1);
        }
        response = response.Replace("`", "");
        return response.Trim();
    }

    private string? GetUserId()
    {
        if (!HttpContext.Request.Cookies.TryGetValue("jwt", out var token))
        {
            return null;
        }
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
    
}