using System.ComponentModel.DataAnnotations;

namespace AluraCrawler.Domain.Entities
{
    public class CursoAlura
    {
        [Key]
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string? Instrutor { get; set; }
        public int CargaHoraria { get; set; }
        public string? Descricao { get; set; }
        public string Link { get; set; }
    }
}
