using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Backend.DTOs.AIDTOs;
using Backend.Exceptions.AIExceptions;
using Backend.Models;
using Backend.Services.AIServices;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/test")]
[ApiController]
public class TestController : ControllerBase
{

    private readonly IUserLanguageRepository _repository;
    private readonly IAIClient _aiClient;
    private readonly  string[] levels = new string[]{"beginner", "elementary", "intermediate", "upper intermediate", "advanced", "proficient"};
    
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
            if (!HttpContext.Request.Cookies.TryGetValue("jwt", out var userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
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
            return Ok(questions);
        }
        catch (NullGeminiUrlException ex)
        {
            return StatusCode(500, new { message = "Couldn't find Gemini Url!" });
        }
        catch (NullAnswerException ex)
        {
            return StatusCode(500, new { message = "AI answer is null!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("writing-result")]
    public async Task<ActionResult> WritingTestResult(WritingResultRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var formattedQA = string.Join("\n\n", 
                request.Questions.Zip(request.Answers, (q, a) => $"{q}\n{a}"));
            var languageLevel = await _aiClient.GetAiAnswer(
                $"How would you rate the user's language level based on the questions-answers? Choose between: Beginner, Elementary, Intermediate, Upper Intermediate, Advanced, Proficient \n\n{formattedQA} IMPORTANT: Only return the language level, nothing else" );
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
                $"Create a reading comprehension exercise in {language} language that precisely tests the UPPER BOUNDARY of the {level} language level. The text should be between 200 and 300 words, and include complex sentence structures and vocabulary typical of the {level} level, but incorporate 2 advanced vocabulary words and one complex grammatical structure characteristic of the next level. EXPECTED OUTPUT IN JSON FORMAT:{{\n  \"\"text\"\": \"\"The text for the reading comprehension task\"\",\n  \"\"questions\"\": [\n    \"\"First question as a string\"\",\n    \"\"Second question as a string\"\"\n  ]\n}}\"";
            var aiAnswer = await _aiClient.GetAiAnswer(prompt);
            string cleanedResponse = CleanMarkdownResponse(aiAnswer);
            var responseData = JsonSerializer.Deserialize<ReadingTestResponse>(cleanedResponse);
            if (responseData == null)  return StatusCode(500, new { message = "Invalid response format from AI" });
            return Ok(new { 
                text = responseData.Text, 
                questions = responseData.Questions 
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

    [HttpPost("reading-result")]
    public async Task<IActionResult> ReadingResult(ReadingResultRequest request)
    {
        try
        {
            var userId = GetUserId();
            var currentLevel = await _repository.GetUserLanguageLevel(userId, request.LanguageId);
            var nextLevel = await _repository.GetNextLevel(currentLevel);
            var prevLevel = await _repository.GetPreviousLevel(currentLevel);
            var prompt =
                $"User had a reading test. The story was: {request.Text}, the question and answers combos: {request.Questions[0]} - {request.Answers[0]}, {request.Questions[1]} - {request.Answers[1]}. DO you think the user should stay on {currentLevel} or down rank to {prevLevel} or uprank to {nextLevel}? Answer with just the level, nothing else and only examine the user's answers!";
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
            var prompt = $"In {language} language, in language level {level}, generate a text where you blind 10 words with '-----'. 3 words/sentences should be greater than the {level} level. Return more than the 15 words, 10 of them should be the correct answers. Return the text, the 15 choosable answers, the 10 correct answers. Return your response as a valid JSON object with this structure: {{ \"\"story\"\": \"\"your story text here\"\",  \"\"words\"\": [\"\"word 1\"\", \"\"word 2\"\", \"\"word 3\"\", \"\"word 4\"\", \"\"word 5\"\", \"\"word 6\"\", \"\"word 7\"\", \"\"word 8\"\", \"\"word 9\"\", \"\"word 10\"\", \"\"word 11\"\", \"\"word 12\"\", \"\"word 13\"\", \"\"word 14\"\", \"\"word 15\"\"], \\\"\\\"answers\\\"\\\": [\\\"\\\"word 1\\\"\\\", \\\"\\\"word 2\\\"\\\", \\\"\\\"word 3\\\"\\\", \\\"\\\"word 4\\\"\\\", \\\"\\\"word 5\\\"\\\", \\\"\\\"word 6\\\"\\\", \\\"\\\"word 7\\\"\\\", \\\"\\\"word 8\\\"\\\", \\\"\\\"word 9\\\"\\\", \\\"\\\"word 10\\\"\\\" }} Important: Return ONLY the raw JSON object without any markdown formatting, code blocks, or backticks.";
            var aiAnswer = await _aiClient.GetAiAnswer(prompt);
            string cleanedResponse = CleanMarkdownResponse(aiAnswer);
            var responseData = JsonSerializer.Deserialize<BlindedStoryResponse>(cleanedResponse);
            if (responseData == null)  return StatusCode(500, new { message = "Invalid response format from AI" });
            if (responseData.Words.Length < 15)  return StatusCode(500, new { message = "Failed to generate enough words" });
            return Ok(new { 
                story = responseData.Story, 
                words = responseData.Words,
                corrects = responseData.Answers
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
            var userId = GetUserId();
            int correct = 0;
            for (int i = 0; i < request.CorrectAnswers.Length; i++)
            {
                if (request.CorrectAnswers[i].ToLower() == request.UserAnswers[i].ToLower()) correct++;
            }

            if (correct == 10)
            {
                var level = await _repository.GetUserLanguageLevel(userId, request.LanguageId);
                var next = await _repository.GetNextLevel(level);
                await _repository.ChangeLanguageLevel(userId, request.LanguageId, next);
            }

            if (correct < 4)
            {
                var level = await _repository.GetUserLanguageLevel(userId, request.LanguageId);
                var prev = await _repository.GetPreviousLevel(level);
                await _repository.ChangeLanguageLevel(userId, request.LanguageId, prev);
            }
            return Ok(correct);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
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