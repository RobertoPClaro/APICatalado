using System.ComponentModel.DataAnnotations;

namespace APICatalogo.Models;
public class Categoria
{
    [Key]
    public int CategoriaId { get; set; }
    [Required(ErrorMessage = "Nome obrigatorio")]
    [StringLength(80, ErrorMessage = "Tamanho maximo do 80 caracteres")]
    public string? Nome { get; set; }
    [Required(ErrorMessage = "Caminho da imagem obrigatorio")]
    [StringLength(300, ErrorMessage = "Tamanho maximo do 300 caracteres")]
    public string? ImagemUrl { get; set; }
    public List<Produto>? Produtos { get; set; }
    public Categoria()
    {
        Produtos = new List<Produto>();
    }
}
