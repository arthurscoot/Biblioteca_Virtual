using Library.Data;
using Library.Entities;
using Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteCpfAsync(string cpf)
        {
            return await _context.Usuario.AnyAsync(u => u.Cpf == cpf);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _context.Usuario.AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Usuario>> ListarAtivosAsync()
        {
            return await _context.Usuario
                .Where(u => u.Ativo)
                .ToListAsync();
        }

        public async Task<Usuario?> BuscarPorCpfAsync(string cpf)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Cpf == cpf && u.Ativo);
        }

        public async Task<Usuario?> BuscarPorIdAsync(int id)
        {
            return await _context.Usuario.FindAsync(id);
        }

        public async Task<bool> ExisteCpfEmOutroUsuarioAsync(int id, string cpf)
        {
            return await _context.Usuario
                .AnyAsync(u => u.Id != id && u.Cpf == cpf);
        }

        public async Task<bool> ExisteEmailEmOutroUsuarioAsync(int id, string email)
        {
            return await _context.Usuario
                .AnyAsync(u => u.Id != id && u.Email == email);
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
