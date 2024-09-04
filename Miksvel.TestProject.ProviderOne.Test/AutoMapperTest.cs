using AutoMapper;
using Miksvel.TestProject.Core.Model;
using Miksvel.TestProject.ProviderOne.Model;
using Xunit;

namespace Miksvel.TestProject.ProviderOne.Test
{
    public class AutoMapperTest
    {
        private IMapper _mapper;
        private MapperConfiguration _config;

        public AutoMapperTest()
        {
            _config = new MapperConfiguration(cfg => cfg.AddProfile<ProviderOneProfile>());
            _mapper = _config.CreateMapper();
        }

        [Fact]
        public void Map_FromSearchRequest_ToProviderOneSearchRequest_SuccessTest()
        {
            var startTime = new DateTime(2001, 1, 1);
            var from = new SearchRequest
            {
                Origin = "Moscow",
                Destination = "Sochi",
                OriginDateTime = startTime,
                Filters = new SearchFilters
                {
                    DestinationDateTime = startTime.AddHours(1),
                    MaxPrice = 300
                }
            };
            var to = _mapper.Map<ProviderOneSearchRequest>(from);

            Assert.Equal(from.Origin, to.From);
            Assert.Equal(from.Destination, to.To);
            Assert.Equal(from.OriginDateTime, to.DateFrom);
            Assert.Equal(from.Filters.DestinationDateTime, to.DateTo);
            Assert.Equal(from.Filters.MaxPrice, to.MaxPrice);
        }

        [Fact]
        public void Map_FromSearchRequest_ToProviderOneSearchRequest_NullableFilter()
        {
            var startTime = new DateTime(2001, 1, 1);
            var from = new SearchRequest
            {
                Origin = "Moscow",
                Destination = "Sochi",
                OriginDateTime = startTime
            };
            var to = _mapper.Map<ProviderOneSearchRequest>(from);

            Assert.Equal(from.Origin, to.From);
            Assert.Equal(from.Destination, to.To);
            Assert.Equal(from.OriginDateTime, to.DateFrom);
        }

        [Fact]
        public void Map_FromProviderOneRoute_ToRoute_SuccessTest()
        {
            var startTime = new DateTime(2001, 1, 1);
            var from = new ProviderOneRoute
            {
                From = "Moscow",
                DateFrom = startTime,
                To = "Sochi",
                DateTo = startTime.AddDays(1),
                TimeLimit = startTime.AddHours(1),
                Price = 12333
            };
            var to = _mapper.Map<Route>(from);

            Assert.Equal(from.From, to.Origin);
            Assert.Equal(from.DateFrom, to.OriginDateTime);
            Assert.Equal(from.To, to.Destination);
            Assert.Equal(from.DateTo, to.DestinationDateTime);
            Assert.Equal(from.Price, to.Price);
            Assert.Equal(from.TimeLimit, to.TimeLimit);
        }
    }
}
