using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Stories.API.Contracts;
using Stories.API.Controllers;
using Stories.API.Models;
using Stories.API.Services;
using System.Net;

namespace Stories.API.Test
{
    public class StoriesControllerTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly StoriesController _controller;

        /// <summary>
        /// Initializing service collection and adding into DI.
        /// </summary>
        public StoriesControllerTests()
        {
            // Set up service collection
            var serviceCollection = new ServiceCollection();

            // Register services and dependencies
            serviceCollection.AddTransient<IStoryService, StoryService>();
            serviceCollection.AddSingleton<IMemoryCache, MemoryCache>();
            serviceCollection.AddHttpClient("Stories", c =>
            {
                c.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
            });

            // Build the service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            //Get Services from Service Collection
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();

            //Initialize Service class
            var storyService = new StoryService(httpClientFactory);
            
            //Intialize Controller class
            _controller = new StoriesController(storyService, memoryCache);
        }

        /// <summary>
        /// This will test GetAllTopStoriesAsync method from StroyController.
        /// </summary>
        /// <returns>Return Ok response with List of Top Stories</returns>
        [Fact]
        public async Task GetAllTopStoriesAsync_Return_Success()
        {
            // Arrange

            // Act
            var result = await _controller.GetAllTopStoriesAsync();
            var resultType = result as OkObjectResult;
            var resultList = resultType?.Value as List<Story>;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Story>>(resultList);
            Assert.Equal(500, resultList.Count);
        }

        /// <summary>
        /// This will test GetAllTopStoriesAsync method from StroyController and throws Bad Reques error.
        /// </summary>
        /// <returns>Return BadRequest response.</returns>
        [Fact]
        public async Task GetAllTopStoriesAsync_Return_BadRequest_Exception()
        {
            // Arrange
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var storyService = new StoryService(httpClientFactory);
            StoriesController _controller = new StoriesController(storyService, null);

            // Act
            var result = await _controller.GetAllTopStoriesAsync();
            var resultType = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest,resultType?.StatusCode);
            Assert.Equal("Error while getting top stories from server. Message : Object reference not set to an instance of an object.", resultType?.Value?.ToString());
        }

    }
}
