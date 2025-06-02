using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly LingoLiftContext _context;
    
    public TopicRepository(LingoLiftContext context)
    {
        _context = context;
    }
    
    public async Task<List<Topic>> GetAllTopicsAsync()
    {
        return await _context.Topics
            .OrderBy(t => t.Name)
            .ToListAsync();
    }
    
    public async Task<Topic?> GetTopicByIdAsync(int topicId)
    {
        return await _context.Topics.FindAsync(topicId);
    }
    
    public async Task<List<Topic>> GetTopPopularTopicsAsync(int count = 10)
    {
        return await _context.Topics
            .OrderByDescending(t => t.Popularity)
            .Take(count)
            .ToListAsync();
    }
    
    public async Task IncrementPopularityAsync(int topicId)
    {
        var topic = await _context.Topics.FindAsync(topicId);
        if (topic != null)
        {
            topic.Popularity++;
            await _context.SaveChangesAsync();
        }
    }
}