using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;


namespace Ejercicio2
{
    class Server
    {
        static List<Cliente> clientes = new List<Cliente>(); // Creo una colección de clientes para ir guardando en ella los clientes que se conectan


        // Función que contiene las acciones que realiza el cliente cuando se conecta al servidor
        static void funcionCliente(object cl)
        {
            Cliente cliente = (Cliente)cl;
            string msg; // Creo variable para el mensaje que manda el cliente
            string nombre;


            using (NetworkStream ns = new NetworkStream(cliente.SClient))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                sw.WriteLine("Identifiquese!");
                sw.Flush();

                cliente.Nombre = sr.ReadLine();

                nombre = cliente.Nombre;

                foreach (Cliente cli in clientes)
                {
                    using (NetworkStream ns2 = new NetworkStream(cli.SClient))
                    using (StreamReader sr2 = new StreamReader(ns2))
                    using (StreamWriter sw2 = new StreamWriter(ns2))
                    {
                        sw2.WriteLine("\n" + nombre + " se ha conectado\n");
                        sw2.Flush();
                    }
                }

                while (true)
                {
                    try
                    {
                        msg = sr.ReadLine(); // Mensaje que escribe el cliente

                        //El mensaje es null en el Shutdown
                        if (msg != null)
                        {
                            if (msg == "#salir")
                            {
                                break;
                            }
                            else if(msg == "#lista")
                            {
                                sw.WriteLine("Lista de usuarios conectados : \n");

                                foreach (Cliente cli in clientes)
                                {
                                    if (cli != cliente)
                                    {
                                        sw.WriteLine("{0}@{1}", cli.IeCliente.Address, cli.Nombre);
                                    }
                                }

                                sw.Flush();
                            }
                            else
                            {

                                foreach (Cliente cli in clientes)
                                {
                                    if (cli != cliente)
                                    {
                                        using (NetworkStream ns2 = new NetworkStream(cli.SClient))
                                        using (StreamReader sr2 = new StreamReader(ns2))
                                        using (StreamWriter sw2 = new StreamWriter(ns2))
                                        {
                                            sw2.WriteLine("{0}@{1} : {2}", cliente.IeCliente.Address, cliente.Nombre, msg);
                                            sw2.Flush();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (IOException)
                    {
                        //Salta al acceder al socket y no estar permitido
                        break;
                    }
                }

                foreach (Cliente cli in clientes)
                {
                    if (cli != cliente)
                    {
                        using (NetworkStream ns2 = new NetworkStream(cli.SClient))
                        using (StreamReader sr2 = new StreamReader(ns2))
                        using (StreamWriter sw2 = new StreamWriter(ns2))
                        {
                            sw2.WriteLine("{0} se ha desconectado", cliente.Nombre); // Mensaje de despedida
                            sw2.Flush();
                        }
                    }
                }

            }

            cliente.SClient.Close();

            clientes.Remove(cliente);
        }


        static void Main(string[] args)
        {
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, 31416); // Creo y defino el IPEndPoint del server
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Creo y defino el socket


            try
            {
                s.Bind(ie); // Enlazo el socket al IPEndPoint
            }
            catch (SocketException e) when (e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
            {
                // Si está ocupado, lo cambio a otro secundario
                ie.Port = 31415;
                try
                {
                    s.Bind(ie);
                    Console.WriteLine("Servidor lanzado en el puerto " + ie.Port);
                }
                catch (SocketException e1) when (e1.ErrorCode == (int)SocketError.AddressAlreadyInUse)
                {
                    // Si el secundario también está ocupado, cierro el server
                    Console.WriteLine("Puertos ocupados, no se puede lanzar el servidor");
                    s.Close();
                    return;
                }
            }

            s.Listen(30); // Se queda esperando una conexión y se establece la cola a 30

            while (true)
            {
                Socket sClient = s.Accept(); // Aceptamos la conexión del cliente

                // Después de aceptar la conexión, añadimos a la colección un nuevo cliente pasándole su Socket como parámetro para así inicializarlo
                clientes.Add(new Cliente(sClient));

                // Uso la funcionCliente para el hiloCliente y lo lanzo pasándole el último cliente añadido a la colección como parámetro
                Thread hiloCliente = new Thread(funcionCliente);
                hiloCliente.Start(clientes[clientes.Count-1]);
            }
        }
    }
}
