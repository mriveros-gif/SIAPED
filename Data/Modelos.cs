// ============================================================
//  Modelos.cs - Estructuras de datos del sistema SIAPED
//  Define las estructuras simples usadas en todos los módulos
// ============================================================

namespace SIAPED.Data
{
    // ── Mesa ────────────────────────────────────────────────
    // Estado: LIBRE | OCUPADA
    struct Mesa
    {
        public int    Numero;
        public int    Capacidad;
        public string Estado;    // "LIBRE" o "OCUPADA"
    }

    // ── Producto del Menú ───────────────────────────────────
    // Archivo menu.txt: CODIGO|NOMBRE|PRECIO|CATEGORIA
    struct Producto
    {
        public string Codigo;
        public string Nombre;
        public double Precio;
        public string Categoria;
    }

    // ── Ítem de un pedido ───────────────────────────────────
    struct ItemPedido
    {
        public string CodigoProducto;
        public string NombreProducto;
        public int    Cantidad;
        public double PrecioUnitario;
        public double Subtotal;
    }

    // ── Pedido completo de una mesa ─────────────────────────
    // Archivo pedidos.txt: ID|MESA|CLIENTE|ITEMS|TOTAL|ESTADO
    struct Pedido
    {
        public string       Id;
        public int          NumeroMesa;
        public string       NombreCliente;
        public ItemPedido[] Items;
        public double       Total;
        public string       Estado;  // "ABIERTO" | "ENVIADO" | "ENTREGADO" | "PAGADO"
    }

    // ── Inventario ──────────────────────────────────────────
    // Archivo inventario.txt: CODIGO|NOMBRE|STOCK
    struct StockProducto
    {
        public string Codigo;
        public string Nombre;
        public int    Stock;
    }

    // ── Venta registrada ────────────────────────────────────
    // Archivo ventas.txt: ID|FECHA|MESA|CLIENTE|TOTAL|METODOPAGO|TIPODOC
    struct Venta
    {
        public string Id;
        public string Fecha;
        public int    NumeroMesa;
        public string NombreCliente;
        public double Total;
        public string MetodoPago;   // "EFECTIVO" | "TARJETA" | "YAPE" | "PLIN"
        public string TipoDocumento; // "BOLETA" | "FACTURA"
        public string RucDni;
    }
}
