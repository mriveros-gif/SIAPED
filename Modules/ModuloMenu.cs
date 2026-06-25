// ============================================================
//  ModuloMenu.cs - Visualización del menú del restaurante
// ============================================================

using System;
using SIAPED.Data;
using SIAPED.Utils;

namespace SIAPED.Modules
{
    static class ModuloMenu
    {
        // ── Mostrar menú completo agrupado por categoría ─────
        public static void MostrarMenu()
        {
            Consola.MostrarEncabezado("MENÚ DEL RESTAURANTE");
            Producto[] productos = Archivos.LeerMenu();

            
            string[] categorias = { "ENTRADA", "FONDO",  "BEBIDA" };   //"POSTRE",

            foreach (string cat in categorias)
            {
                Consola.MostrarSeparador(cat + "S");
                foreach (Producto p in productos)
                {
                    if (p.Categoria == cat)
                    {
                        int stock = Archivos.ObtenerStock(p.Codigo);
                        ConsoleColor color = stock > 0 ? ConsoleColor.White : ConsoleColor.DarkGray;
                        Console.ForegroundColor = color;
                        string disponible = stock > 0 ? "" : " [AGOTADO]";
                        Console.WriteLine($"  [{p.Codigo}] {p.Nombre,-30} S/ {p.Precio,6:F2}{disponible}");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
            Consola.Pausar();
        }

        // ── Buscar producto por código ───────────────────────
        public static bool BuscarProducto(string codigo, out Producto encontrado)
        {
            encontrado = new Producto();
            Producto[] productos = Archivos.LeerMenu();
            foreach (Producto p in productos)
            {
                if (p.Codigo.ToUpper() == codigo.ToUpper())
                {
                    encontrado = p;
                    return true;
                }
            }
            return false;
        }

        // ── Mostrar alternativas de la misma categoría ───────
        public static void MostrarAlternativas(string categoria, string codigoAgotado)
        {
            Consola.MostrarSeparador("PRODUCTOS ALTERNATIVOS");
            Producto[] productos = Archivos.LeerMenu();
            bool hayAlternativas = false;

            Console.WriteLine($"  Alternativas disponibles en {categoria}:\n");
            foreach (Producto p in productos)
            {
                if (p.Categoria == categoria && p.Codigo != codigoAgotado)
                {
                    int stock = Archivos.ObtenerStock(p.Codigo);
                    if (stock > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"  [{p.Codigo}] {p.Nombre,-30} S/ {p.Precio,6:F2}");
                        Console.ResetColor();
                        hayAlternativas = true;
                    }
                }
            }

            if (!hayAlternativas)
                Consola.MostrarInfo("No hay alternativas disponibles en esta categoría.");
        }
    }
}
