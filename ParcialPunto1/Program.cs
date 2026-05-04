using System;
using System.Collections.Generic;
using System.Linq;

// =====================================================
// PARTE A — MODELO DE CLASES
// =====================================================

// Interfaz IPersonalizable
// General: define el contrato para los objetos que admiten modificaciones.
// Técnico: expone una propiedad booleana y dos métodos para administrar
// una colección de modificaciones; también incluye un método por defecto
// que devuelve el estado basado en si existen modificaciones.
public interface IPersonalizable
{
    bool TieneModificaciones { get; }
    void AgregarModificacion(string detalle);
    void QuitarModificaciones();

    // Método por defecto (C# 8+). Devuelve el estado del objeto según si
    // tiene modificaciones aplicadas.
    string ObtenerEstado()
    {
        return TieneModificaciones ? "Personalizado" : "Estándar";
    }


}

// Clase abstracta Plato
// General: representa la información común a todos los platos.
// Técnico: define campos privados para encapsular datos, validación
// de categoría y un método abstracto que obliga a las subclases a calcular
// su costo base.
public abstract class Plato
{
    private string _nombre;
    private string _idPlato;

    public string Nombre
    {
        get { return _nombre; }
    }

    public string IdPlato
    {
        get { return _idPlato; }
    }

    private string _categoria;
    public string Categoria
    {
        get
        {
            return _categoria;
        }
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
        _idPlato = id;
        _nombre = nombre;
        Categoria = categoria;
        Anio = anio;
    }

    public abstract decimal CalcularCostoBase();

    public virtual string ObtenerFicha()
    {
        return $"{IdPlato} | {Nombre} | {Categoria} | Año: {Anio}";
    }
}

// Clase PlatoPrincipal : Plato, IPersonalizable.
// General: modelo de plato principal con atributos como carne roja y porciones.
// Técnico: hereda de Plato, implementa IPersonalizable, y calcula el costo
// base según el tipo de carne y la cantidad de porciones.
public class PlatoPrincipal : Plato, IPersonalizable
{
    public bool TieneCarneRoja { get; private set; }
    public int CantidadPorciones { get; private set; }

    // Implementación de IPersonalizable: mantiene una lista de modificaciones.
    private List<string> _modificaciones = new List<string>();
    public bool TieneModificaciones
    {
        get
        {
            return _modificaciones.Count > 0;
        }
    }    

    public void AgregarModificacion(string detalle)
    {
        _modificaciones.Add(detalle);
    }

    public void QuitarModificaciones()
    {
        _modificaciones.Clear();
    }

    public PlatoPrincipal(string id, string nombre, string categoria, int anio,
                          bool tieneCarneRoja, int cantidadPorciones)
        : base(id, nombre, categoria, anio)
    {
        TieneCarneRoja = tieneCarneRoja;
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
        string carne = TieneCarneRoja ? "Con carne roja" : "Sin carne";
        return $"{base_} | {carne} | Porciones: {CantidadPorciones} | Costo base: ${CalcularCostoBase()}";
    }

    public void AccionPropia()
    {
        Console.WriteLine($"[Plato Principal] '{Nombre}' está siendo preparado en la parrilla.");
    }
}

// Clase Postre : Plato.
// General: modelo de postre con propiedades de crema y cantidad de azúcar.
// Técnico: hereda de Plato y calcula el costo base usando la condición de
// crema y el valor por gramo de azúcar.
public class Postre : Plato
{
    public bool LlevaCrema { get; private set; }
    public int GramosAzucar { get; private set; }

    public Postre(string id, string nombre, string categoria, int anio,
                  bool llevaCrema, int gramosAzucar)
        : base(id, nombre, categoria, anio)
    {
        LlevaCrema = llevaCrema;
        GramosAzucar = gramosAzucar;
    }

