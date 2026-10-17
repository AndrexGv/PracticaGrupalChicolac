using System;

class Program
{
    static void Main()
    {
        int opcion;
        do
        {
            Console.WriteLine();
            Console.WriteLine("===== MENU DEL EQUIPO N =====");
            Console.WriteLine("1. Saludo");
            Console.WriteLine("2. Suma de dos numeros");
            Console.WriteLine("3. Par o impar");
            Console.WriteLine("4. Tabla de multiplicar");
            Console.WriteLine("0. Salir");
            Console.Write("Elige una opcion: ");
            opcion = int.Parse(Console.ReadLine());

            switch (opcion)
            {
                case 1: Saludo(); break;
                case 2: Suma(); break;
                case 3: ParImpar(); break;
                case 4: Tabla(); break;
                case 0: Console.WriteLine("Hasta luego"); break;
                default: Console.WriteLine("Opcion no valida"); break;
            }
        } while (opcion != 0);
    }

    // Opcion 1 (la programa A)
    static void Saludo()
    {
        Console.WriteLine("En construccion");
    }

    // Opcion 2 (la programa B)
    static void Suma()
    {
        Console.WriteLine("En construccion");
    }

    // Opcion 3 (la programa C)
    static void ParImpar()
    {
        Console.WriteLine("En construccion");
    }

    // Opcion 4 (la programa D)
    static void Tabla()
    {
        Console.WriteLine("En construccion");
    }
}