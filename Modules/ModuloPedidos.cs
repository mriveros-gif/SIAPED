// ============================================================
//  ModuloPedidos.cs - Registro, modificación y gestión de pedidos
// ============================================================

using System;
using System.Collections.Generic;
using SIAPED.Data;
using SIAPED.Utils;

namespace SIAPED.Modules
{
    static class ModuloPedidos
    {
        // ════════════════════════════════════════════════════
        //  REGISTRAR NUEVO PEDIDO
        //  Retorna el pedido construido (aún no guardado en disco)
        // ════════════════════════════════════════════════════
        public static Pedido RegistrarPedido(int numMesa, string nombreCliente)
        {
            Consola.MostrarEncabezado("REGISTRO DE PEDIDO");
            Console.WriteLine($"  Mesa: {numMesa}   Cliente: {nombreCliente}\n");

            Pedido pedido = new Pedido
            {
                Id            = Archivos.GenerarId("PED"),
                NumeroMesa    = numMesa,
                NombreCliente = nombreCliente,
                Estado        = "ABIERTO"
            };

            List<ItemPedido> items = new List<ItemPedido>();
            bool agregarMas = true;

            while (agregarMas)
            {
                ModuloMenu.MostrarMenu();
                Consola.MostrarEncabezado("AGREGAR ÍTEM AL PEDIDO");
                Console.WriteLine($"  Pedido actual: {items.Count} ítem(s)\n");

                Console.Write("  Ingrese código del producto (o 0 para terminar): ");
                string codigo = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (codigo == "0") break;

                // Verificar que el producto existe en el menú
                if (!ModuloMenu.BuscarProducto(codigo, out Producto producto))
                {
                    Consola.MostrarError("Código de producto no encontrado. Intente de nuevo.");
                    continue;
                }

                // Verificar disponibilidad en inventario
                int stock = Archivos.ObtenerStock(codigo);
                if (stock == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n  [!] '{producto.Nombre}' está agotado.");
                    Console.ResetColor();

                    ModuloMenu.MostrarAlternativas(producto.Categoria, codigo);
                    Console.WriteLine();
                    Consola.MostrarInfo("Puede seleccionar una alternativa o continuar con otro producto.");
                    Consola.Pausar();
                    continue;
                }

                int cantidad = Consola.LeerEntero("Cantidad", 1, stock);

                ItemPedido item = new ItemPedido
                {
                    CodigoProducto  = producto.Codigo,
                    NombreProducto  = producto.Nombre,
                    Cantidad        = cantidad,
                    PrecioUnitario  = producto.Precio,
                    Subtotal        = producto.Precio * cantidad
                };

                items.Add(item);
                Consola.MostrarExito($"'{producto.Nombre}' x{cantidad} agregado al pedido.");

                MostrarResumenPedido(items);
                agregarMas = Consola.Confirmar("\n  ¿Desea agregar otro producto?");
            }

            if (items.Count == 0)
            {
                Consola.MostrarInfo("No se registró ningún ítem en el pedido.");
                pedido.Items = new ItemPedido[0];
                pedido.Total = 0;
                return pedido;
            }

            pedido.Items = items.ToArray();
            pedido.Total = CalcularTotal(items);
            return pedido;
        }

        // ════════════════════════════════════════════════════
        //  MODIFICAR PEDIDO (quitar ítems)
        // ════════════════════════════════════════════════════
        public static ItemPedido[] ModificarPedido(ItemPedido[] itemsActuales)
        {
            Consola.MostrarEncabezado("MODIFICAR PEDIDO");
            MostrarResumenPedido(new List<ItemPedido>(itemsActuales));

            if (itemsActuales.Length == 0)
            {
                Consola.MostrarInfo("El pedido está vacío.");
                Consola.Pausar();
                return itemsActuales;
            }

            Console.WriteLine();
            Console.WriteLine("  ¿Qué desea hacer?");
            Console.WriteLine("  [1] Eliminar un ítem");
            Console.WriteLine("  [2] Cambiar cantidad de un ítem");
            Console.WriteLine("  [0] Cancelar (sin cambios)");
            Console.Write("\n  Opción: ");
            string op = Console.ReadLine()?.Trim() ?? "0";

            var lista = new List<ItemPedido>(itemsActuales);

            if (op == "1")
            {
                int idx = Consola.LeerEntero("Número de ítem a eliminar", 1, lista.Count);
                Consola.MostrarInfo($"Eliminando: {lista[idx - 1].NombreProducto}");
                lista.RemoveAt(idx - 1);
                Consola.MostrarExito("Ítem eliminado.");
            }
            else if (op == "2")
            {
                int idx = Consola.LeerEntero("Número de ítem a modificar", 1, lista.Count);
                int stockDisp = Archivos.ObtenerStock(lista[idx - 1].CodigoProducto);
                int nuevaCant = Consola.LeerEntero("Nueva cantidad", 1, stockDisp > 0 ? stockDisp : 99);
                ItemPedido item = lista[idx - 1];
                item.Cantidad  = nuevaCant;
                item.Subtotal  = item.PrecioUnitario * nuevaCant;
                lista[idx - 1] = item;
                Consola.MostrarExito("Cantidad actualizada.");
            }

            Consola.Pausar();
            return lista.ToArray();
        }

