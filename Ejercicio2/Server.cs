using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
// Nuevos imports para trabajar con Networking
using System.Net;
using System.Net.Sockets;


namespace Ejercicio2
{
    class Server
    {
        static void Main(string[] args)
        {
            IPEndPoint ie = new IPEndPoint(IPAddress.Loopback, 31416); // Creo y defino el IPEndPoint del server
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Creo y defino el socket
            s.Bind(ie); // Enlazo el socket al IPEndPoint
            s.Listen(30); // Se queda esperando una conexión y se establece la cola a 30

            bool conexion = true;
            while (conexion)
            {
                Socket sClient = s.Accept(); // Aceptamos la conexión del cliente
                // Obtenemos info del cliente :
                IPEndPoint ieClient = (IPEndPoint)sClient.RemoteEndPoint; // Casteo a IPEndPoint porque EndPoint es más genérico

                // Conexión :
                using (NetworkStream ns = new NetworkStream(sClient)) // Se crea un Stream que hará de puente entre el Socket, el StreamReader y el StreamWriter
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    sw.WriteLine("Identifiquese!");
                    string nombre = sr.ReadLine();

                    sw.WriteLine("\n" + nombre + " se ha conectado\n");

                    string msg = ""; // Creo y defino variable para el mensaje que manda el cliente
                    while (msg != null) // Mientras el mensaje escrito por el cliente no sea null...
                    {
                        try
                        {
                            msg = sr.ReadLine();

                            if (msg != null)
                            {
                                Console.WriteLine(nombre + "@" + ieClient.Address + " : " + msg); // Se muestra la info en el server
                                sw.WriteLine(msg); // El server reenvía el mismo mensaje al cliente
                                sw.Flush();
                            }
                        }
                        catch (IOException e) // Si ocurre algún error, se sale del bucle
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
