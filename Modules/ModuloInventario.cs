// ============================================================
//  ModuloInventario.cs - Actualización y consulta de inventario
// ============================================================

using System;
using SIAPED.Data;
using SIAPED.Utils;

namespace SIAPED.Modules
{
    static class ModuloInventario
    {
        // ── Mostrar inventario actual ─────────────────────────
        public static void MostrarInventario()
        {
            Consola.MostrarEncabezado("INVENTARIO ACTUAL");
            StockProducto[] stocks = Archivos.LeerInventario();

            Console.WriteLine($"  {"Código",-8} {"Producto",-32} {"Stock",6}");
            Consola.MostrarSeparador();

            foreach (StockProducto s in stocks)
            {
                ConsoleColor color = s.Stock > 5 ? ConsoleColor.Green
                                   : s.Stock > 0 ? ConsoleColor.Yellow
                                   : ConsoleColor.Red;
                Console.ForegroundColor = color;
                string estado = s.Stock == 0 ? " ← AGOTADO" : s.Stock <= 3 ? " ← POCO STOCK" : "";
                Console.WriteLine($"  {s.Codigo,-8} {s.Nombre,-32} {s.Stock,6}{estado}");
                Console.ResetColor();
            }

            Consola.Pausar();
        }

        // ── Descontar stock al confirmar venta ───────────────
        public static void ActualizarInventario(ItemPedido[] items)
        {
            Consola.MostrarEncabezado("ACTUALIZACIÓN DE INVENTARIO");

            foreach (ItemPedido item in items)
            {
                int stockAntes = Archivos.ObtenerStock(item.CodigoProducto);
                Archivos.ReducirStock(item.CodigoProducto, item.Cantidad);
                int stockDespues = Archivos.ObtenerStock(item.CodigoProducto);

                Console.WriteLine($"  {item.NombreProducto,-30} Stock: {stockAntes} → {stockDespues}");

                if (stockDespues == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"    [!] ALERTA: '{item.NombreProducto}' quedó agotado.");
                    Console.ResetColor();
                }
                else if (stockDespues <= 2)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"    [i] AVISO: Poco stock de '{item.NombreProducto}'.");
                    Console.ResetColor();
                }
            }

            Consola.MostrarExito("Inventario actualizado.");
            Consola.Pausar();
        }
    }
}
