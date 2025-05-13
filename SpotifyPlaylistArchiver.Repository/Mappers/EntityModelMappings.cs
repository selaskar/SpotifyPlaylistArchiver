using AutoMapper;

namespace SpotifyPlaylistArchiver.Repository.Mappers;

internal class EntityModelMappings : Profile
{
    public EntityModelMappings()
    {
        CreateMap<Models.SavedTrack, Entities.Track>()
            .ForMember(x => x.Name, x => x.MapFrom(y => y.Track.Name))
            .ForMember(x => x.SpotifyId, x => x.MapFrom(y => y.Track.Id))
            .ForMember(x => x.AddedAt, x => x.MapFrom(y => y.AddedAt))
            .ForMember(x => x.Album, x => x.MapFrom(y => y.Track.Album))
            .ForMember(x => x.Id, x => x.MapFrom(y => Guid.NewGuid()))
            .ForMember(x => x.AlbumId, x => x.Ignore())
            .ForMember(x => x.Artists, x => x.Ignore())
            .ForMember(x => x.RemovedAt, x => x.Ignore());

        CreateMap<Models.Album, Entities.Album>()
            .ForMember(x => x.SpotifyId, x => x.MapFrom(y => y.Id))
            .ForMember(x => x.Name, x => x.MapFrom(y => y.Name))
            .ForMember(x => x.Id, x => x.MapFrom(y => Guid.NewGuid()))
            .ForMember(x => x.Artists, x => x.Ignore())
            .ForMember(x => x.AddedAt, x => x.Ignore())
            .ForMember(x => x.RemovedAt, x => x.Ignore());

        CreateMap<Models.Artist, Entities.Artist>()
            .ForMember(x => x.SpotifyId, x => x.MapFrom(y => y.Id))
            .ForMember(x => x.Name, x => x.MapFrom(y => y.Name))
            .ForMember(x => x.Id, x => x.MapFrom(y => Guid.NewGuid()))
            .ForMember(x => x.AddedAt, x => x.Ignore())
            .ForMember(x => x.RemovedAt, x => x.Ignore());
    }
}
