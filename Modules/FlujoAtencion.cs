// ============================================================
//  FlujoAtencion.cs - Orquestador del flujo completo
//  Implementa paso a paso el diagrama de flujo del restaurante:
//  cliente llega → asignar mesa → registrar pedido →
//  verificar disponibilidad → generar orden cocina →
//  preparar → entregar → pedido adicional? →
//  calcular cuenta → cobrar → emitir comprobante →
//  actualizar inventario → registrar venta → liberar mesa
// ============================================================

using System;
using System.Collections.Generic;
using SIAPED.Data;
using SIAPED.Utils;

namespace SIAPED.Modules
{
    static class FlujoAtencion
    {
        public static void IniciarAtencion()
        {
            // ══════════════════════════════════════════
            //  PASO 1: Cliente llega - datos iniciales
            // ══════════════════════════════════════════
            Consola.MostrarEncabezado("NUEVA ATENCIÓN AL CLIENTE");

            Console.WriteLine("  Bienvenido al sistema SIAPED.");
            Console.WriteLine("  Complete los datos del cliente para comenzar.\n");

            string nombreCliente = Consola.LeerTexto("Nombre del cliente");
            int    numPersonas   = Consola.LeerEntero("Número de personas en el grupo", 1, 20);


            // ══════════════════════════════════════════
            //  PASO 2: Asignar mesa
            // ══════════════════════════════════════════
            int numMesa = ModuloMesas.AsignarMesa(numPersonas);

            if (numMesa == -1)
            {
                Consola.MostrarInfo("No se pudo iniciar la atención. No hay mesa disponible.");
                Consola.Pausar();
                return;
            }

            // ══════════════════════════════════════════
            //  PASO 3 y 4: Registrar pedido con
            //  verificación de disponibilidad integrada
            // ══════════════════════════════════════════
            Pedido pedido = ModuloPedidos.RegistrarPedido(numMesa, nombreCliente);

            if (pedido.Items == null || pedido.Items.Length == 0)
            {
                Consola.MostrarInfo("No se registró ningún pedido. Liberando mesa.");
                ModuloMesas.LiberarMesa(numMesa);
                Consola.Pausar();
                return;
            }

            // ¿Desea modificar el pedido antes de enviarlo?
            bool modificar = Consola.Confirmar("¿Desea modificar el pedido antes de enviarlo?");
            if (modificar)
            {
                pedido.Items = ModuloPedidos.ModificarPedido(pedido.Items);
                pedido.Total = ModuloPedidos.CalcularTotal(pedido.Items);
            }

            // Guardar pedido en archivo
            Archivos.GuardarPedido(pedido);

            // ══════════════════════════════════════════
            //  PASO 5: Generar orden para cocina
            // ══════════════════════════════════════════
            ModuloCocina.EnviarOrdenCocina(pedido);

            // ══════════════════════════════════════════
            //  PASO 6: Preparar pedido
            // ══════════════════════════════════════════
            ModuloCocina.PrepararPedido(pedido);

            // ══════════════════════════════════════════
            //  PASO 7: Entregar pedido al cliente
            // ══════════════════════════════════════════
            ModuloCocina.EntregarPedido(pedido);

            // ══════════════════════════════════════════
            //  PASO 8: ¿Desea agregar algo más?
            // ══════════════════════════════════════════
            bool agregarMas = Consola.Confirmar("¿El cliente desea agregar algo más?");

            if (agregarMas)
            {
                ItemPedido[] nuevosItems = ModuloPedidos.RegistrarPedidoAdicional(pedido.Items);
                pedido.Items = nuevosItems;
                pedido.Total = ModuloPedidos.CalcularTotal(nuevosItems);

                // Enviar adición a cocina
                Pedido pedidoAdicional = new Pedido
                {
                    Id            = Archivos.GenerarId("ADD"),
                    NumeroMesa    = pedido.NumeroMesa,
                    NombreCliente = pedido.NombreCliente,
                    Items         = pedido.Items,
                    Total         = pedido.Total,
                    Estado        = "ABIERTO"
                };
                Archivos.GuardarPedido(pedidoAdicional);
                ModuloCocina.EnviarOrdenCocina(pedidoAdicional);
                ModuloCocina.PrepararPedido(pedidoAdicional);
                ModuloCocina.EntregarPedido(pedidoAdicional);
            }

            // ══════════════════════════════════════════
            //  PASO 9: Calcular consumo total y
            //          generar cuenta
            // ══════════════════════════════════════════
            ModuloPago.GenerarCuenta(pedido);

            // ══════════════════════════════════════════
            //  PASO 10: Seleccionar y validar pago
            // ══════════════════════════════════════════
            string metodoPago;
            bool   pagoValido = false;

            do
            {
                metodoPago = ModuloPago.SeleccionarMetodoPago();
                pagoValido = ModuloPago.ValidarPago(metodoPago, pedido.Total);

                if (!pagoValido)
                    Consola.MostrarInfo("Intente nuevamente con otro método de pago.");

            } while (!pagoValido);

            // ══════════════════════════════════════════
            //  PASO 11: Emitir boleta o factura
            // ══════════════════════════════════════════
            (string tipoDoc, string rucDni) = ModuloPago.SeleccionarTipoDocumento();
            ModuloPago.EmitirComprobante(pedido, metodoPago, tipoDoc, rucDni);
            Consola.Pausar();

            // ══════════════════════════════════════════
            //  PASO 12: Actualizar inventario
            // ══════════════════════════════════════════
            ModuloInventario.ActualizarInventario(pedido.Items);

            // ══════════════════════════════════════════
            //  PASO 13: Registrar venta
            // ══════════════════════════════════════════
            ModuloVentas.RegistrarVenta(pedido, metodoPago, tipoDoc, rucDni);
            Consola.Pausar();

            // ══════════════════════════════════════════
            //  PASO 14: Liberar mesa
            // ══════════════════════════════════════════
            ModuloMesas.LiberarMesa(numMesa);

            Consola.MostrarEncabezado("ATENCIÓN FINALIZADA");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ¡Gracias por su visita, {nombreCliente}!");
            Console.WriteLine($"  La Mesa {numMesa} ha sido liberada.");
            Console.ResetColor();
            Consola.Pausar();
        }
    }
}
