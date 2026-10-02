namespace FlexSpace.BLL.Tarifas;

/// <summary>Un paso del calculo de tarifa. Cada regla es una clase (polimorfismo).</summary>
public interface IReglaTarifa
{
    void Aplicar(ContextoTarifa contexto, DesgloseTarifa desglose);
}
