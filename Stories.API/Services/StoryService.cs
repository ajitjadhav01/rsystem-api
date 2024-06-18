using Stories.API.Contracts;
using Stories.API.Models;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Stories.API.Services;

/// <summary>
/// This is the service class responsible to perform story related operations.
/// </summary>
public class StoryService : IStoryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public StoryService(IHttpClientFactory httpClientFactory)
    {

        _httpClientFactory = httpClientFactory;

    }


    /// <summary>
    /// This method is responsible for getting all top storis from server.
    /// </summary>
    /// <returns>List of Top Stories Id's eg. [1234,56789] etc</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<List<Story>> GetAllTopStoriesAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("Stories");
            var httpResponseMessage = await httpClient.GetAsync(
                "topstories.json?print=pretty");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();

                var topStories = await JsonSerializer.DeserializeAsync
                    <List<long>>(contentStream);

                if (topStories == null && !topStories.Any())
                    return null;


                ParallelOptions parallelOptions = new()
                {
                    MaxDegreeOfParallelism = 100
                };

                ConcurrentBag<Story> storyDetails = new();

                await Parallel.ForEachAsync(topStories, parallelOptions, async (id, _) =>
                {
                    var storyDetail = await GetStoryByIdAsync(id);
                    storyDetails.Add(storyDetail);
                });


                return storyDetails.ToList();
            }

            return null;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// This method is responsible for getting Single Story by Id.
    /// </summary>
    /// <param name="id">Story Id</param>
    /// <returns>Story Details</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Story> GetStoryByIdAsync(long id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("Stories");
            var httpResponseMessage = await httpClient.GetAsync(
                $"item/{id}.json?print=pretty");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();

                var story = await JsonSerializer.DeserializeAsync
                    <Story>(contentStream);

                if (story != null) return story;
                else return null;
            }

            return null;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
