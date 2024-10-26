using AluraCrawler.Domain.Entities;
using AluraCrawler.Domain.Repositories;
using AluraCrawler.Domain.Services;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace AluraCrawler.Infra.Services
{
    public class AlureService(IWebDriver driver, ILogger<AlureService> logger, ICursoRepo repo) : IAlureService
    {
        Regex re = new Regex(@"\d+");        

        public async Task Run(CancellationToken stoppingToken)
        {
            // Define os criterios de busca
            string keywords = "RPA .net".Replace(" ", "+");
            string filters = "typeFilters=COURSE";

            List<string> links = [];

            // Primeira etapa, encontra o link dos cursos atraves do criterio de busca
            try
            {
                driver.Navigate().GoToUrl($"https://www.alura.com.br/busca?query={keywords}&{filters}");

                var element = driver.FindElement(By.Id("header-barraBusca-form-campoBusca"));

                var results = driver.FindElements(By.XPath("//*[@id=\"busca-resultados\"]/ul/li/a"));

                foreach (var item in results)
                {
                    var link = item.GetAttribute("href");
                    links.Add(link);
                }
            }
            catch
            {
                logger.LogCritical("Falha na pesquisa {keywords}", keywords);
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                return;
            }

            // Segunda etapa, alimenta os cursos
            foreach (var link in links)
            {
                if (await repo.Exists(link))
                {
                    logger.LogInformation("Esse curso já foi adicionado {link}", link);
                    continue;
                }

                try
                {
                    driver.Navigate().GoToUrl(link);
                }
                catch
                {
                    logger.LogCritical("Erro ao acessar o link {link}", link);
                    continue;
                }

                if (driver.PageSource.Contains("Acho que nos perdemos..."))
                {
                    logger.LogError("Link quebrado {link}", link);
                    continue;
                }

                var cargaHoraria = FindE("formacao__info-destaque", "courseInfo-card-wrapper-infos");
                var titulo = FindE("formacao-headline-titulo", "curso-banner-course-title");
                var descricao = FindE("formacao-headline-subtitulo", "course--banner-text-category");

                if (cargaHoraria is null)
                    logger.LogError("Falha ao obter carga horaria {link}", link);

                if (titulo is null)
                    logger.LogError("Falha ao obter titulo {link}", link);

                if (descricao is null)
                    logger.LogError("Falha ao obter descricao {link}", link);

                if (cargaHoraria is null || titulo is null || descricao is null)
                    continue;

                if(!int.TryParse(re.Match(cargaHoraria)?.Value, out int ch))
                {
                    logger.LogError("Falha ao converter cargaHoraria {cargaHoraira} {link}", cargaHoraria, link);
                    continue;
                }

                var curso = new CursoAlura() { Link = link, Titulo = titulo, CargaHoraria = ch, Descricao = descricao };

                try
                {
                    var instrutor = driver.FindElement(By.ClassName("instructor-title--name")).Text;
                    if (!string.IsNullOrEmpty(instrutor))
                        curso.Instrutor = instrutor;
                }
                catch
                {
                    logger.LogError("Falha na obtenção do instrutor {link}", link);
                }

                logger.LogInformation("Curso encontrado {titulo} {carga} Instrutor {total}", curso.Titulo, curso.CargaHoraria, curso.Instrutor);

                var rows = await repo.Add(curso);
            }

            // Close the browser
            try
            {
                driver.Quit();
            }
            catch
            {
                logger.LogError("Falha ao fechar o navegador");
            }
        }

        string? FindE(string v1, string v2)
        {
            try
            {
                return driver.FindElement(By.ClassName(v1)).Text;
            }
            catch (Exception)
            {
            }

            try
            {
                return driver.FindElement(By.ClassName(v2)).Text;
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
