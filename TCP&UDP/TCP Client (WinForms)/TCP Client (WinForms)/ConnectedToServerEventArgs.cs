﻿using System.Net.Sockets;

namespace TCP_Client__WinForms_
{
    public class ConnectedToServerEventArgs
    {
        public TcpListener Server { set; get; }

        public ConnectedToServerEventArgs()
        {
            
        }
    }

    public delegate void ConnectedToServerEventHandler(object sender, ConnectedToServerEventArgs e);
}