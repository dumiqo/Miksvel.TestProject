using AutoMapper;
using Miksvel.TestProject.Core.Model;
using Miksvel.TestProject.ProviderTwo.Model;

namespace Miksvel.TestProject.ProviderTwo
{
    public class ProviderTwoProfile : Profile
    {
        public ProviderTwoProfile()
        {
            CreateMap<SearchRequest, ProviderTwoSearchRequest>()
                .ForMember(x => x.Departure, x => x.MapFrom(y => y.Origin))
                .ForMember(x => x.DepartureDate, x => x.MapFrom(y => y.OriginDateTime))
                .ForMember(x => x.Arrival, x => x.MapFrom(y => y.Destination))
                .ForMember(x => x.MinTimeLimit, x => x.MapFrom(y => y.Filters != null ? y.Filters.MinTimeLimit : null));

            CreateMap<ProviderTwoRoute, Route>()
                .ForMember(x => x.Origin, x => x.MapFrom(y => y.Departure.Point))
                .ForMember(x => x.OriginDateTime, x => x.MapFrom(y => y.Departure.Date))
                .ForMember(x => x.Destination, x => x.MapFrom(y => y.Arrival.Point))
                .ForMember(x => x.DestinationDateTime, x => x.MapFrom(y => y.Arrival.Date))
                .ForMember(x => x.Price, x => x.MapFrom(y => y.Price))
                .ForMember(x => x.TimeLimit, x => x.MapFrom(y => y.TimeLimit))
                .BeforeMap((pr, r) => r.Id = Guid.NewGuid());
        }
    }
}
