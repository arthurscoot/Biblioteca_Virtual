using AutoMapper;
using Domain.Exceptions;
using Library.DTOs;
using Library.Entities;
using Library.Interfaces;

namespace Library.Services
{
    public class AutorService : IAutorService
    {
        private readonly IAutorRepository _repository;
        private readonly IMapper _mapper;

        public AutorService(IAutorRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AutorDto>> ListarAsync(int page, int size)
        {
            var autores = await _repository.ListarPorPageTamanhoAsync(page, size);
            return _mapper.Map<IEnumerable<AutorDto>>(autores);
        }

        public async Task<AutorDto> BuscarAtivoPorIdAsync(int id)
        {
            var autor = await _repository.BuscarAtivoPorIdAsync(id);

            if (autor == null || !autor.Ativo)
                throw new NotFoundException("Autor não encontrado.");

            return _mapper.Map<AutorDto>(autor);
        }

        public async Task<AutorDto> CriarAsync(CreateAutorDto dto)
        {
            if (await _repository.ExisteAutorComMesmoNomeAsync(dto.Nome))
                throw new BusinessException("Já existe um autor com o mesmo nome.");

            var autor = new Autor(
                dto.Nome,
                dto.DataNascimento,
                dto.PaisOrigem,
                dto.Biografia
            );

            await _repository.AddAsync(autor);

            return _mapper.Map<AutorDto>(autor);
        }

        public async Task AtualizarAsync(int id, CreateAutorDto dto)
        {
            var autor = await _repository.BuscarAtivoPorIdAsync(id);

            if (autor == null)
                throw new NotFoundException("Autor não encontrado.");

            autor.Atualizar(dto.Nome, dto.DataNascimento, dto.PaisOrigem, dto.Biografia);

            await _repository.UpdateAsync(autor);
        }

        public async Task DesativarAsync(int id)
        {
            var autor = await _repository.BuscarAtivoPorIdAsync(id);

            if (autor == null)
                throw new NotFoundException("Autor não encontrado ou já está inativo.");

            autor.Desativar();
            await _repository.UpdateAsync(autor);
        }
    }
}
