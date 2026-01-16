using AutoMapper;
using Library.DTOs;
using Library.Entities;

namespace Library.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, UsuarioDTO>();
            CreateMap<CreateUsuarioDTO, Usuario>();

            CreateMap<Autor, AutorDto>();
            CreateMap<CreateAutorDto, Autor>();

            CreateMap<Livro, LivroDTO>()
                .ForMember(dest => dest.NomeAutor, opt => opt.MapFrom(src => src.Autor != null ? src.Autor.Nome : string.Empty));
            CreateMap<CreateLivroDTO, Livro>();

            CreateMap<Emprestimo, EmprestimoDTO>();
        }
    }
}