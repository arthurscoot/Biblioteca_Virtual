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
            ValidarIdade(dto.DataNascimento);

            if (await _repository.ExisteAutorComMesmoNomeAsync(dto.Nome))
                throw new BusinessException("Já existe um autor com o mesmo nome.");

            var autor = _mapper.Map<Autor>(dto);
            await _repository.AddAsync(autor);

            return _mapper.Map<AutorDto>(autor);
        }

        public async Task AtualizarAsync(int id, CreateAutorDto dto)
        {
            var autor = await _repository.BuscarAtivoPorIdAsync(id);

            if (autor == null)
                throw new NotFoundException("Autor não encontrado.");

            ValidarIdade(dto.DataNascimento);

            autor.Nome = dto.Nome;
            autor.DataNascimento = dto.DataNascimento;
            autor.PaisOrigem = dto.PaisOrigem;
            autor.Biografia = dto.Biografia;

            await _repository.UpdateAsync(autor);
        }

        public async Task DesativarAsync(int id)
        {
            var autor = await _repository.BuscarAtivoPorIdAsync(id);

            if (autor == null)
                throw new NotFoundException("Autor não encontrado ou já está inativo.");

            autor.Ativo = false;
            await _repository.UpdateAsync(autor);
        }

        private static void ValidarIdade(DateTime dataNascimento)
        {
            if (dataNascimento.Date > DateTime.Today)
                throw new ValidationException("A data de nascimento não pode ser futura.");

            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Year;

            if (dataNascimento.Date > hoje.AddYears(-idade))
                idade--;

            if (idade < 16)
                throw new BusinessException("O autor deve ter no mínimo 16 anos.");
        }
    }
}
