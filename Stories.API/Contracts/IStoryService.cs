using Stories.API.Models;

namespace Stories.API.Contracts;

/// <summary>
/// This is a story service contract,
/// </summary>
public interface IStoryService
{
    Task<List<Story>> GetAllTopStoriesAsync();
    Task<Story> GetStoryByIdAsync(long id);
}
