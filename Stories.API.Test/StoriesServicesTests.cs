using Microsoft.Extensions.DependencyInjection;
using Stories.API.Contracts;
using Stories.API.Services;

namespace Stories.API.Test
{
    /// <summary>
    /// Stories Service Test Class
    /// </summary>
    public class StoriesServicesTests
    {

        private readonly ServiceProvider _serviceProvider;

        /// <summary>
        /// Initializing service collection and adding into DI.
        /// </summary>
        public StoriesServicesTests()
        {
            // Set up service collection
            var serviceCollection = new ServiceCollection();

            // Register services and dependencies

            serviceCollection.AddTransient<IStoryService, StoryService>();
            serviceCollection.AddHttpClient("Stories", c =>
            {
                c.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
            });

            // Build the service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// This will test GetAllTopStoriesAsync method from StoryService class.
        /// </summary>
        /// <returns>List of Top Stories</returns>
        [Fact]
        public async Task GetAllTopStoriesAsync_Success()
        {
            // Arrange
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var storyService = new StoryService(httpClientFactory);

            // Act
            var result = await storyService.GetAllTopStoriesAsync();

            // Assert
            Assert.NotEmpty(result);
            Assert.NotNull(result);
            Assert.Equal(500,result.Count);
        }

        /// <summary>
        /// This will test GetStoryByIdAsync method from StoryService class.
        /// </summary>
        /// <param name="id">Id of Story</param>
        /// <returns>Signle Story Object by Id</returns>
        [Theory]
        [InlineData(40660761)]
        public async Task GetStoryByIdAsync_Success(long id)
        {
            // Arrange
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var storyService = new StoryService(httpClientFactory);

            // Act
            var result = await storyService.GetStoryByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        /// <summary>
        /// This will test GetStoryByIdAsync method from StoryService class.
        /// </summary>
        /// <param name="id">Id of Story</param>
        /// <returns>return null</returns>
        [Theory]
        [InlineData(null)]
        public async Task GetStoryByIdAsync_Success_With_InValid_Id(long id)
        {
            // Arrange
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var storyService = new StoryService(httpClientFactory);

            // Act
            var result = await storyService.GetStoryByIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// This will test GetStoryByIdAsync method from StoryService class.
        /// </summary>
        /// <param name="id">Id of Story</param>
        /// <returns>return null</returns>
        [Theory]
        [InlineData(40660761)]
        public async Task GetStoryByIdAsync_Failure(long id)
        {
            // Arrange
            var storyService = new StoryService(null);

            // Act
            var result = await Assert.ThrowsAsync<NullReferenceException>(()=> storyService.GetStoryByIdAsync(id));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Object reference not set to an instance of an object.", result.Message);
        }
    }
}