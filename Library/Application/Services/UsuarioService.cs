using AutoMapper;
using Domain.Exceptions;
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
                throw new BusinessException("Já existe um usuário cadastrado com este CPF.");

            if (await _repository.ExisteEmailAsync(dto.Email))
                throw new BusinessException("Já existe um usuário cadastrado com este e-mail.");

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

        public async Task<UsuarioDTO> BuscarPorCpfAsync(string cpf)
        {
            var usuario = await _repository.BuscarPorCpfAsync(cpf);

            if (usuario == null)
                throw new NotFoundException("Usuário não encontrado.");

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task AtualizarAsync(int id, CreateUsuarioDTO dto)
        {
            var usuario = await _repository.BuscarPorIdAsync(id);

            if (usuario == null || !usuario.Ativo)
                throw new NotFoundException("Usuário não encontrado ou inativo.");

            if (await _repository.ExisteCpfEmOutroUsuarioAsync(id, dto.Cpf))
                throw new BusinessException("O CPF informado já está em uso por outro usuário.");

            if (await _repository.ExisteEmailEmOutroUsuarioAsync(id, dto.Email))
                throw new BusinessException("O e-mail informado já está em uso por outro usuário.");

            usuario.Nome = dto.Nome;
            usuario.Cpf = dto.Cpf;
            usuario.Email = dto.Email;
            usuario.Telefone = dto.Telefone;

            await _repository.UpdateAsync(usuario);
        }

        public async Task DesativarAsync(int id)
        {
            var usuario = await _repository.BuscarPorIdAsync(id);

            if (usuario == null || !usuario.Ativo)
                throw new NotFoundException("Usuário não encontrado ou já está inativo.");

            usuario.Ativo = false;

            await _repository.UpdateAsync(usuario);
        }
    }
}
