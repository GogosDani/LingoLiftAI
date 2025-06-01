using Backend.Models;

namespace Backend.Services.Repositories;

public interface ITopicRepository
{
    Task<List<Topic>> GetAllTopicsAsync();
    Task<Topic?> GetTopicByIdAsync(int topicId);
    Task<List<Topic>> GetTopPopularTopicsAsync(int count = 10);
    Task IncrementPopularityAsync(int topicId);
}