using AutoMapper;
using Library.DTOs;
using Library.Entities;

namespace Library.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Usuario
            CreateMap<Usuario, UsuarioDTO>();
            CreateMap<CreateUsuarioDTO, Usuario>();

            // Autor
            CreateMap<Autor, AutorDto>();
            CreateMap<CreateAutorDto, Autor>();

            // Livro
            CreateMap<Livro, LivroDTO>()
                .ForMember(dest => dest.NomeAutor, opt => opt.MapFrom(src => src.Autor != null ? src.Autor.Nome : string.Empty));
            CreateMap<CreateLivroDTO, Livro>();
        }
    }
}