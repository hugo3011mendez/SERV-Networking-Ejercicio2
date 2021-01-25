using System;
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
        static void funcionCliente(object cliente)
        {
            string msg; // Creo variable para el mensaje que manda el cliente
            string nombre;

            Socket sClient = (Socket)cliente;
            IPEndPoint ieCliente = (IPEndPoint)sClient.RemoteEndPoint;

            using (NetworkStream ns = new NetworkStream(sClient))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                sw.WriteLine("Identifiquese!");
                sw.Flush();

                nombre = sr.ReadLine();

                sw.WriteLine("\n" + nombre + " se ha conectado\n");
                sw.Flush();

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

                            }
                            else
                            {
                                sw.WriteLine("{0}@{1} : {2}", ieCliente.Address, nombre, msg);
                                sw.Flush();
                            }
                        }
                    }
                    catch (IOException)
                    {
                        //Salta al acceder al socket y no estar permitido
                        break;
                    }
                }

                sw.WriteLine("{0} se ha desconectado", nombre); // Mensaje de despedida
                sw.Flush();
            }

            sClient.Close();
        }


        static void Main(string[] args)
        {
            IPEndPoint ie = new IPEndPoint(IPAddress.Loopback, 31416); // Creo y defino el IPEndPoint del server
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Creo y defino el socket
            s.Bind(ie); // Enlazo el socket al IPEndPoint
            s.Listen(30); // Se queda esperando una conexión y se establece la cola a 30

            while (true)
            {
                Socket sClient = s.Accept(); // Aceptamos la conexión del cliente
                Thread hiloCliente = new Thread(funcionCliente);
                hiloCliente.Start(sClient);
            }
        }
    }
}
