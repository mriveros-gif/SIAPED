// ============================================================
//  Archivos.cs - Gestión de lectura y escritura de archivos
//  Maneja todos los archivos .txt del sistema
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using SIAPED.Data;

namespace SIAPED.Utils
{
    static class Archivos
    {
        // ── Rutas de archivos ────────────────────────────────
        public static readonly string RutaMesas      = "Data/mesas.txt";
        public static readonly string RutaMenu       = "Data/menu.txt";
        public static readonly string RutaPedidos    = "Data/pedidos.txt";
        public static readonly string RutaOrdenes    = "Data/ordenes.txt";
        public static readonly string RutaVentas     = "Data/ventas.txt";
        public static readonly string RutaInventario = "Data/inventario.txt";

        // ── Inicializar archivos con datos de ejemplo ────────
        public static void InicializarArchivos()
        {
            Directory.CreateDirectory("Data");

            if (!File.Exists(RutaMesas))
                File.WriteAllText(RutaMesas,
                    "1|4|LIBRE\n2|4|LIBRE\n3|4|LIBRE\n4|3|LIBRE\n5|3|LIBRE\n6|3|LIBRE\n7|3|LIBRE\n8|2|LIBRE\n9|2|LIBRE\n10|2|LIBRE");

            if (!File.Exists(RutaMenu))
                File.WriteAllText(RutaMenu,
                    "E001|Lomo Saltado|10.00|FONDO\n" +
                    "E002|Tallarines Verdes|10.00|FONDO\n" +
                    "E003|Pollo a la olla|10.00|FONDO\n" +
                  //  "P001|Arroz con Leche|12.00|POSTRE\n" +
                  //  "P002|Tres Leches|14.00|POSTRE\n" +
                    "B001|Chicha Morada|5.00|BEBIDA\n" +
                    "B002|Inca Kola Personal|5.00|BEBIDA\n" +
                    "B003|Agua Mineral|3.00|BEBIDA\n" +
                    "F001|Enrollado de atún|0.00|ENTRADA\n" +
                    "F002|Sopa de pollo|0.00|ENTRADA\n" +
                    "F003|Papa a la huancaina|0.00|ENTRADA\n");

            if (!File.Exists(RutaInventario))
                File.WriteAllText(RutaInventario,
                    "E001|Lomo Saltado|50\n" +
                    "E002|Tallarines Verdes|50\n" +
                    "E003|Pollo a la olla|50\n" +
                    //"P001|Arroz con Leche|20\n" +
                    //"P002|Tres Leches|12\n" +
                    "B001|Chicha Morada|5\n" +
                    "B002|Inca Kola Personal|5\n" +
                    "B003|Agua Mineral|3\n" +
                    "F001|Enrollado de atún |50\n" +
                    "F002|Sopa de pollo|50\n" +
                    "F003|Papa a la huancaina|50\n");

            if (!File.Exists(RutaPedidos))
                File.WriteAllText(RutaPedidos, "");

            if (!File.Exists(RutaOrdenes))
                File.WriteAllText(RutaOrdenes, "");

            if (!File.Exists(RutaVentas))
                File.WriteAllText(RutaVentas, "");
        }

        // ════════════════════════════════════════════════════
        //  MESAS
        // ════════════════════════════════════════════════════

        public static Mesa[] LeerMesas()
        {
            var lista = new List<Mesa>();
            if (!File.Exists(RutaMesas)) return lista.ToArray();

            foreach (string linea in File.ReadAllLines(RutaMesas))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                string[] partes = linea.Split('|');
                if (partes.Length < 3) continue;
                Mesa m = new Mesa
                {
                    Numero    = int.Parse(partes[0]),
                    Capacidad = int.Parse(partes[1]),
                    Estado    = partes[2].Trim()
                };
                lista.Add(m);
            }
            return lista.ToArray();
        }

        public static void GuardarMesas(Mesa[] mesas)
        {
            var lineas = new List<string>();
            foreach (Mesa m in mesas)
                lineas.Add($"{m.Numero}|{m.Capacidad}|{m.Estado}");
            File.WriteAllLines(RutaMesas, lineas);
        }

        // ════════════════════════════════════════════════════
        //  MENÚ
        // ════════════════════════════════════════════════════