    // Con crema: $1800 + $2/gr azúcar; sin crema: $1200 + $1/gr azúcar
    public override decimal CalcularCostoBase()
    {
        return LlevaCrema
            ? 1800m + (GramosAzucar * 2m)
            : 1200m + (GramosAzucar * 1m);
    }
// explicar calcularCostobase con psudocodigo
    // Si LlevaCrema es true, el costo base es $1800 más $2 por cada gramo de azúcar.
    // Si LlevaCrema es false, el costo base es $1200 más $1 por cada gramo de azúcar.
    public override string ObtenerFicha()
    {
        string base_ = base.ObtenerFicha();
        string crema = LlevaCrema ? "Con crema" : "Sin crema";
        return $"{base_} | {crema} | Azúcar: {GramosAzucar}gr | Costo base: ${CalcularCostoBase()}";
    }
}

// Record Pedido
// General: representa un pedido realizado en el sistema.
// Técnico: define un tipo inmutable con propiedades nombradas que se usan
// en las consultas LINQ para agrupar y filtrar información.
public record Pedido(
    string IdPedido,
    string IdPlato,
    string NombreMozo,
    string Mesa,
    decimal Precio,
    DateTime Fecha
);

// =====================================================
// PROGRAMA PRINCIPAL
// =====================================================

