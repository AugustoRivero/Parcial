using System;
using System.Collections.Generic;

// =====================================================
// MODELO DE CLASES — igual al Problema 1
// =====================================================

public interface IPersonalizable
{
    bool TieneModificaciones { get; }
    void AgregarModificacion(string detalle);
    void QuitarModificaciones();
    string ObtenerEstado() => TieneModificaciones ? "Personalizado" : "Estándar";
}

public abstract class Plato
{
    private string _nombre;
    private string _idPlato;

    public string Nombre  => _nombre;
    public string IdPlato => _idPlato;

    private string _categoria;
    public string Categoria
    {
        get => _categoria;
        set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("La categoría no puede estar vacía.");
            _categoria = value;
        }
    }

    public int Anio { get; set; }

    public Plato(string id, string nombre, string categoria, int anio)
    {
        _idPlato  = id;
        _nombre   = nombre;
        Categoria = categoria;
        Anio      = anio;
    }

    public abstract decimal CalcularCostoBase();

    public virtual string ObtenerFicha()
    {
        return $"{IdPlato} | {Nombre} | {Categoria} | Año: {Anio}";
    }
}

public class PlatoPrincipal : Plato, IPersonalizable
{
    public bool TieneCarneRoja    { get; private set; }
    public int  CantidadPorciones { get; private set; }

    private List<string> _modificaciones = new List<string>();
    public bool TieneModificaciones => _modificaciones.Count > 0;

    public void AgregarModificacion(string detalle) => _modificaciones.Add(detalle);
    public void QuitarModificaciones()              => _modificaciones.Clear();

    public PlatoPrincipal(string id, string nombre, string categoria, int anio,
                          bool tieneCarneRoja, int cantidadPorciones)
        : base(id, nombre, categoria, anio)
    {
        TieneCarneRoja    = tieneCarneRoja;
        CantidadPorciones = cantidadPorciones;
    }

    // Con carne roja: $4500 + $800/porción; sin carne: $3000 + $600/porción
    public override decimal CalcularCostoBase()
    {
        return TieneCarneRoja
            ? 4500m + (CantidadPorciones * 800m)
            : 3000m + (CantidadPorciones * 600m);
    }

    public override string ObtenerFicha()
    {
        string base_ = base.ObtenerFicha();
        string carne  = TieneCarneRoja ? "Con carne roja" : "Sin carne";
        return $"{base_} | {carne} | Porciones: {CantidadPorciones} | Costo base: ${CalcularCostoBase()}";
    }

    public void AccionPropia()
    {
        Console.WriteLine($"[Plato Principal] '{Nombre}' está siendo preparado en la parrilla.");
    }
}

public class Postre : Plato
{
    public bool LlevaCrema   { get; private set; }
    public int  GramosAzucar { get; private set; }

    public Postre(string id, string nombre, string categoria, int anio,
                  bool llevaCrema, int gramosAzucar)
        : base(id, nombre, categoria, anio)
    {
        LlevaCrema   = llevaCrema;
        GramosAzucar = gramosAzucar;
    }

    // Con crema: $1800 + $2/gr azúcar; sin crema: $1200 + $1/gr azúcar
    public override decimal CalcularCostoBase()
    {
        return LlevaCrema
            ? 1800m + (GramosAzucar * 2m)
            : 1200m + (GramosAzucar * 1m);
    }

    public override string ObtenerFicha()
    {
        string base_ = base.ObtenerFicha();
        string crema  = LlevaCrema ? "Con crema" : "Sin crema";
        return $"{base_} | {crema} | Azúcar: {GramosAzucar}gr | Costo base: ${CalcularCostoBase()}";
    }
}

public record Pedido(
    string   IdPedido,
    string   IdPlato,
    string   NombreMozo,
    string   Mesa,
    decimal  Precio,
    DateTime Fecha
);

// =====================================================
// CLASE ESTÁTICA DE UTILIDADES — contiene el método recursivo
// =====================================================

