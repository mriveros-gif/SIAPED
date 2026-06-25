// ============================================================
//  Consola.cs - Utilidades de presentación en pantalla
//  Centraliza colores, bordes y mensajes del sistema
// ============================================================

using System;

namespace SIAPED.Utils
{
    static class Consola
    {
        // ── Configuración inicial de la consola ─────────────
        public static void ConfigurarConsola()
        {
            Console.Title = "SIAPED - Sistema de Atención y Pedidos";
            try { Console.OutputEncoding = System.Text.Encoding.UTF8; } catch { }
        }

        // ── Encabezado con borde ─────────────────────────────
        public static void MostrarEncabezado(string titulo)
        {
            string name = "RESTAURANT \"DON JULIO\"";
            string sistema = "SIAPED - Sistema de Atención y Pedidos";
            Console.Clear();
            string borde = new string('═', 60);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  ╔{borde}╗");
            Console.WriteLine($"  ║{name.PadLeft((60 + name.Length) / 2).PadRight(60)}║");
            //Console.WriteLine($"  ║{"SIAPED - Sistema de Atención y Pedidos".PadLeft(39).PadRight(60)}║");
            Console.WriteLine($"  ║{sistema.PadLeft((60 + sistema.Length) / 2).PadRight(60)}║");
            Console.WriteLine($"  ║{titulo.PadLeft((60 + titulo.Length) / 2).PadRight(60)}║");
            Console.WriteLine($"  ╚{borde}╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        // ── Separador de sección ─────────────────────────────
        public static void MostrarSeparador(string seccion = "")
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            if (seccion == "")
                Console.WriteLine("  " + new string('─', 58));
            else
                Console.WriteLine($"  ── {seccion} " + new string('─', Math.Max(0, 54 - seccion.Length)));
            Console.ResetColor();
        }

        // ── Mensaje de error ─────────────────────────────────
        public static void MostrarError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n  [!] {mensaje}");
            Console.ResetColor();
            Pausar();
        }

        // ── Mensaje de éxito ─────────────────────────────────
        public static void MostrarExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  [✓] {mensaje}");
            Console.ResetColor();
        } 

        // ── Mensaje informativo ──────────────────────────────
        public static void MostrarInfo(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  [i] {mensaje}");
            Console.ResetColor();
        }

        // ── Pausa con mensaje ────────────────────────────────
        public static void Pausar(string mensaje = "Presione ENTER para continuar...")
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"\n  {mensaje}");
            Console.ResetColor();
            Console.ReadLine();
        }

        // ── Leer texto con validación no vacío ───────────────
        public static string LeerTexto(string etiqueta, int maxLen = 50)
        {
            string valor = "";
            while (string.IsNullOrWhiteSpace(valor))
            {
                Console.Write($"  {etiqueta}: ");
                valor = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(valor))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  [!] El campo no puede estar vacío.");
                    Console.ResetColor();
                }
                else if (valor.Length > maxLen)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [!] Máximo {maxLen} caracteres.");
                    Console.ResetColor();
                    valor = "";
                }
            }
            return valor;
        }

        // ── Leer entero con rango ────────────────────────────
        public static int LeerEntero(string etiqueta, int min, int max)
        {
            int valor = -1;
            bool valido = false;
            while (!valido)
            {
                Console.Write($"  {etiqueta} ({min}-{max}): ");
                string entrada = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(entrada, out valor) && valor >= min && valor <= max)
                    valido = true;
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [!] Ingrese un número entre {min} y {max}.");
                    Console.ResetColor();
                }
            }
            return valor;
        }

        // ── Leer decimal positivo ────────────────────────────
        public static double LeerDecimal(string etiqueta, double min = 0)
        {
            double valor = -1;
            bool valido = false;
            while (!valido)
            {
                Console.Write($"  {etiqueta}: ");
                string entrada = Console.ReadLine()?.Trim() ?? "";
                if (double.TryParse(entrada, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out valor)
                    && valor >= min)
                    valido = true;
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [!] Ingrese un número válido mayor o igual a {min}.");
                    Console.ResetColor();
                }
            }
            return valor;
        }

        // ── Confirmar Sí/No ──────────────────────────────────
        public static bool Confirmar(string pregunta)
        {
            while (true)
            {
                Console.Write($"  {pregunta} (S/N): ");
                string r = Console.ReadLine()?.Trim().ToUpper() ?? "";
                if (r == "S") return true;
                if (r == "N") return false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  [!] Ingrese S o N.");
                Console.ResetColor();
            }
        }
    }
}