public class Program
{
    public static void Main(string[] args)
    {
        // =====================================================
        // PASO 1: CARGA DE REGISTROS
        // =====================================================
        // General: inicializa el catálogo de platos disponibles en el sistema.
        // Técnico: crea una lista de objetos Plato utilizando subclases concretas
        // y muestra la ficha de cada uno para verificar que se cargó correctamente.
        Console.WriteLine("=== PASO 1: CARGA DE REGISTROS ===");

        // Se crea la lista de platos con ejemplos de platos principales y postres.
        List<Plato> registros = new List<Plato>
        {
            new PlatoPrincipal("PL001", "Bife de chorizo", "Principal", 2026, true, 2),
            new PlatoPrincipal("PL002", "Pasta primavera",  "Principal", 2026, false, 3),
            new Postre("PL003", "Tiramisú",       "Postre",    2026, true,  120),
            new Postre("PL004", "Flan casero",    "Postre",    2026, false, 80),
            new PlatoPrincipal("PL005", "Asado mixto",    "Principal", 2026, true, 4),
            new Postre("PL006", "Mousse choco",   "Postre",    2026, true,  100),
        };

        foreach (var plato in registros)
        {
            Console.WriteLine(plato.ObtenerFicha());
        }

        // =====================================================
        // PASO 2: CARGA DE PEDIDOS
        // =====================================================
        // General: crea el registro de ventas asociando cada pedido a un plato.
        // Técnico: inicializa una lista de Pedido y utiliza la propiedad IdPlato
        // para poder enlazarla con los objetos Plato en consultas posteriores.
        Console.WriteLine("\n=== PASO 2: CARGA DE PEDIDOS ===");

        // Se crea la lista de pedidos que referencian cada plato por su Id.
        List<Pedido> registros2 = new List<Pedido>
        {
            new Pedido("PD001", "PL001", "Mozo: Díaz",   "Mesa VIP",         6900m,  new DateTime(2026,  4,  1)),
            new Pedido("PD002", "PL001", "Mozo: Suárez", "Mesa familiar",    6900m,  new DateTime(2026,  4, 15)),
            new Pedido("PD003", "PL002", "Mozo: Díaz",   "Mesa corporativa", 4800m,  new DateTime(2026,  4, 10)),
            new Pedido("PD004", "PL003", "Mozo: Flores", "Mesa romántica",   2040m,  new DateTime(2026,  3, 20)),
            new Pedido("PD005", "PL004", "Mozo: Suárez", "Mesa casual",      1280m,  new DateTime(2026,  4,  5)),
            new Pedido("PD006", "PL005", "Mozo: Díaz",   "Mesa grupal",      7700m,  new DateTime(2026,  4, 22)),
            new Pedido("PD007", "PL006", "Mozo: Flores", "Mesa VIP",         2000m,  new DateTime(2026,  4, 25)),
            new Pedido("PD008", "PL003", "Mozo: Suárez", "Mesa cumpleaños",  2040m,  new DateTime(2026,  2, 18)),
        };

        foreach (var pedido in registros2)
        {
            Console.WriteLine(pedido);
        }

        // =====================================================
        // PASO 3: AGREGAR UN NUEVO REGISTRO
        // =====================================================
        // General: demuestra cómo agregar un nuevo elemento al catálogo.
        // Técnico: crea una instancia de PlatoPrincipal y la agrega a la lista
        // usando el método Add de List<T>.
        Console.WriteLine("\n=== PASO 3: AGREGAR UN NUEVO REGISTRO ===");

        // Se crea y añade un nuevo plato al listado de registros existentes.
        var risotto = new PlatoPrincipal("PL007", "Risotto", "Principal", 2026, false, 2);
        registros.Add(risotto);
        Console.WriteLine("✔ Risotto agregado exitosamente.");
        Console.WriteLine(risotto.ObtenerFicha());

     // =====================================================
        // PASO 4: ELIMINAR UN REGISTRO
        // =====================================================
        // General: ilustra la eliminación de un elemento del catálogo por un criterio.
        // Técnico: usa LINQ FirstOrDefault para encontrar el primer plato con el nombre
        // especificado y luego Remove para borrarlo de la colección.
        Console.WriteLine("\n=== PASO 4: ELIMINAR UN REGISTRO ===");
 
        // Busca un registro por nombre y lo elimina si se encuentra.
        Plato flanCasero = null;
        foreach (var plato in registros)
        {
            if (plato.Nombre == "Flan casero")
            {
                flanCasero = plato;
                break;
            }
        }
        if (flanCasero != null)
        {
            registros.Remove(flanCasero);
            Console.WriteLine("✔ Flan casero eliminado del sistema.");
        }
 
        Plato sushi = null;
        foreach (var plato in registros)
        {
            if (plato.Nombre == "Sushi")
            {
                sushi = plato;
                break;
            }
        }
        bool eliminado = registros.Remove(sushi);
        if (!eliminado)
        {
            Console.WriteLine("✘ No se encontró ningún registro con el nombre Sushi.");
        }
         

        // =====================================================
        // PASO 5: RECORRIDO POLIMÓRFICO
        // =====================================================
        // General: muestra cómo se comportan diferentes tipos de platos dentro de
        // una colección común.
        // Técnico: itera sobre List<Plato> y utiliza métodos virtuales y pattern matching
        // para ejecutar el código específico de cada subclase.
        Console.WriteLine("\n=== PASO 5: RECORRIDO POLIMÓRFICO ===");

        // POLIMORFISMO: la lista es de tipo Plato, pero en tiempo de ejecución
        // .NET invoca el ObtenerFicha() real de cada objeto (PlatoPrincipal o Postre).
        foreach (var plato in registros)
        {
            Console.WriteLine(plato.ObtenerFicha());
            if (plato is PlatoPrincipal pp)
            {
                pp.AccionPropia();
            }
        }

        // =====================================================
        // PASO 6: USAR IPERSONALIZABLE
        // =====================================================
        // General: demuestra la interfaz como mecanismo para trabajar con
        // características transversales de los objetos.
        // Técnico: convierte un Plato a IPersonalizable y llama métodos de la interfaz
        // para agregar, eliminar y consultar modificaciones.
        Console.WriteLine("\n=== PASO 6: USAR IPERSONALIZABLE ===");

        // Demuestra cómo usar la interfaz IPersonalizable sobre un plato.
        var bifePlato = registros.FirstOrDefault(p => p.Nombre == "Bife de chorizo");
        IPersonalizable bifePersonalizable = (IPersonalizable)bifePlato;

        bifePersonalizable.AgregarModificacion("prueba");
        Console.WriteLine("✔ AgregarModificacion aplicado a Bife de chorizo.");

        bifePersonalizable.QuitarModificaciones();
        Console.WriteLine("✔ QuitarModificaciones ejecutado para Bife de chorizo.");

        string estado = bifePersonalizable.ObtenerEstado();
        Console.WriteLine($"Estado de Bife de chorizo: {estado}");

        // =====================================================
        // PARTE C — CONSULTAS LINQ
        // =====================================================

        // =====================================================
        // CONSULTA 1: PLATOS DE MAYOR A MENOR COSTO BASE
        // =====================================================
        // General: ordena los platos para identificar cuáles son los de mayor costo.
        // Técnico: utiliza LINQ OrderByDescending sobre el valor retornado por
        // CalcularCostoBase() que es específico de cada subclase.
        Console.WriteLine("\n=== CONSULTA 1: PLATOS DE MAYOR A MENOR COSTO BASE ===");

        // Ordena los platos por costo base para mostrar el más caro primero.
        var platosPorCosto = registros.OrderByDescending(p => p.CalcularCostoBase());
        foreach (var plato in platosPorCosto)
        {
            Console.WriteLine($"{plato.Nombre} — ${plato.CalcularCostoBase()}");
        }

        // =====================================================
        // CONSULTA 2: MOZO DÍAZ EN ABRIL 2026
        // =====================================================
        // General: selecciona las ventas realizadas por un mozo y un período.
        // Técnico: aplica un filtro LINQ con condiciones sobre NombreMozo y Fecha.
        Console.WriteLine("\n=== CONSULTA 2: MOZO DÍAZ EN ABRIL 2026 ===");

        // Filtra los pedidos hechos por el mozo Díaz durante abril de 2026.
        var pedidosDiaz = registros2
            .Where(p => p.NombreMozo == "Mozo: Díaz" && p.Fecha.Month == 4 && p.Fecha.Year == 2026);

        foreach (var pedido in pedidosDiaz)
        {
            Console.WriteLine($"{pedido.Mesa} | ID: {pedido.IdPedido} | Importe: ${pedido.Precio}");
        }

        // =====================================================
        // CONSULTA 3: TOTAL FACTURADO POR PLATO
        // =====================================================
        // General: calcula cuánto se facturó en total por cada plato vendido.
        // Técnico: agrupa los pedidos por IdPlato, suma los precios y une el resultado
        // con la lista de platos para mostrar el nombre correspondiente.
        Console.WriteLine("\n=== CONSULTA 3: TOTAL FACTURADO POR PLATO ===");

        // Agrupa los pedidos por plato y calcula el total facturado por cada uno.
        var totalPorPlato = registros2
            .GroupBy(p => p.IdPlato)
            .Select(g => new
            {
                IdPlato = g.Key,
                Total = g.Sum(p => p.Precio)
            })
            .Join(registros,
                  grupo => grupo.IdPlato,
                  plato => plato.IdPlato,
                  (grupo, plato) => new { plato.Nombre, grupo.Total })
            .OrderByDescending(x => x.Total);

        foreach (var item in totalPorPlato)
        {
            Console.WriteLine($"{item.Nombre}: ${item.Total}");
        }

        // =====================================================
        // CONSULTA 4: ESTADÍSTICAS GENERALES
        // =====================================================
        // General: resume los indicadores clave del conjunto de datos.
        // Técnico: calcula contadores, promedio y máximo utilizando métodos de agregación
        // LINQ sobre las colecciones de platos y pedidos.
        Console.WriteLine("\n=== CONSULTA 4: ESTADÍSTICAS GENERALES ===");

        // Calcula métricas generales como totales, promedios y máximos.
        int totalPlatos = registros.Count();
        int cantidadPrincipales = registros.Count(p => p is PlatoPrincipal);
        int cantidadPostres = registros.Count(p => p is Postre);
        decimal precioPromedio = registros2.Average(p => p.Precio);
        decimal pedidoMasCaro = registros2.Max(p => p.Precio);

        Console.WriteLine($"Total de platos registrados: {totalPlatos}");
        Console.WriteLine($"Cantidad de platos principales: {cantidadPrincipales}");
        Console.WriteLine($"Cantidad de postres: {cantidadPostres}");
        Console.WriteLine($"Precio promedio de pedidos: {precioPromedio}");
        Console.WriteLine($"Pedido más caro: {pedidoMasCaro}");
    }
}