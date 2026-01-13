using AutoMapper;
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

        public async Task<AutorDto?> BuscarPorIdAsync(int id)
        {
            var autor = await _repository.BuscarPorIdAsync(id);

            if (autor == null || !autor.Ativo)
                return null;

            return _mapper.Map<AutorDto>(autor);
        }

        public async Task<AutorDto> CriarAsync(CreateAutorDto dto)
        {
         
            if (dto.DataNascimento.Date > DateTime.Today)
                throw new Exception("A data de nascimento não pode ser maior que a data atual.");

 
            var hoje = DateTime.Today;
            var idade = hoje.Year - dto.DataNascimento.Year;

            if (dto.DataNascimento.Date > hoje.AddYears(-idade))
                idade--;

            if (idade < 16)
                throw new Exception("O autor deve ter no mínimo 16 anos.");

            if (await _repository.ExisteAutorComMesmoNomeAsync(dto.Nome))
                throw new Exception("Já existe um autor com o mesmo nome.");

            var autor = _mapper.Map<Autor>(dto);

            await _repository.AddAsync(autor);

            return _mapper.Map<AutorDto>(autor);
        }

        public async Task<bool> AtualizarAsync(int id, CreateAutorDto dto)
        {
            var autor = await _repository.BuscarPorIdAsync(id);

            if (autor == null)
                return false;
            if (dto.DataNascimento.Date > DateTime.Today)
                throw new Exception("A data de nascimento não pode ser maior que a data atual.");

            var hoje = DateTime.Today;
            var idade = hoje.Year - dto.DataNascimento.Year;

            if (dto.DataNascimento.Date > hoje.AddYears(-idade))
                idade--;

            if (idade < 16)
                throw new Exception("O autor deve ter no mínimo 16 anos.");

            autor.Nome = dto.Nome;
            autor.DataNascimento = dto.DataNascimento;
            autor.PaisOrigem = dto.PaisOrigem;
            autor.Biografia = dto.Biografia;

            await _repository.UpdateAsync(autor);

            return true;
        }

        public async Task<bool> DesativarAsync(int id)
        {
            var autor = await _repository.BuscarPorIdAsync(id);
            
            if (autor == null || !autor.Ativo) {
                return false;
            }
            autor.Ativo = false; 
            
            await _repository.UpdateAsync(autor);
            return true;
        }
    }
}