        // ════════════════════════════════════════════════════
        //  PEDIDO ADICIONAL (agregar más ítems)
        // ════════════════════════════════════════════════════
        public static ItemPedido[] RegistrarPedidoAdicional(ItemPedido[] itemsActuales)
        {
            Consola.MostrarEncabezado("PEDIDO ADICIONAL");
            Console.WriteLine("  El cliente desea agregar más productos.\n");

            var lista = new List<ItemPedido>(itemsActuales);
            bool agregarMas = true;

            while (agregarMas)
            {
                ModuloMenu.MostrarMenu();
                Consola.MostrarEncabezado("AGREGAR ÍTEM ADICIONAL");

                Console.Write("  Código del producto (0 para terminar): ");
                string codigo = Console.ReadLine()?.Trim().ToUpper() ?? "";
                if (codigo == "0") break;

                if (!ModuloMenu.BuscarProducto(codigo, out Producto producto))
                {
                    Consola.MostrarError("Código no encontrado.");
                    continue;
                }

                int stock = Archivos.ObtenerStock(codigo);
                if (stock == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n  [!] '{producto.Nombre}' agotado.");
                    Console.ResetColor();
                    ModuloMenu.MostrarAlternativas(producto.Categoria, codigo);
                    Consola.Pausar();
                    continue;
                }

                int cantidad = Consola.LeerEntero("Cantidad", 1, stock);

                // Si ya existe ese producto en el pedido, sumar cantidad
                bool encontrado = false;
                for (int i = 0; i < lista.Count; i++)
                {
                    if (lista[i].CodigoProducto == codigo)
                    {
                        ItemPedido item = lista[i];
                        item.Cantidad += cantidad;
                        item.Subtotal  = item.PrecioUnitario * item.Cantidad;
                        lista[i]       = item;
                        encontrado     = true;
                        break;
                    }
                }

                if (!encontrado)
                {
                    lista.Add(new ItemPedido
                    {
                        CodigoProducto = producto.Codigo,
                        NombreProducto = producto.Nombre,
                        Cantidad       = cantidad,
                        PrecioUnitario = producto.Precio,
                        Subtotal       = producto.Precio * cantidad
                    });
                }

                Consola.MostrarExito($"'{producto.Nombre}' x{cantidad} agregado.");
                MostrarResumenPedido(lista);
                agregarMas = Consola.Confirmar("\n  ¿Desea agregar algo más?");
            }

            return lista.ToArray();
        }

        // ════════════════════════════════════════════════════
        //  MOSTRAR RESUMEN DEL PEDIDO
        // ════════════════════════════════════════════════════
        public static void MostrarResumenPedido(List<ItemPedido> items)
        {
            Console.WriteLine();
            Consola.MostrarSeparador("PEDIDO ACTUAL");
            Console.WriteLine($"  {"#",-4} {"Producto",-28} {"Cant",-6} {"P.Unit",-10} {"Subtotal"}");
            Consola.MostrarSeparador();

            int num = 1;
            foreach (ItemPedido item in items)
            {
                Console.WriteLine($"  {num,-4} {item.NombreProducto,-28} {item.Cantidad,-6} S/{item.PrecioUnitario,-8:F2} S/{item.Subtotal:F2}");
                num++;
            }

            Consola.MostrarSeparador();
            double total = CalcularTotal(items);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  {"TOTAL:",-44} S/{total:F2}");
            Console.ResetColor();
        }

        // ── Calcular total de la lista de ítems ──────────────
        public static double CalcularTotal(List<ItemPedido> items)
        {
            double total = 0;
            foreach (ItemPedido item in items) total += item.Subtotal;
            return total;
        }

        public static double CalcularTotal(ItemPedido[] items)
        {
            double total = 0;
            foreach (ItemPedido item in items) total += item.Subtotal;
            return total;
        }
    }
}
