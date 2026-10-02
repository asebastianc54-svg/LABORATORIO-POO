namespace FlexSpace.DAL.Entities;

public class Puesto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public TipoPuesto TipoPuesto { get; set; }
    public decimal TarifaBasePorHora { get; set; }
}
