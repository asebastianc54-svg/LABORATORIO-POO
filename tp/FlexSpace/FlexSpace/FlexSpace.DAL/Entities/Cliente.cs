namespace FlexSpace.DAL.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoCliente TipoCliente { get; set; }
    public int SancionesActivas { get; set; }
}