// General: agrupa los algoritmos de búsqueda que operan sobre las listas del sistema.
// Técnico: clase estática porque sus métodos no dependen de ningún estado de instancia;
// BuscarPlatoPorId implementa recursión de cola sin bucles internos.
public static class Utilidades
{
    // Busca un Plato en la lista por su IdPlato de forma recursiva.
    // Caso base 1: índice fuera de rango → retorna null (elemento no encontrado).
    // Caso base 2: el elemento en la posición actual coincide → retorna ese elemento.
    // Caso recursivo: avanza al siguiente índice llamándose a sí mismo con indice + 1.
    public static Plato BuscarPlatoPorId(List<Plato> lista, string id, int indice = 0)
    {
        // Caso base 1: se llegó al final de la lista sin encontrar el id.
        if (indice >= lista.Count)
            return null;

        // Caso base 2: el elemento actual tiene el id buscado.
        if (lista[indice].IdPlato == id)
            return lista[indice];

        // Caso recursivo: el elemento actual no coincide, se prueba con el siguiente.
        return BuscarPlatoPorId(lista, id, indice + 1);
    }
}
//explicar con pseudocódigo la lógica de la función BuscarPlatoPorId y indicar donde se usara en el programa principal.     
/*
Pseudocódigo de BuscarPlatoPorId:           
Función BuscarPlatoPorId(lista, id, indice = 0):
    Si indice >= longitud de lista:
        Retornar null  // Caso base: no encontrado
    Si lista[indice].IdPlato == id:
        Retornar lista[indice]  // Caso base: encontrado
    Retornar BuscarPlatoPorId(lista, id, indice + 1)  // Caso recursivo: siguiente índice   
En el programa principal, BuscarPlatoPorId se utiliza en la sección "PARTE B — RECURSIVIDAD, ARRAYS Y MATRICES", específicamente en el "Ejercicio 1: BuscarPlatoPorId (recursivo)". Allí se realizan dos pruebas: una con un id existente ("PL005") que debería retornar el plato correspondiente, y otra con un id inexistente ("PL999") que debería retornar null. El resultado de cada búsqueda se imprime en la consola.
*/
// que significa el static en este caso?    
/*En este caso, la palabra clave "static" indica que la clase "Utilidades" es una clase estática
, lo que significa que no se pueden crear instancias de esta clase. 
Todos los métodos y miembros de una clase estática también deben ser estáticos. 
Esto es útil para agrupar funciones que no dependen de un estado de instancia y que pueden ser llamadas directamente a través del nombre de la clase
, sin necesidad de crear un objeto. 
En el caso de "Utilidades", el método "BuscarPlatoPorId" es estático,
 lo que permite llamarlo directamente como "Utilidades.BuscarPlatoPorId(...)" 
 desde cualquier parte del programa sin necesidad de instancar la clase "Utilidades".
  Esto es especialmente adecuado para funciones de utilidad o helper que realizan operaciones generales,
   como la búsqueda en una lista, sin requerir datos específicos de una instancia.
*/
// =====================================================
// PROGRAMA PRINCIPAL
// =====================================================

