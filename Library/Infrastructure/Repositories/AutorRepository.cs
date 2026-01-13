using Library.Data;
using Library.Entities;
using Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories
{
    public class AutorRepository : IAutorRepository
    {
        private readonly AppDbContext _context;

        public AutorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Autor autor)
        {
            await _context.Autor.AddAsync(autor);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Autor>> ListarPorPageTamanhoAsync(int page, int size)
        {
            return await _context.Autor
                .Where(a => a.Ativo)
                .OrderBy(a => a.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }

        public async Task<Autor?> BuscarPorIdAsync(int id)
        {
            return await _context.Autor.FindAsync(id);
        }

        public async Task<bool> ExisteAutorComMesmoNomeAsync(string nome)
        {
            return await _context.Autor.AnyAsync(a => a.Nome == nome);
        }

        public async Task UpdateAsync(Autor autor)
        {
            _context.Autor.Update(autor);
            await _context.SaveChangesAsync();
        }
    }
}