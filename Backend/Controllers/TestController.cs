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
            if (!await _repository.UserHasLanguageLevel(userId)) return Ok(new { TestAvailable = false });
            return Ok(new { TestAvailable = true });
        } 
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpGet("writing")]
    public async Task<ActionResult> WritingTest(Languages language)
    {
        try
        {
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
    public async Task<ActionResult> WritingTestResult(WritingResult result, Languages language)
    {
        try
        {
            if (!HttpContext.Request.Cookies.TryGetValue("jwt", out var token))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var formattedQA = string.Join("\n\n", 
                result.Questions.Zip(result.Answers, (q, a) => $"{q}\n{a}"));
            var aiAnswer = await _aiClient.GetAiAnswer(
                $"How would you rate the user's language level based on the questions-answers? Choose between: Beginner, Elementary, Intermediate, Upper Intermediate, Advanced, Proficient \n\n{formattedQA} IMPORTANT: Only return the language level, nothing else" );
            if(!levels.Contains(aiAnswer.ToLower().Trim()))  return StatusCode(500, new { message = "AI did not return a proper level" });
            await _repository.AddUserLanguageLevel(userId, language, aiAnswer);
            return Ok();
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
            return StatusCode(500, new { message = ex.Message });
        }
    }
    

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    [HttpGet("reading")]
    public async Task<ActionResult> ReadingTest(Languages language)
    {
        try
        {
            var prompt = $@"In {language} language Create a short story (about 100 words) with 5 comprehension questions. Return your response as a valid JSON object with this structure: {{ ""story"": ""your story text here"",  ""questions"": [""question 1"", ""question 2"", ""question 3"", ""question 4"", ""question 5""] }} Important: Return ONLY the raw JSON object without any markdown formatting, code blocks, or backticks.";
            var aiAnswer = await _aiClient.GetAiAnswer(prompt);
            string cleanedResponse = CleanMarkdownResponse(aiAnswer);
            var responseData = JsonSerializer.Deserialize<ReadingTestResponse>(cleanedResponse);
            
            if (responseData == null)  return StatusCode(500, new { message = "Invalid response format from AI" });
            if (responseData.Questions.Length < 5)  return StatusCode(500, new { message = "Failed to generate enough questions" });
                    
            return Ok(new { 
                story = responseData.Story, 
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

    [HttpGet("blinded-text")]
    public async Task<IActionResult> BlindedTest(Languages language)
    {
        try
        {
            var prompt = $"In {language} language, generate a text where you blind 10 words with '-----', return more than the 10 valid words. Return your response as a valid JSON object with this structure: {{ \"\"story\"\": \"\"your story text here\"\",  \"\"words\"\": [\"\"word 1\"\", \"\"word 2\"\", \"\"word 3\"\", \"\"word 4\"\", \"\"word 5\"\", \"\"word 6\"\", \"\"word 7\"\", \"\"word 8\"\", \"\"word 9\"\", \"\"word 10\"\", \"\"word 11\"\", \"\"word 12\"\", \"\"word 13\"\", \"\"word 14\"\", \"\"word 15\"\"] }} Important: Return ONLY the raw JSON object without any markdown formatting, code blocks, or backticks.";
            var aiAnswer = await _aiClient.GetAiAnswer(prompt);
            string cleanedResponse = CleanMarkdownResponse(aiAnswer);
            var responseData = JsonSerializer.Deserialize<BlindedStoryResponse>(cleanedResponse);
            
            if (responseData == null)  return StatusCode(500, new { message = "Invalid response format from AI" });
            if (responseData.Words.Length < 15)  return StatusCode(500, new { message = "Failed to generate enough words" });
            return Ok(new { 
                story = responseData.Story, 
                words = responseData.Words
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
    
    //TODO: Repair the wrong sentences

   
    private string CleanMarkdownResponse(string response)
    {
        if (response.StartsWith("```json") || response.StartsWith("```"))
        {
            response = response.Substring(response.IndexOf('\n') + 1);
        }
        response = response.Replace("`", "");
        return response.Trim();
    }
    
}