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
    class Cliente
    {
        private Socket sClient;
        public Socket SClient
        {
            set
            {
                sClient = (Socket)value;
            }

            get
            {
                return sClient;
            }
        }


        private IPEndPoint ieCliente;
        public IPEndPoint IeCliente
        {
            set
            {
                IeCliente = (IPEndPoint)SClient.RemoteEndPoint;
            }

            get
            {
                return ieCliente;
            }
        }


        private String nombre;
        public String Nombre
        {
            set
            {
                Nombre = value.Trim();
            }

            get
            {
                return nombre;
            }
        }


        public Cliente(Socket s)
        {
            SClient = s;
            IeCliente = (IPEndPoint)s.RemoteEndPoint;
        }
    }
}
