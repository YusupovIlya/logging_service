using AutoMapper;
using Logging.Server.Models.StreamData.Api;
using Logging.Server.Models.StreamData.Api.Aggregations;
using Logging.Server.Service.StreamData.Models;
using Newtonsoft.Json;

namespace Logging.Server.Service.StreamData.Configuration.MapperProfiles
{
#pragma warning disable 1591
    public class StreamDataProfile : Profile
    {
        public StreamDataProfile()
        {
            CreateMap<BaseStreamDataEvent, StreamDataEventViewModel>()
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => JsonConvert.DeserializeObject(src.RawJson)));
                //.ForMember(dest => dest.Labels,
                //    opt => opt.MapFrom(src =>
                //        !string.IsNullOrWhiteSpace(src.LabelsRawJson) && src.LabelsRawJson != "{}"
                //            ? JsonConvert.DeserializeObject(src.LabelsRawJson)
                //            : null));

            CreateMap<StreamDataAggregateModel, StreamDataAggregateViewModel>();
            CreateMap<AggregationResultModel, AggregationResultViewModel>();
            CreateMap<Alias, AliasViewModel>();
            CreateMap<AliasViewModel, Alias>();
        }
    }
}
