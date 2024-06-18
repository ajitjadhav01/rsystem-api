using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Stories.API.Contracts;
using Stories.API.Models;

namespace Stories.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoriesController : ControllerBase
    {

        private const string StoriesCacheKey = "Stories";

        private readonly IStoryService _storyService;
        private readonly IMemoryCache _memoryCache;

        public StoriesController(
            IStoryService storyService,
            IMemoryCache memoryCache)
        {
            _storyService = storyService;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopStoriesAsync() 
        {
            try
            {
                //If values is present in cache then return it from cache
                //else make server call and retrieve data from api and store in cache
                if (_memoryCache.TryGetValue(StoriesCacheKey, out List<Story> stories))
                    return Ok(stories);

                //Get data from api.
                var res = await _storyService.GetAllTopStoriesAsync();

                //Cache configuration
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1024);

                //Add data to cache
                _memoryCache.Set(StoriesCacheKey, res, cacheEntryOptions);

                return Ok(res);
            }
            catch(Exception ex)
            {
                //We can log excepition here in file, database etc based on requirement.
                //ex.Message, ex.StackTrace etc
                return BadRequest($"Error while getting top stories from server. Message : {ex.Message}");
            }
        }

    }
}
