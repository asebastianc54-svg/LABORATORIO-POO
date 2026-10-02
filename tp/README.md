# FlexSpace – Sistema de Gestión de Reservas y Tarificación

Trabajo Práctico en C# (.NET 8) con arquitectura en 3 capas.
**Estado de la entrega: avance parcial (aprox. 50 %).** Más abajo se detalla qué está hecho y qué falta.

## 1. Estructura de la solución

```
FlexSpace.sln
├── FlexSpace.UI   (Consola)        → referencia solo a BLL
├── FlexSpace.BLL  (Biblioteca)     → referencia a DAL
└── FlexSpace.DAL  (Biblioteca)     → no referencia a nadie
```

| Capa | Qué contiene |
|------|--------------|
| **UI** | `Program.cs` (menú y flujo), `EntradaConsola.cs` (lectura y formato de datos). No calcula tarifas, no valida reglas ni toca la base. |
| **BLL** | `ReservaService` (orquesta las reglas), `Tarifas/` (cálculo de precio), `Exceptions/`, `Dtos/`, `ReglasNegocio.cs` (constantes: 15 %, 10 %, 5 %, 20 %, 3 sanciones, 2 horas). Sin `Console`. |
| **DAL** | `Entities/` (Cliente, Puesto, Reserva y enums), `Repositories/` (ADO.NET con `Microsoft.Data.Sqlite`), `Database/` (conexión y creación de tablas + datos de prueba). Sin validaciones de negocio. |

### Cómo respetamos las reglas estrictas de arquitectura
- La **UI** solo tiene `ProjectReference` a la BLL. Para armar los objetos usa `FlexSpaceFactory` (en la BLL), que es quien crea los repositorios de la DAL internamente. La UI además solo trabaja con DTOs (`ResumenReservaDto`, `CancelacionResultadoDto`), nunca con entidades.
- La **BLL** no tiene ningún `Console.WriteLine` / `Console.ReadLine`.
- La **DAL** no conoce a la UI ni decide nada de negocio. Por ejemplo, para cancelar, la BLL decide si hay que penalizar y la DAL solo ejecuta el `UPDATE` (ambos cambios dentro de una transacción).

## 2. Qué hicimos

### Base de datos (SQLite)
- Tablas `Cliente`, `Puesto` y `Reserva` con los campos pedidos, claves foráneas y `CHECK` para los valores de los enums.
- Se crean solas al primer arranque (`flexspace.db`) y se cargan datos de prueba:

| Id | Cliente | Tipo | Sanciones | Para probar |
|----|---------|------|-----------|-------------|
| 1 | Ana Perez | Estándar | 0 | Caso base |
| 2 | Bruno Gomez | VIP | 0 | Descuento VIP |
| 3 | Carla Diaz | Estándar | 1 | Penalización por sanciones |
| 4 | Diego Ruiz | Estándar | 3 | Bloqueo (`ClienteSancionadoException`) |
| 5 | Eva Lopez | VIP | 2 | VIP con sanciones (pierde descuentos) |

| Id | Puesto | Tipo | Tarifa/hora |
|----|--------|------|-------------|
| 1 | ESC-01 | EscritorioIndividual | 1000 |
| 2 | ESC-02 | EscritorioIndividual | 1000 |
| 3 | SALA-01 | SalaReuniones | 3000 |
| 4 | CAB-01 | CabinaPrivada | 2000 |

### Reglas de negocio (BLL)
- **A. Disponibilidad:** no se permite reservar si existe otra reserva *Confirmada* del mismo puesto que se solape (`inicio_existente < fin_nuevo` y `fin_existente > inicio_nuevo`). Una reserva que termina justo cuando otra empieza **no** se considera solapada. Lanza `PuestoNoDisponibleException`.
- **B. Tarifa dinámica:** implementada con una clase por regla (`IReglaTarifa`) que se ejecutan en el orden de la consigna:
  1. Subtotal = horas × tarifa base
  2. +15 % si incluye sábado o domingo
  3. −10 % sobre el acumulado si dura 5 h o más
  4. −5 % sobre el acumulado si es VIP
  5. Si tiene sanciones > 0: se anulan los descuentos (volumen y VIP) y se suma 20 % sobre la tarifa base
- **C. Bloqueo:** con 3 o más sanciones se lanza `ClienteSancionadoException`.
- Validaciones extra: fin posterior a inicio, no reservar en el pasado, cliente/puesto inexistente.

Ejemplos de cálculo (tarifa 1000/h, 6 horas, sábado):
- Cliente VIP sin sanciones: 6000 → 6900 (+15 %) → 6210 (−10 %) → **5899,50** (−5 %)
- Cliente con 1 sanción: 6000 + 900 (fin de semana) + 1200 (20 %) = **8100,00**

### Menú de consola
| Opción | Estado |
|--------|--------|
| 1. Registrar nueva reserva | ✅ Hecha. Pide datos, muestra el desglose del precio y recién después de confirmar guarda. |
| 2. Cancelar reserva | ✅ Hecha. Si faltan menos de 2 h para el inicio suma +1 sanción al cliente en la BD. |
| 3. Consultar reservas activas por puesto | ⏳ Pendiente (el menú la muestra, avisa que no está implementada). |
| 4. Listar clientes sancionados | ⏳ Pendiente (ídem). |

## 3. Hasta dónde llegamos

Completo: arquitectura de 3 capas, modelo de datos, las 3 reglas de negocio (A, B y C), opciones 1 y 2 del menú.

Falta:
- **Opción 3:** agregar en la DAL una consulta de reservas `Confirmada` con `FechaInicio > ahora` filtrada por `Puesto.Codigo`, un método en `ReservaService` y la pantalla en la UI.
- **Opción 4:** agregar en la DAL un `SELECT` de clientes con `SancionesActivas > 0`, su método en la BLL y la pantalla.
- **Tests unitarios** del `CalculadorTarifa` y de `ReservaService` (por eso `ReservaService` recibe la hora actual por parámetro: para poder probarlo con fechas fijas).
- Pasar a *Finalizada* las reservas cuyo horario ya terminó (hoy el estado existe pero nada lo cambia).
- **Compilar y probar de punta a punta:** el código todavía no fue compilado ni ejecutado, queda pendiente hacerlo y corregir lo que aparezca.

## 4. Decisiones e interpretaciones de la consigna

- **Sanciones y descuentos:** entendimos que "pierde todos los descuentos previos" afecta solo a los descuentos (volumen y VIP); el recargo de fin de semana se mantiene. El 20 % se calcula sobre el subtotal base.
- **Cancelar una reserva que ya empezó:** no lo permitimos (la consigna no lo define).
- **Horas fraccionadas:** se calculan con decimales (1 h 30 min = 1,5 h). Montos redondeados a 2 decimales.
- **Fin de semana:** se considera si cualquier parte del rango cae en sábado o domingo. Una reserva que termina exactamente a las 00:00 de un sábado no cuenta ese sábado.
- **Base de datos:** SQLite, accedida con ADO.NET. Se eligió por no requerir instalar un servidor.

## 5. Cómo ejecutarlo

Requisitos: .NET 8 SDK.

```bash
dotnet restore
dotnet run --project FlexSpace.UI
```

Las fechas se ingresan con formato `dd/MM/yyyy HH:mm` (ej.: `15/11/2026 10:00`). Para reiniciar los datos, borrar el archivo `flexspace.db`.
