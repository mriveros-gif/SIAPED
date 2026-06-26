using System;
using SIAPED.Modules;
using SIAPED.Utils;

namespace SIAPED
{
    class Program
    {
        static void Main(string[] args)
        {
            Consola.ConfigurarConsola();
            Archivos.InicializarArchivos();

            bool salir = false;

            while (!salir)
            {
                Consola.MostrarEncabezado("MENÚ PRINCIPAL");
                Console.WriteLine("  [1] Iniciar atención a cliente");
                Console.WriteLine("  [2] Ver mesas disponibles");
                Console.WriteLine("  [3] Ver menú del restaurante");
                Console.WriteLine("  [4] Ver inventario");
                Console.WriteLine("  [5] Ver registro de ventas");
                Console.WriteLine("  [0] Salir del sistema");
                Console.WriteLine();
                Console.Write("  Seleccione una opción: ");

                string opcion = Console.ReadLine()?.Trim() ?? "";

                switch (opcion)
                {
                    case "1":
                        FlujoAtencion.IniciarAtencion();
                        break;
                    case "2":
                        ModuloMesas.MostrarMesas();
                        break;
                    case "3":
                        ModuloMenu.MostrarMenu();
                        break;
                    case "4":
                        ModuloInventario.MostrarInventario();
                        break;
                    case "5":
                        ModuloVentas.MostrarVentas();
                        break;
                    case "0":
                        salir = true;
                        Console.WriteLine();
                        Console.WriteLine("  Cerrando SIAPED... ¡Hasta luego!");
                        break;
                    default:
                        Consola.MostrarError("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }
    }
}