        public static Producto[] LeerMenu()
        {
            var lista = new List<Producto>();
            if (!File.Exists(RutaMenu)) return lista.ToArray();

            foreach (string linea in File.ReadAllLines(RutaMenu))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                string[] partes = linea.Split('|');
                if (partes.Length < 4) continue;
                Producto p = new Producto
                {
                    Codigo    = partes[0].Trim(),
                    Nombre    = partes[1].Trim(),
                    Precio    = double.Parse(partes[2].Trim(),
                                    System.Globalization.CultureInfo.InvariantCulture),
                    Categoria = partes[3].Trim()
                };
                lista.Add(p);
            }
            return lista.ToArray();
        }

        // ════════════════════════════════════════════════════
        //  INVENTARIO
        // ════════════════════════════════════════════════════

        public static StockProducto[] LeerInventario()
        {
            var lista = new List<StockProducto>();
            if (!File.Exists(RutaInventario)) return lista.ToArray();

            foreach (string linea in File.ReadAllLines(RutaInventario))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                string[] partes = linea.Split('|');
                if (partes.Length < 3) continue;
                StockProducto s = new StockProducto
                {
                    Codigo = partes[0].Trim(),
                    Nombre = partes[1].Trim(),
                    Stock  = int.Parse(partes[2].Trim())
                };
                lista.Add(s);
            }
            return lista.ToArray();
        }

        public static void GuardarInventario(StockProducto[] stocks)
        {
            var lineas = new List<string>();
            foreach (StockProducto s in stocks)
                lineas.Add($"{s.Codigo}|{s.Nombre}|{s.Stock}");
            File.WriteAllLines(RutaInventario, lineas);
        }

        public static int ObtenerStock(string codigo)
        {
            StockProducto[] stocks = LeerInventario();
            foreach (StockProducto s in stocks)
                if (s.Codigo == codigo) return s.Stock;
            return 0;
        }

        public static void ReducirStock(string codigo, int cantidad)
        {
            StockProducto[] stocks = LeerInventario();
            for (int i = 0; i < stocks.Length; i++)
            {
                if (stocks[i].Codigo == codigo)
                {
                    stocks[i].Stock = Math.Max(0, stocks[i].Stock - cantidad);
                    break;
                }
            }
            GuardarInventario(stocks);
        }

        // ════════════════════════════════════════════════════
        //  PEDIDOS
        // ════════════════════════════════════════════════════
        // Formato línea: ID|MESA|CLIENTE|ESTADO|TOTAL|ITEM1;ITEM2...
        // Formato ítem:  CODIGO,NOMBRE,CANTIDAD,PRECIO_UNIT

        public static void GuardarPedido(Pedido pedido)
        {
            var itemsStr = new List<string>();
            foreach (ItemPedido item in pedido.Items)
                itemsStr.Add($"{item.CodigoProducto},{item.NombreProducto},{item.Cantidad},{item.PrecioUnitario.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

            string linea = $"{pedido.Id}|{pedido.NumeroMesa}|{pedido.NombreCliente}|{pedido.Estado}|{pedido.Total.ToString(System.Globalization.CultureInfo.InvariantCulture)}|{string.Join(";", itemsStr)}";
            File.AppendAllText(RutaPedidos, linea + "\n");
        }

        public static void ActualizarEstadoPedido(string idPedido, string nuevoEstado)
        {
            if (!File.Exists(RutaPedidos)) return;
            string[] lineas = File.ReadAllLines(RutaPedidos);
            for (int i = 0; i < lineas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lineas[i])) continue;
                string[] partes = lineas[i].Split('|');
                if (partes[0] == idPedido)
                {
                    partes[3] = nuevoEstado;
                    lineas[i] = string.Join("|", partes);
                }
            }
            File.WriteAllLines(RutaPedidos, lineas);
        }

        // ════════════════════════════════════════════════════
        //  ÓRDENES DE COCINA
        // ════════════════════════════════════════════════════
        // Formato: HORA|ID_PEDIDO|MESA|ITEMS_RESUMEN

        public static void GuardarOrdenCocina(string idPedido, int mesa, string resumenItems)
        {
            string hora  = DateTime.Now.ToString("HH:mm:ss");
            string linea = $"{hora}|{idPedido}|Mesa {mesa}|{resumenItems}";
            File.AppendAllText(RutaOrdenes, linea + "\n");
        }

        // ════════════════════════════════════════════════════
        //  VENTAS
        // ════════════════════════════════════════════════════
        // Formato: ID|FECHA|MESA|CLIENTE|TOTAL|METODO_PAGO|TIPO_DOC|RUC_DNI

        public static void GuardarVenta(Venta venta)
        {
            string linea = $"{venta.Id}|{venta.Fecha}|{venta.NumeroMesa}|{venta.NombreCliente}|{venta.Total.ToString(System.Globalization.CultureInfo.InvariantCulture)}|{venta.MetodoPago}|{venta.TipoDocumento}|{venta.RucDni}";
            File.AppendAllText(RutaVentas, linea + "\n");
        }

        public static Venta[] LeerVentas()
        {
            var lista = new List<Venta>();
            if (!File.Exists(RutaVentas)) return lista.ToArray();

            foreach (string linea in File.ReadAllLines(RutaVentas))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                string[] partes = linea.Split('|');
                if (partes.Length < 8) continue;
                Venta v = new Venta
                {
                    Id             = partes[0],
                    Fecha          = partes[1],
                    NumeroMesa     = int.Parse(partes[2]),
                    NombreCliente  = partes[3],
                    Total          = double.Parse(partes[4], System.Globalization.CultureInfo.InvariantCulture),
                    MetodoPago     = partes[5],
                    TipoDocumento  = partes[6],
                    RucDni         = partes[7]
                };
                lista.Add(v);
            }
            return lista.ToArray();
        }

        // ── Generar ID único basado en fecha y hora ──────────
        public static string GenerarId(string prefijo)
        {
            return prefijo + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
