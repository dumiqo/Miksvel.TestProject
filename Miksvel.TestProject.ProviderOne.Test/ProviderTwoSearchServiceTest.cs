using AutoMapper;
using Miksvel.TestProject.Core.Model;
using Miksvel.TestProject.ProviderOne.Model;
using NSubstitute;
using Xunit;

namespace Miksvel.TestProject.ProviderOne.Test
{
    public class ProviderOneSearchServiceTest
    {
        private IMapper _mapper;
        private MapperConfiguration _config;

        public ProviderOneSearchServiceTest()
        {
            _config = new MapperConfiguration(cfg => cfg.AddProfile<ProviderOneProfile>());
            _mapper = _config.CreateMapper();
        }

        [Fact]
        public async Task IsAvailableAsync_WhenServiceAvailable_ShouldReturnTrue()
        {
            var client = Substitute.For<IProviderOneClient>();

            client.IsAvailableAsync(default).Returns(true);
            var service = new ProviderOneSearchService(_mapper, client);
            Assert.True(await service.IsAvailableAsync(default));
        }

        [Fact]
        public async Task IsAvailableAsync_WhenServiceUnAvailable_ShouldReturnFalse()
        {
            var client = Substitute.For<IProviderOneClient>();

            client.IsAvailableAsync(default).Returns(false);
            var service = new ProviderOneSearchService(_mapper, client);
            Assert.False(await service.IsAvailableAsync(default));
        }

        [Fact]
        public async Task SearchAsync_WithFilter_ShouldFilter()
        {
            var client = Substitute.For<IProviderOneClient>();

            var startDate = DateTime.UtcNow;

            client.SearchAsync(Arg.Any<ProviderOneSearchRequest>(), default)
                .Returns(Task.FromResult(new ProviderOneSearchResponse()
                {
                    Routes = [
                        new ProviderOneRoute{
                            From = "Msk",
                            DateFrom = startDate.AddHours(6),
                            To = "SPB",
                            DateTo = startDate.AddHours(7),
                            Price = 90,
                            TimeLimit = startDate.AddMinutes(37)
                        },
                        new ProviderOneRoute{
                            From = "Msk",
                            DateFrom = startDate.AddHours(7),
                            To = "SPB",
                            DateTo = startDate.AddHours(9),
                            Price = 50,
                            TimeLimit = startDate.AddMinutes(31)
                        },
                        new ProviderOneRoute{
                            From = "Msk",
                            DateFrom = startDate.AddHours(6),
                            To = "SPB",
                            DateTo = startDate.AddHours(7),
                            Price = 10,
                            TimeLimit = startDate.AddMinutes(37)
                        },
                        new ProviderOneRoute{
                            From = "Msk",
                            DateFrom = startDate.AddHours(6),
                            To = "SPB",
                            DateTo = startDate.AddHours(17),
                            Price = 60,
                            TimeLimit = startDate.AddMinutes(17)
                        }
                    ]
                }));

            var request = new SearchRequest()
            {
                Origin = "Msk",
                Destination = "SPB",
                OriginDateTime = startDate,
                Filters = new SearchFilters
                {
                    DestinationDateTime = startDate.AddHours(12),
                    MaxPrice = 100,
                    MinTimeLimit = startDate.AddMinutes(30),
                    OnlyCached = false
                }
            };
            var service = new ProviderOneSearchService(_mapper, client);
            var response = await service.SearchAsync(request, default);

            Assert.Equal(3, response.Routes.Length);

            Assert.Equal(10, response.MinPrice);
            Assert.Equal(90, response.MaxPrice);
            Assert.Equal(60, response.MinMinutesRoute);
            Assert.Equal(120, response.MaxMinutesRoute);

            Assert.Equal("Msk", response.Routes[0].Origin);
            Assert.Equal("SPB", response.Routes[0].Destination);
            Assert.Equal(startDate.AddHours(6), response.Routes[0].OriginDateTime);
            Assert.Equal(startDate.AddHours(7), response.Routes[0].DestinationDateTime);
            Assert.Equal(90, response.Routes[0].Price);
            Assert.Equal(startDate.AddMinutes(37), response.Routes[0].TimeLimit);

            Assert.Equal("Msk", response.Routes[1].Origin);
            Assert.Equal("SPB", response.Routes[1].Destination);
            Assert.Equal(startDate.AddHours(7), response.Routes[1].OriginDateTime);
            Assert.Equal(startDate.AddHours(9), response.Routes[1].DestinationDateTime);
            Assert.Equal(50, response.Routes[1].Price);
            Assert.Equal(startDate.AddMinutes(31), response.Routes[1].TimeLimit);
        }

        [Fact]
        public async Task SearchAsync_WithoutFilter_ShouldIgnoreFilter()
        {
            var client = Substitute.For<IProviderOneClient>();

            var startDate = DateTime.UtcNow;

            client.SearchAsync(Arg.Any<ProviderOneSearchRequest>(), default)
                .Returns(Task.FromResult(new ProviderOneSearchResponse()
                {
                    Routes = [
                        new ProviderOneRoute{
                            From = "Msk",
                            DateFrom = startDate.AddHours(6),
                            To = "SPB",
                            DateTo = startDate.AddHours(7),
                            Price = 90,
                            TimeLimit = startDate.AddMinutes(37)
                        },
                        new ProviderOneRoute{
                            From = "Msk",
                            DateFrom = startDate.AddHours(7),
                            To = "SPB",
                            DateTo = startDate.AddHours(9),
                            Price = 50,
                            TimeLimit = startDate.AddMinutes(31)
                        },
                        new ProviderOneRoute{
                            From = "Msk",
                            DateFrom = startDate.AddHours(6),
                            To = "SPB",
                            DateTo = startDate.AddHours(7),
                            Price = 190,
                            TimeLimit = startDate.AddMinutes(37)
                        },
                        new ProviderOneRoute{
                            From = "Msk",
                            DateFrom = startDate.AddHours(6),
                            To = "SPB",
                            DateTo = startDate.AddHours(13),
                            Price = 10,
                            TimeLimit = startDate.AddMinutes(7)
                        }
                    ]
                }));

            var request = new SearchRequest()
            {
                Origin = "Msk",
                Destination = "SPB",
                OriginDateTime = startDate
            };
            var service = new ProviderOneSearchService(_mapper, client);
            var response = await service.SearchAsync(request, default);

            Assert.Equal(4, response.Routes.Length);

            Assert.Equal(10, response.MinPrice);
            Assert.Equal(190, response.MaxPrice);
            Assert.Equal(60, response.MinMinutesRoute);
            Assert.Equal(420, response.MaxMinutesRoute);

            Assert.Equal("Msk", response.Routes[0].Origin);
            Assert.Equal("SPB", response.Routes[0].Destination);
            Assert.Equal(startDate.AddHours(6), response.Routes[0].OriginDateTime);
            Assert.Equal(startDate.AddHours(7), response.Routes[0].DestinationDateTime);
            Assert.Equal(90, response.Routes[0].Price);
            Assert.Equal(startDate.AddMinutes(37), response.Routes[0].TimeLimit);

            Assert.Equal("Msk", response.Routes[1].Origin);
            Assert.Equal("SPB", response.Routes[1].Destination);
            Assert.Equal(startDate.AddHours(7), response.Routes[1].OriginDateTime);
            Assert.Equal(startDate.AddHours(9), response.Routes[1].DestinationDateTime);
            Assert.Equal(50, response.Routes[1].Price);
            Assert.Equal(startDate.AddMinutes(31), response.Routes[1].TimeLimit);
        }
    }
}
