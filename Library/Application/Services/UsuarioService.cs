using AutoMapper;
using Library.DTOs;
using Library.Entities;
using Library.Interfaces;

namespace Library.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UsuarioDTO> CriarAsync(CreateUsuarioDTO dto)
        {
            if (await _repository.ExisteCpfAsync(dto.Cpf))
                throw new InvalidOperationException("Já existe um usuário cadastrado com este CPF.");

            if (await _repository.ExisteEmailAsync(dto.Email))
                throw new InvalidOperationException("Já existe um usuário cadastrado com este e-mail.");

            var usuario = _mapper.Map<Usuario>(dto);
            usuario.DatadeCadastro = DateTime.UtcNow;
            usuario.Ativo = true;

            await _repository.AddAsync(usuario);

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<IEnumerable<UsuarioDTO>> ListarAtivosAsync()
        {
            var usuarios = await _repository.ListarAtivosAsync();

            return _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
        }

        public async Task<UsuarioDTO?> BuscarPorCpfAsync(string cpf)
        {
            var usuario = await _repository.BuscarPorCpfAsync(cpf);

            return usuario == null ? null : _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<bool> AtualizarAsync(int id, CreateUsuarioDTO dto)
        {
            var usuario = await _repository.BuscarPorIdAsync(id);

            if (usuario == null || !usuario.Ativo)
                return false;

            if (await _repository.ExisteCpfEmOutroUsuarioAsync(id, dto.Cpf))
                throw new InvalidOperationException("O CPF informado já está em uso por outro usuário.");

            if (await _repository.ExisteEmailEmOutroUsuarioAsync(id, dto.Email))
                throw new InvalidOperationException("O e-mail informado já está em uso por outro usuário.");

            usuario.Nome = dto.Nome;
            usuario.Cpf = dto.Cpf;
            usuario.Email = dto.Email;
            usuario.Telefone = dto.Telefone;

            await _repository.UpdateAsync(usuario);
            return true;
        }

        public async Task<bool> DesativarAsync(int id)
        {
            var usuario = await _repository.BuscarPorIdAsync(id);

            if (usuario == null || !usuario.Ativo)
                return false;

            usuario.Ativo = false;

            await _repository.UpdateAsync(usuario);
            return true;
        }
    }
}
