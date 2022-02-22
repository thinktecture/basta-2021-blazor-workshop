using AutoMapper;
using ConfTool.Shared.DTO;

namespace ConfTool.Server.Mappers
{
    public class ConferencesMapper : Profile
    {
        public ConferencesMapper()
        {
            CreateMap<Models.Conference, ConferenceOverview>();
            CreateMap<ConferenceOverview, Models.Conference>();
            CreateMap<Models.Conference, ConferenceDetails>();
            CreateMap<ConferenceDetails, Models.Conference>();
        }
    }
}


