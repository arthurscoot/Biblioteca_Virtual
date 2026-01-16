using Library.Entities;

namespace Library.Interfaces;
public interface IEstatisticaService
{   
Task<IEnumerable<TopLivroDTO>> ObterTopLivrosAsync();

Task<IEnumerable<TopAutorDTO>> ObterTopAutoresAsync();

}