public class Program
{
    public static void Main(string[] args)
    {
        // ── Carga de datos (mismos que Problema 1; Flan casero ya eliminado) ──────

        List<Plato> registros = new List<Plato>
        {
            new PlatoPrincipal("PL001", "Bife de chorizo", "Principal", 2026, true,  2),
            new PlatoPrincipal("PL002", "Pasta primavera", "Principal", 2026, false, 3),
            new Postre        ("PL003", "Tiramisú",        "Postre",    2026, true,  120),
            // PL004 Flan casero eliminado en Problema 1
            new PlatoPrincipal("PL005", "Asado mixto",     "Principal", 2026, true,  4),
            new Postre        ("PL006", "Mousse choco",    "Postre",    2026, true,  100),
            new PlatoPrincipal("PL007", "Risotto",         "Principal", 2026, false, 2),
        };

        List<Pedido> registros2 = new List<Pedido>
        {
            new Pedido("PD001", "PL001", "Mozo: Díaz",   "Mesa VIP",         6900m, new DateTime(2026,  4,  1)),
            new Pedido("PD002", "PL001", "Mozo: Suárez", "Mesa familiar",    6900m, new DateTime(2026,  4, 15)),
            new Pedido("PD003", "PL002", "Mozo: Díaz",   "Mesa corporativa", 4800m, new DateTime(2026,  4, 10)),
            new Pedido("PD004", "PL003", "Mozo: Flores", "Mesa romántica",   2040m, new DateTime(2026,  3, 20)),
            new Pedido("PD005", "PL004", "Mozo: Suárez", "Mesa casual",      1280m, new DateTime(2026,  4,  5)),
            new Pedido("PD006", "PL005", "Mozo: Díaz",   "Mesa grupal",      7700m, new DateTime(2026,  4, 22)),
            new Pedido("PD007", "PL006", "Mozo: Flores", "Mesa VIP",         2000m, new DateTime(2026,  4, 25)),
            new Pedido("PD008", "PL003", "Mozo: Suárez", "Mesa cumpleaños",  2040m, new DateTime(2026,  2, 18)),
        };

        // =====================================================
        // PARTE A — BUCLES
        // =====================================================

        // ── Tarea 1: Historial de "Bife de chorizo" con for ──────────────────────
        Console.WriteLine("=== TAREA 1: HISTORIAL DE BIFE DE CHORIZO ===");

        // Se acumulan en una nueva lista los pedidos cuyo IdPlato sea "PL001".
        // Se usa for con índice entero para recorrer registros2 posición por posición.
        List<Pedido> historialBife = new List<Pedido>();
        for (int i = 0; i < registros2.Count; i++)
        {
            if (registros2[i].IdPlato == "PL001")
            {
                historialBife.Add(registros2[i]);
            }
        }

        // Se recorre la lista acumulada con foreach para imprimir cada registro.
        decimal totalBife = 0m;
        foreach (Pedido p in historialBife)
        {
            Console.WriteLine($"{p.IdPedido} | {p.Fecha:dd/MM/yyyy} | {p.Mesa} | Responsable: {p.NombreMozo} | ${p.Precio}");
            totalBife += p.Precio;
        }
        Console.WriteLine($"Total acumulado de Bife de chorizo: ${totalBife}");

        // ── Tarea 2: Tabla de costos base con while ───────────────────────────────
        Console.WriteLine("\n=== TAREA 2: TABLA DE COSTOS BASE ===");

        // Se recorre registros con un índice entero controlado por while.
        // En cada iteración se llama a CalcularCostoBase(), que por polimorfismo
        // invoca el override real de cada subclase (PlatoPrincipal o Postre).
        int idx = 0;
        while (idx < registros.Count)
        {
            Plato plato = registros[idx];
            Console.WriteLine($"{plato.Nombre} ({plato.Categoria}) → Costo base: ${plato.CalcularCostoBase()}");
            idx++;
        }

        // ── Tarea 3: Reporte acumulado por responsable con do-while ──────────────
        Console.WriteLine("\n=== TAREA 3: REPORTE POR RESPONSABLE ===");

        // Array fijo con los responsables en el orden requerido por el enunciado.
        string[] responsables = { "Mozo: Díaz", "Mozo: Suárez", "Mozo: Flores" };

        decimal totalGeneral = 0m;
        int     r            = 0;

        // do-while garantiza que el cuerpo se ejecute al menos una vez.
        // El for interno recorre todos los pedidos acumulando los que corresponden
        // al responsable de la iteración actual del do-while.
        do
        {
            string  nombreMozo     = responsables[r];
            int     cantidadPedidos = 0;
            decimal totalMozo      = 0m;

            for (int j = 0; j < registros2.Count; j++)
            {
                if (registros2[j].NombreMozo == nombreMozo)
                {
                    cantidadPedidos++;
                    totalMozo += registros2[j].Precio;
                }
            }

            Console.WriteLine($"{nombreMozo} → {cantidadPedidos} registros | Total: ${totalMozo}");
            totalGeneral += totalMozo;
            r++;
        }
        while (r < responsables.Length);

        Console.WriteLine("─────────────────────────────");
        Console.WriteLine($"TOTAL GENERAL: ${totalGeneral}");

        // =====================================================
        // PARTE B — RECURSIVIDAD, ARRAYS Y MATRICES
        // =====================================================

        // ── Ejercicio 1: BuscarPlatoPorId (recursivo) ────────────────────────────
        Console.WriteLine("\n=== EJERCICIO 1: BÚSQUEDA RECURSIVA ===");

        // Caso exitoso: PL005 existe en la lista.
        Plato encontrado = Utilidades.BuscarPlatoPorId(registros, "PL005");
        if (encontrado != null)
            Console.WriteLine(encontrado.ObtenerFicha());
        else
            Console.WriteLine("PL005 no encontrado.");

        // Caso fallido: PL999 no existe; la recursión llega al final y retorna null.
        Plato noEncontrado = Utilidades.BuscarPlatoPorId(registros, "PL999");
        if (noEncontrado != null)
            Console.WriteLine(noEncontrado.ObtenerFicha());
        else
            Console.WriteLine("PL999 no encontrado.");

        // ── Ejercicio 2: Array de costos totales por plato ───────────────────────
        Console.WriteLine("\n=== EJERCICIO 2: ARRAY DE COSTOS TOTALES ===");

        // costosPorPlato[i] acumula el total de pedidos del plato registros[i].
        // Se construye con bucles anidados: el externo itera platos,
        // el interno itera pedidos buscando coincidencias de IdPlato.
        decimal[] costosPorPlato = new decimal[registros.Count];

        for (int i = 0; i < registros.Count; i++)
        {
            costosPorPlato[i] = 0m;
            for (int j = 0; j < registros2.Count; j++)
            {
                if (registros2[j].IdPlato == registros[i].IdPlato)
                {
                    costosPorPlato[i] += registros2[j].Precio;
                }
            }
        }

        // Imprimir el array completo.
        for (int i = 0; i < registros.Count; i++)
        {
            Console.WriteLine($"{registros[i].Nombre}: ${costosPorPlato[i]}");
        }

        // Calcular mayor gasto (todos los platos).
        int     indiceMayor = 0;
        for (int i = 1; i < costosPorPlato.Length; i++)
        {
            if (costosPorPlato[i] > costosPorPlato[indiceMayor])
                indiceMayor = i;
        }
        Console.WriteLine($"Mayor gasto: {registros[indiceMayor].Nombre} — ${costosPorPlato[indiceMayor]}");

        // Calcular menor gasto (solo platos con al menos una consulta, es decir total > 0).
        int indiceMenor = -1;
        for (int i = 0; i < costosPorPlato.Length; i++)
        {
            if (costosPorPlato[i] > 0m)
            {
                if (indiceMenor == -1 || costosPorPlato[i] < costosPorPlato[indiceMenor])
                    indiceMenor = i;
            }
        }
        Console.WriteLine($"Menor gasto: {registros[indiceMenor].Nombre} — ${costosPorPlato[indiceMenor]}");

        // Calcular promedio de platos con al menos una consulta.
        decimal sumaConConsultas  = 0m;
        int     cantConConsultas  = 0;
        for (int i = 0; i < costosPorPlato.Length; i++)
        {
            if (costosPorPlato[i] > 0m)
            {
                sumaConConsultas += costosPorPlato[i];
                cantConConsultas++;
            }
        }
        decimal promedio = sumaConConsultas / cantConConsultas;
        Console.WriteLine($"Promedio: ${promedio}");

        // ── Ejercicio 3: Matriz platos × responsables ─────────────────────────────
        Console.WriteLine("\n=== EJERCICIO 3: MATRIZ PLATOS × RESPONSABLES ===");

        // Definición de filas (platos) y columnas (responsables) de la matriz.
        int cantidadRegistros     = registros.Count;
        int cantidadResponsables  = 3;

        // columna 0 = Mozo: Díaz | columna 1 = Mozo: Suárez | columna 2 = Mozo: Flores
        string[] nombresResponsables = { "Mozo: Díaz", "Mozo: Suárez", "Mozo: Flores" };

        // Se declara la matriz bidimensional. Todas las celdas inician en 0 por defecto.
        decimal[,] matriz = new decimal[cantidadRegistros, cantidadResponsables];

        // Triple for: el primero itera platos (filas), el segundo itera responsables
        // (columnas), el tercero itera pedidos buscando coincidencias dobles.
        for (int i = 0; i < cantidadRegistros; i++)
        {
            for (int j = 0; j < cantidadResponsables; j++)
            {
                for (int k = 0; k < registros2.Count; k++)
                {
                    if (registros2[k].IdPlato    == registros[i].IdPlato &&
                        registros2[k].NombreMozo == nombresResponsables[j])
                    {
                        matriz[i, j] += registros2[k].Precio;
                    }
                }
            }
        }

        // Imprimir encabezado de la tabla.
        Console.WriteLine($"{"Plato",-20} {"Díaz",10} {"Suárez",10} {"Flores",10}");
        Console.WriteLine(new string('-', 54));

        // Imprimir cada fila de la matriz.
        for (int i = 0; i < cantidadRegistros; i++)
        {
            Console.WriteLine($"{registros[i].Nombre,-20} {matriz[i,0],10} {matriz[i,1],10} {matriz[i,2],10}");
        }

        Console.WriteLine(new string('-', 54));

        // Calcular total y cantidad de pedidos por responsable (suma de cada columna).
        Console.WriteLine("\n── Totales por responsable ──");
        int    indiceMayorRecaudacion = 0;
        decimal mayorRecaudacion      = 0m;

        for (int j = 0; j < cantidadResponsables; j++)
        {
            decimal totalColumna   = 0m;
            int     pedidosColumna = 0;

            // Se recorre registros2 directamente para incluir pedidos de platos
            // que ya no están en registros (ej. PL004 Flan casero eliminado).
            for (int k = 0; k < registros2.Count; k++)
            {
                if (registros2[k].NombreMozo == nombresResponsables[j])
                {
                    pedidosColumna++;
                    totalColumna += registros2[k].Precio;
                }
            }

            Console.WriteLine($"{nombresResponsables[j]} → {pedidosColumna} pedidos | Total: ${totalColumna}");

            if (totalColumna > mayorRecaudacion)
            {
                mayorRecaudacion       = totalColumna;
                indiceMayorRecaudacion = j;
            }
        }

        Console.WriteLine($"\nResponsable con mayor recaudación: {nombresResponsables[indiceMayorRecaudacion]}");
    }
}