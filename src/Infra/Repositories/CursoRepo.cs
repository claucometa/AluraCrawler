using AluraCrawler.Domain.Entities;
using AluraCrawler.Domain.Repositories;
using Dapper;
using System.Data;

public class CursoRepo(ILogger<CursoRepo> log, IDbConnection con) : ICursoRepo
{
    public async Task<bool> Exists(string link)
    {
        try
        {
            con.Open();
            var query = @"SELECT COUNT(*) FROM dbo.CursoAlura WHERE Link = @Link";
            return await con.ExecuteScalarAsync<int>(query, new { link }) > 0;
        }
        catch (Exception ex)
        {
            log.LogCritical("Db erro: {erro}", ex.Message);
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
        finally
        {
            con.Close();
        }

        return false;
    }

    public async Task<int> Add(CursoAlura curso)
    {
        try
        {
            con.Open();
            var query = @"INSERT INTO dbo.CursoAlura(Titulo, Instrutor, CargaHoraria, Descricao, Link) VALUES(@Titulo, @Instrutor, @CargaHoraria, @Descricao, @Link); 
                          SELECT CAST(SCOPE_IDENTITY() as INT);";

            return await con.ExecuteAsync(query, curso);
        }
        catch (Exception ex)
        {
            log.LogCritical("Db erro: {erro}", ex.Message);
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
        finally
        {
            con.Close();
        }

        return -1;
    }
}
