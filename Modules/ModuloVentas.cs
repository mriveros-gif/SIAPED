// ============================================================
//  ModuloVentas.cs - Registro y consulta de ventas del día
// ============================================================

using System;
using SIAPED.Data;
using SIAPED.Utils;

namespace SIAPED.Modules
{
    static class ModuloVentas
    {
        // ── Registrar venta en ventas.txt ────────────────────
        public static void RegistrarVenta(Pedido pedido, string metodoPago,
                                          string tipoDoc, string rucDni)
        {
            Venta venta = new Venta
            {
                Id            = Archivos.GenerarId("VTA"),
                Fecha         = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                NumeroMesa    = pedido.NumeroMesa,
                NombreCliente = pedido.NombreCliente,
                Total         = pedido.Total,
                MetodoPago    = metodoPago,
                TipoDocumento = tipoDoc,
                RucDni        = rucDni
            };

            Archivos.GuardarVenta(venta);
            Archivos.ActualizarEstadoPedido(pedido.Id, "PAGADO");
            Consola.MostrarExito($"Venta registrada con ID: {venta.Id}");
        }

        // ── Mostrar reporte de ventas ────────────────────────
        public static void MostrarVentas()
        {
            Consola.MostrarEncabezado("REGISTRO DE VENTAS");
            Venta[] ventas = Archivos.LeerVentas();

            if (ventas.Length == 0)
            {
                Consola.MostrarInfo("No hay ventas registradas.");
                Consola.Pausar();
                return;
            }

            Console.WriteLine($"  {"ID Venta",-20} {"Fecha",-18} {"Mesa",6} {"Cliente",-18} {"Total",8} {"Método",-10} {"Doc",-8}");
            Consola.MostrarSeparador();

            double totalDia = 0;
            foreach (Venta v in ventas)
            {
                Console.WriteLine($"  {v.Id,-20} {v.Fecha,-18} {v.NumeroMesa,6} {v.NombreCliente,-18} S/{v.Total,6:F2} {v.MetodoPago,-10} {v.TipoDocumento,-8}");
                totalDia += v.Total;
            }

            Consola.MostrarSeparador();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  Total de ventas: {ventas.Length}   Total recaudado: S/ {totalDia:F2}");
            Console.ResetColor();
            Consola.Pausar();
        }
    }
}
