// ============================================================
//  ModuloMesas.cs - Asignación y liberación de mesas
// ============================================================

using System;
using SIAPED.Data;
using SIAPED.Utils;

namespace SIAPED.Modules
{
    static class ModuloMesas
    {
        // ── Mostrar todas las mesas con estado ───────────────
        public static void MostrarMesas()
        {
            Consola.MostrarEncabezado("ESTADO DE MESAS");
            Mesa[] mesas = Archivos.LeerMesas();

            Console.WriteLine($"  {"N°",-5} {"Capacidad",-12} {"Estado",-10}");
            Consola.MostrarSeparador();

            foreach (Mesa m in mesas)
            {
                ConsoleColor color = m.Estado == "LIBRE" ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($"  Mesa {m.Numero,-3} {m.Capacidad + " personas",-12} ");
                Console.ForegroundColor = color;
                Console.WriteLine(m.Estado);
                Console.ResetColor();
            }
            Consola.Pausar();
        }

        // ── Asignar una mesa libre al cliente ────────────────
        // Retorna el número de mesa asignada, o -1 si no hay disponibles
        public static int AsignarMesa(int personas)
        {
            Consola.MostrarEncabezado("ASIGNACIÓN DE MESA");
            Mesa[] mesas = Archivos.LeerMesas();

            // Mostrar solo mesas libres con capacidad suficiente
            bool hayDisponibles = false;
            Console.WriteLine($"  Personas en el grupo: {personas}");
            Console.WriteLine($"  Mesas disponibles:\n");
            Console.WriteLine($"  {"N°",-8} {"Capacidad",-12}");
            Consola.MostrarSeparador();

            foreach (Mesa m in mesas)
            {
                if (m.Estado == "LIBRE" && m.Capacidad >= personas)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  Mesa {m.Numero,-5} {m.Capacidad} personas");
                    Console.ResetColor();
                    hayDisponibles = true;
                }
            }

            if (!hayDisponibles)
            {
                Consola.MostrarError($"No hay mesas libres para {personas} personas.");
                return -1;
            }

            Console.WriteLine();
            int numMesa = Consola.LeerEntero("Número de mesa a asignar", 1, mesas.Length);

            // Validar que la mesa elegida esté libre y tenga capacidad
            foreach (Mesa m in mesas)
            {
                if (m.Numero == numMesa)
                {
                    if (m.Estado != "LIBRE")
                    {
                        Consola.MostrarError($"La Mesa {numMesa} está OCUPADA.");
                        return -1;
                    }
                    if (m.Capacidad < personas)
                    {
                        Consola.MostrarError($"La Mesa {numMesa} solo tiene capacidad para {m.Capacidad} personas.");
                        return -1;
                    }
                    break;
                }
            }

            // Marcar mesa como OCUPADA
            CambiarEstadoMesa(numMesa, "OCUPADA");
            Consola.MostrarExito($"Mesa {numMesa} asignada correctamente.");
            return numMesa;
        }

        // ── Liberar una mesa al finalizar la atención ────────
        public static void LiberarMesa(int numMesa)
        {
            CambiarEstadoMesa(numMesa, "LIBRE");
            Consola.MostrarExito($"Mesa {numMesa} liberada. Lista para el siguiente cliente.");
        }

        // ── Cambiar el estado de una mesa en el archivo ──────
        private static void CambiarEstadoMesa(int numMesa, string nuevoEstado)
        {
            Mesa[] mesas = Archivos.LeerMesas();
            for (int i = 0; i < mesas.Length; i++)
            {
                if (mesas[i].Numero == numMesa)
                {
                    mesas[i].Estado = nuevoEstado;
                    break;
                }
            }
            Archivos.GuardarMesas(mesas);
        }
    }
}
