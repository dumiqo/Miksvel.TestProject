using AutoMapper;
using Miksvel.TestProject.Core.Model;
using Miksvel.TestProject.ProviderTwo.Model;
using Xunit;

namespace Miksvel.TestProject.ProviderTwo.Test
{
    public class AutoMapperTest
    {
        private IMapper _mapper;
        private MapperConfiguration _config;

        public AutoMapperTest()
        {
            _config = new MapperConfiguration(cfg => cfg.AddProfile<ProviderTwoProfile>());
            _mapper = _config.CreateMapper();
        }

        [Fact]
        public void Map_FromSearchRequest_ToProviderTwoSearchRequest_SuccessTest()
        {
            var startTime = new DateTime(2001, 1, 1);
            var from = new SearchRequest
            {
                Origin = "Moscow",
                Destination = "Sochi",
                OriginDateTime = startTime,
                Filters = new SearchFilters
                {
                    MinTimeLimit = startTime.AddHours(1)
                }
            };
            var to = _mapper.Map<ProviderTwoSearchRequest>(from);

            Assert.Equal(from.Origin, to.Departure);
            Assert.Equal(from.Destination, to.Arrival);
            Assert.Equal(from.OriginDateTime, to.DepartureDate);
            Assert.Equal(from.Filters.MinTimeLimit, to.MinTimeLimit);
        }

        [Fact]
        public void Map_FromSearchRequest_ToProviderTwoSearchRequest_NullableFilter()
        {
            var startTime = new DateTime(2001, 1, 1);
            var from = new SearchRequest
            {
                Origin = "Moscow",
                Destination = "Sochi",
                OriginDateTime = startTime
            };
            var to = _mapper.Map<ProviderTwoSearchRequest>(from);

            Assert.Equal(from.Origin, to.Departure);
            Assert.Equal(from.Destination, to.Arrival);
            Assert.Equal(from.OriginDateTime, to.DepartureDate);
            Assert.Null(to.MinTimeLimit);
        }

        [Fact]
        public void Map_FromProviderTwoRoute_ToRoute_SuccessTest()
        {
            var startTime = new DateTime(2001, 1, 1);
            var from = new ProviderTwoRoute
            {
                Departure = new ProviderTwoPoint
                {
                    Point = "Moscow",
                    Date = startTime
                },
                Arrival = new ProviderTwoPoint
                {
                    Point = "Sochi",
                    Date = startTime.AddDays(1)
                },
                TimeLimit = startTime.AddHours(1),
                Price = 12333
            };
            var to = _mapper.Map<Route>(from);

            Assert.Equal(from.Departure.Point, to.Origin);
            Assert.Equal(from.Departure.Date, to.OriginDateTime);
            Assert.Equal(from.Arrival.Point, to.Destination);
            Assert.Equal(from.Arrival.Date, to.DestinationDateTime);
            Assert.Equal(from.Price, to.Price);
            Assert.Equal(from.TimeLimit, to.TimeLimit);
        }
    }
}
