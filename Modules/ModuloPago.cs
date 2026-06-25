// ============================================================
//  ModuloPago.cs - Cálculo de cuenta, cobro y emisión de comprobante
// ============================================================

using System;
using SIAPED.Data;
using SIAPED.Utils;

namespace SIAPED.Modules
{
    static class ModuloPago
    {
        // ════════════════════════════════════════════════════
        //  GENERAR CUENTA DETALLADA
        // ════════════════════════════════════════════════════
        public static void GenerarCuenta(Pedido pedido)
        {
            Consola.MostrarEncabezado("CUENTA DEL CLIENTE");

            Console.WriteLine($"  Mesa: {pedido.NumeroMesa}   Cliente: {pedido.NombreCliente}");
            Console.WriteLine($"  Pedido: {pedido.Id}   Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
            Console.WriteLine();

            Console.WriteLine($"  {"Producto",-30} {"Cant",4} {"P.Unit",8} {"Subtotal",10}");
            Consola.MostrarSeparador();

            double subtotalSinIgv = 0;
            foreach (ItemPedido item in pedido.Items)
            {
                Console.WriteLine($"  {item.NombreProducto,-30} {item.Cantidad,4} S/{item.PrecioUnitario,6:F2} S/{item.Subtotal,8:F2}");
                subtotalSinIgv += item.Subtotal;
            }

            Consola.MostrarSeparador();

            double igv   = subtotalSinIgv * 0.18;
            double total = subtotalSinIgv; // Precio ya incluye IGV en este modelo

            Console.WriteLine($"  {"Subtotal:",-44} S/{subtotalSinIgv / 1.18,8:F2}");
            Console.WriteLine($"  {"IGV (18%):",-44} S/{subtotalSinIgv - (subtotalSinIgv / 1.18),8:F2}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  {"TOTAL A PAGAR:",-44} S/{total,8:F2}");
            Console.ResetColor();
            Console.WriteLine();
        }

        // ════════════════════════════════════════════════════
        //  SELECCIONAR MÉTODO DE PAGO
        // ════════════════════════════════════════════════════
        public static string SeleccionarMetodoPago()
        {
            Consola.MostrarSeparador("MÉTODO DE PAGO");
            Console.WriteLine("  [1] Efectivo");
            Console.WriteLine("  [2] Tarjeta (débito/crédito)");
            Console.WriteLine("  [3] Yape");
            Console.WriteLine("  [4] Plin");
            Console.WriteLine();

            int opcion = Consola.LeerEntero("Seleccione método de pago", 1, 4);

            switch (opcion)
            {
                case 1: return "EFECTIVO";
                case 2: return "TARJETA";
                case 3: return "YAPE";
                case 4: return "PLIN";
                default: return "EFECTIVO";
            }
        }

        // ════════════════════════════════════════════════════
        //  VALIDAR PAGO
        //  Retorna true si el pago fue validado correctamente
        // ════════════════════════════════════════════════════
        public static bool ValidarPago(string metodoPago, double totalCobrar)
        {
            Consola.MostrarSeparador("VALIDACIÓN DE PAGO");
            Console.WriteLine($"  Método: {metodoPago}");
            Console.WriteLine($"  Total:  S/ {totalCobrar:F2}\n");

            if (metodoPago == "EFECTIVO")
            {
                double montoEntregado = 0;
                bool montoValido = false;

                while (!montoValido)
                {
                    montoEntregado = Consola.LeerDecimal("Monto entregado por el cliente (S/)", 0);
                    if (montoEntregado < totalCobrar)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"  [!] Monto insuficiente. Faltan S/ {totalCobrar - montoEntregado:F2}");
                        Console.ResetColor();
                    }
                    else
                    {
                        montoValido = true;
                    }
                }

                double vuelto = montoEntregado - totalCobrar;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  Monto recibido: S/ {montoEntregado:F2}");
                Console.WriteLine($"  Vuelto:         S/ {vuelto:F2}");
                Console.ResetColor();
                return true;
            }
            else
            {
                // Para pagos digitales: confirmación manual
                Consola.MostrarInfo($"Esperando confirmación de pago por {metodoPago}...");
                bool confirmado = Consola.Confirmar($"¿Se confirmó el pago de S/ {totalCobrar:F2} por {metodoPago}?");

                if (confirmado)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n  Pago por {metodoPago} confirmado correctamente.");
                    Console.ResetColor();
                    return true;
                }
                else
                {
                    Consola.MostrarError("Pago no confirmado. Intente con otro método.");
                    return false;
                }
            }
        }

        // ════════════════════════════════════════════════════
        //  SELECCIONAR TIPO DE DOCUMENTO
        // ════════════════════════════════════════════════════
        public static (string tipoDoc, string rucDni) SeleccionarTipoDocumento()
        {
            Consola.MostrarSeparador("TIPO DE COMPROBANTE");
            Console.WriteLine("  [1] Boleta de Venta");
            Console.WriteLine("  [2] Factura");
            Console.WriteLine();

            int opcion = Consola.LeerEntero("Seleccione tipo de comprobante", 1, 2);
            string tipo = opcion == 1 ? "BOLETA" : "FACTURA";
            string rucDni;

            if (tipo == "BOLETA")
            {
                Console.Write("  DNI del cliente (opcional, ENTER para omitir): ");
                rucDni = Console.ReadLine()?.Trim() ?? "00000000";
                if (string.IsNullOrWhiteSpace(rucDni)) rucDni = "00000000";
            }
            else
            {
                // Factura requiere RUC con validación básica
                rucDni = "";
                while (rucDni.Length != 11 || !long.TryParse(rucDni, out _))
                {
                    Console.Write("  RUC de la empresa (11 dígitos): ");
                    rucDni = Console.ReadLine()?.Trim() ?? "";
                    if (rucDni.Length != 11 || !long.TryParse(rucDni, out _))
                        Consola.MostrarInfo("El RUC debe tener exactamente 11 dígitos numéricos.");
                }
            }

            return (tipo, rucDni);
        }

        // ════════════════════════════════════════════════════
        //  EMITIR COMPROBANTE (Boleta o Factura)
        // ════════════════════════════════════════════════════
        //public static void EmitirComprobante(Pedido pedido, string metodoPago,
        //                                      string tipoDoc, string rucDni)
        //{
        //    Consola.MostrarEncabezado($"EMISIÓN DE {tipoDoc}");

        //    string fecha     = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        //    string numComp   = Archivos.GenerarId(tipoDoc == "BOLETA" ? "B" : "F");

        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine();
        //    Console.WriteLine("  ╔══════════════════════════════════════════╗");
        //    Console.WriteLine($"  ║           RESTAURANTE SIAPED            ║");
        //    Console.WriteLine($"  ║         {tipoDoc} DE VENTA              ║");
        //    Console.WriteLine($"  ║  N°: {numComp,-37}║");
        //    Console.WriteLine($"  ║  Fecha: {fecha,-35}║");
        //    Console.WriteLine($"  ╠══════════════════════════════════════════╣");
        //    Console.WriteLine($"  ║  Mesa: {pedido.NumeroMesa,-36}║");
        //    Console.WriteLine($"  ║  Cliente: {pedido.NombreCliente,-33}║");
        //    if (tipoDoc == "FACTURA")
        //        Console.WriteLine($"  ║  RUC: {rucDni,-37}║");
        //    else
        //        Console.WriteLine($"  ║  DNI: {rucDni,-37}║");
        //    Console.WriteLine($"  ╠══════════════════════════════════════════╣");

        //    foreach (ItemPedido item in pedido.Items)
        //        Console.WriteLine($"  ║  {item.Cantidad}x {item.NombreProducto,-28} S/{item.Subtotal,5:F2} ║");

        //    Console.WriteLine($"  ╠══════════════════════════════════════════╣");

        //    double baseImponible = pedido.Total / 1.18;
        //    double igv           = pedido.Total - baseImponible;

        //    Console.WriteLine($"  ║  OP. GRAVADA:                  S/{baseImponible,7:F2} ║");
        //    Console.WriteLine($"  ║  IGV 18%:                      S/{igv,7:F2} ║");
        //    Console.WriteLine($"  ║  TOTAL:                        S/{pedido.Total,7:F2} ║");
        //    Console.WriteLine($"  ║  Método de pago: {metodoPago,-24}║");
        //    Console.WriteLine($"  ╚══════════════════════════════════════════╝");
        //    Console.ResetColor();
        //    Console.WriteLine();
        //    Consola.MostrarExito($"{tipoDoc} emitida correctamente.");




        //    string borde = "  ╔══════════════════════════════════════════╗";
        //    Console.WriteLine(borde.Length);
        //    string linea = $"  ║  N°: {numComp,-37}║";
        //    Console.WriteLine(linea.Length);

        //}
        const int ANCHO = 42;
        public static void EmitirComprobante(Pedido pedido, string metodoPago,string tipoDoc,string rucDni)
        {
            Consola.MostrarEncabezado($"EMISIÓN DE {tipoDoc}");

            string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string numComp = Archivos.GenerarId(tipoDoc == "BOLETA" ? "B" : "F");

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine();
            Console.WriteLine($"  ╔{new string('═', ANCHO)}╗");

            Linea("        RESTAURANTE DON JULIO");
            Linea($"        {tipoDoc} DE VENTA");
            Linea($"  N°: {numComp}");
            Linea($"  Fecha: {fecha}");

            Console.WriteLine($"  ╠{new string('═', ANCHO)}╣");

            Linea($"  Mesa: {pedido.NumeroMesa}");
            Linea($"  Cliente: {pedido.NombreCliente}");

            if (tipoDoc == "FACTURA")
                Linea($"  RUC: {rucDni}");
            else
                Linea($"  DNI: {rucDni}");

            Console.WriteLine($"  ╠{new string('═', ANCHO)}╣");

            foreach (ItemPedido item in pedido.Items)
            {
                string nombre = item.NombreProducto.Length > 20
                    ? item.NombreProducto[..20]
                    : item.NombreProducto;

                Linea($"  {item.Cantidad}x {nombre} S/{item.Subtotal:F2}");
            }

            Console.WriteLine($"  ╠{new string('═', ANCHO)}╣");

            double baseImponible = pedido.Total / 1.18;
            double igv = pedido.Total - baseImponible;

            Linea($"  OP. GRAVADA: S/{baseImponible:F2}");
            Linea($"  IGV 18%:     S/{igv:F2}");
            Linea($"  TOTAL:       S/{pedido.Total:F2}");
            Linea($"  Método de pago: {metodoPago}");

            Console.WriteLine($"  ╚{new string('═', ANCHO)}╝");

            Console.ResetColor();
            Console.WriteLine();

            Consola.MostrarExito($"{tipoDoc} emitida correctamente.");
        }

        static void Linea(string texto = "")
        {
            Console.WriteLine($"  ║{texto.PadRight(ANCHO)}║");
        }


    }
}
