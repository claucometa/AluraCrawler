using AluraCrawler.Domain.Entities;

namespace AluraCrawler.Domain.Repositories
{
    public interface ICursoRepo
    {
        Task<bool> Exists(string link);
        Task<int> Add(CursoAlura curso);
    }
}