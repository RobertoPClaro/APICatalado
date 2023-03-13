using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APICatalogo.Models;
public class Produto
{
    [Key]
    public int ProdutoId { get; set; }
    [Required(ErrorMessage = "Nome obrigatorio")]
    [StringLength(80, ErrorMessage = "Tamanho maximo do 80 caracteres")]
    public string? Nome { get; set; }
    [Required(ErrorMessage = "Descrição obrigatorio")]
    [StringLength(300, ErrorMessage = "Tamanho maximo do 300 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Preço obrigatorio")]
    [Column(TypeName = "decimal(8,2)")]
    public decimal Preco { get; set; }
    [Required(ErrorMessage = "Caminho da imagem obrigatorio")]
    [StringLength(300, ErrorMessage = "Tamanho maximo do 300 caracteres")]
    public string? ImagemUrl { get; set; }
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }

    public int CategoriaId { get; set; }
    [JsonIgnore]
    public Categoria? Categoria { get; set; }

}
