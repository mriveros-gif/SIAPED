// ============================================================
//  ModuloCocina.cs - Generación y preparación de órdenes
// ============================================================

using System;
using System.Collections.Generic;
using System.Threading;
using SIAPED.Data;
using SIAPED.Utils;

namespace SIAPED.Modules
{
    static class ModuloCocina
    {
        // ── Enviar orden a cocina ────────────────────────────
        public static void EnviarOrdenCocina(Pedido pedido)
        {
            Consola.MostrarEncabezado("ORDEN PARA COCINA");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ┌─────────────────────────────────────┐");
            Console.WriteLine($"  │         COMANDA DE COCINA           │");
            Console.WriteLine($"  │  Mesa: {pedido.NumeroMesa,-5}  Hora: {DateTime.Now:HH:mm}         │");
            Console.WriteLine($"  │  Pedido: {pedido.Id,-28}│");
            Console.WriteLine($"  ├─────────────────────────────────────┤");
            foreach (ItemPedido item in pedido.Items)
                Console.WriteLine($"  │  x{item.Cantidad} {item.NombreProducto,-34}│");
            Console.WriteLine($"  └─────────────────────────────────────┘");
            Console.ResetColor();

            // Construir resumen para guardar en ordenes.txt
            var partes = new List<string>();
            foreach (ItemPedido item in pedido.Items)
                partes.Add($"x{item.Cantidad} {item.NombreProducto}");
            string resumen = string.Join(", ", partes);

            Archivos.GuardarOrdenCocina(pedido.Id, pedido.NumeroMesa, resumen);
            Archivos.ActualizarEstadoPedido(pedido.Id, "ENVIADO");

            Consola.MostrarExito("Orden enviada a cocina correctamente.");
            Consola.Pausar();
        }

        // ── Simular preparación del pedido ───────────────────
        public static void PrepararPedido(Pedido pedido)
        {
            Consola.MostrarEncabezado("PREPARACIÓN DEL PEDIDO");
            Console.WriteLine($"  Mesa {pedido.NumeroMesa} - Pedido: {pedido.Id}\n");
            Consola.MostrarInfo("La cocina está preparando el pedido...\n");

            // Simulación visual de preparación ítem por ítem
            foreach (ItemPedido item in pedido.Items)
            {
                Console.Write($"  Preparando {item.NombreProducto}... ");
                Thread.Sleep(600); // Pausa breve para simular preparación
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Listo");
                Console.ResetColor();
            }

            Console.WriteLine();
            Consola.MostrarExito("¡Pedido preparado! Listo para entregar.");
            Consola.Pausar();
        }

        // ── Registrar entrega al cliente ─────────────────────
        public static void EntregarPedido(Pedido pedido)
        {
            Consola.MostrarEncabezado("ENTREGA DEL PEDIDO");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  Entregando pedido a Mesa {pedido.NumeroMesa}");
            Console.WriteLine($"  Cliente: {pedido.NombreCliente}");
            Console.ResetColor();
            Console.WriteLine();

            foreach (ItemPedido item in pedido.Items)
                Console.WriteLine($"    ✓ {item.Cantidad}x {item.NombreProducto}");

            Archivos.ActualizarEstadoPedido(pedido.Id, "ENTREGADO");
            Console.WriteLine();
            Consola.MostrarExito("Pedido entregado al cliente.");
            Consola.Pausar();
        }
    }
}
