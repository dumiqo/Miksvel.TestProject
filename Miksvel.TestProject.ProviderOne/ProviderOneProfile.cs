using AutoMapper;
using Miksvel.TestProject.Core.Model;
using Miksvel.TestProject.ProviderOne.Model;

namespace Miksvel.TestProject.ProviderOne
{
    public class ProviderOneProfile : Profile
    {
        public ProviderOneProfile()
        {
            CreateMap<SearchRequest, ProviderOneSearchRequest>()
                .ForMember(x => x.From, x => x.MapFrom(y => y.Origin))
                .ForMember(x => x.DateFrom, x => x.MapFrom(y => y.OriginDateTime))
                .ForMember(x => x.To, x => x.MapFrom(y => y.Destination))
                .ForMember(x => x.MaxPrice, x => x.MapFrom(y => y.Filters != null ? y.Filters.MaxPrice : null))
                .ForMember(x => x.DateTo, x => x.MapFrom(y => y.Filters != null ? y.Filters.DestinationDateTime : null));

            CreateMap<ProviderOneRoute, Route>()
                .ForMember(x => x.Origin, x => x.MapFrom(y => y.From))
                .ForMember(x => x.OriginDateTime, x => x.MapFrom(y => y.DateFrom))
                .ForMember(x => x.Destination, x => x.MapFrom(y => y.To))
                .ForMember(x => x.DestinationDateTime, x => x.MapFrom(y => y.DateTo))
                .ForMember(x => x.Price, x => x.MapFrom(y => y.Price))
                .ForMember(x => x.TimeLimit, x => x.MapFrom(y => y.TimeLimit))
                .BeforeMap((pr, r) => r.Id = Guid.NewGuid());
        }
    }
}
