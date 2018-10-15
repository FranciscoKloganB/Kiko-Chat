using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.ServiceModel.Channels;

namespace kiko_chat_server_console.src
{
    public class AsyncUserToken
    {
        public Socket Socket { get; set; }
    }

    /*
    * Implements the connection logic for the socket server.  
    * After accepting a connection, all data read from the client is sent back to the client.
    * The read and echo back to the client pattern is continued until the client disconnects.
    */
    class Server
    {
        /*
         * opsToPreAlloc >> read, write (don't alloc buffer space for accepts)
         * m_numConnections >> the maximum number of connections the sample is designed to handle simultaneously 
         * m_receiveBufferSize >> buffer size to use for each socket I/O operation 
         * m_totalBytesRead >> counter of the total # bytes received by the server
         * m_numConnectedSockets >> the total number of clients connected to the server
         * m_bufferManager >> represents a large reusable set of buffers for all socket operations
         * listenSocket >> the socket used to listen for incoming connection requests
         * m_readWritePool >> pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations
         */
        private const int opsToPreAlloc = 2;
        private int m_numConnections;  
        private int m_receiveBufferSize;
        private int m_totalBytesRead;
        private int m_numConnectedSockets;
        private BufferManager m_bufferManager;
        private Semaphore m_maxNumberAcceptedClients;
        private Socket listenSocket;
        private SocketAsyncEventArgsPool m_readWritePool;

        /*
        * Create an uninitialized server instance. To start the server listening for connection requests call the Init method followed by Start method  
        */
        public Server(int numConnections = 0, int receiveBufferSize = 0)
        {
            m_totalBytesRead = 0;
            m_numConnectedSockets = 0;
            m_numConnections = numConnections;
            m_receiveBufferSize = receiveBufferSize;
            // allocate buffers such that the maximum number of sockets can have one outstanding read and write posted to the socket simultaneously  
            m_bufferManager = new BufferManager(totalBytes: receiveBufferSize * numConnections * opsToPreAlloc, bufferSize: receiveBufferSize);
            m_readWritePool = new SocketAsyncEventArgsPool(numConnections);
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
        }

        /*
         * Initializes the server by preallocating reusable buffers and context objects.
         * These objects do not need to be preallocated or reused, but it is done this way increase server performance by avoiding multiple instanciations.
         */
        public void Init()
        {
            // InitBuffer allocates one large byte buffer which all I/O operations use a piece of.  This guards against memory fragmentation
            m_bufferManager.InitBuffer();
            // Preallocate pool of SocketAsyncEventArgs objects
            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0 ; i < m_numConnections; i++)
            {
                // Pre-allocate a set of reusable SocketAsyncEventArgs
                readWriteEventArg = new SocketAsyncEventArgs();
                // EventHandler defines that Events of type SocketAsyncEventArgs will be handled by method IO_Completed
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.UserToken = new AsyncUserToken();
                // Assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                m_bufferManager.SetBuffer(readWriteEventArg);
                // Add SocketAsyncEventArg to the pool
                m_readWritePool.Push(readWriteEventArg);
            }
        }

        /*
         * Starts the server such that it is listening for incoming connection requests.    
         * <param name="localEndPoint">The endpoint which the server will listening for connection requests on</param>
         */
        public void Start(IPEndPoint localEndPoint)
        {
            // Create the socket which listens for incoming connections
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
            // Set the server with a listen backlog of 100 connections
            listenSocket.Listen(100);
            // Post accepts on the listening socket
            StartAccept(null);
            // Console.WriteLine("{0} connected sockets with one outstanding receive posted to each....press any key", m_outstandingReadCount);
            Console.WriteLine("Press any key to terminate the server process....");
            Console.ReadKey();
        }


        /*
         * Begins an operation to accept a connection request from the client.
         * <param name="acceptEventArg">The context object to use when issuing the accept operation on the server's listening socket</param>
         */
        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            bool willRaiseEvent;

            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // socket must be cleared since the context object is being reused
                acceptEventArg.AcceptSocket = null;
            }

            m_maxNumberAcceptedClients.WaitOne();

            willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /* 
         * This method is the callback method associated with Socket.AcceptAsync operations and is invoked when an accept operation is complete
         */
        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent;

            Interlocked.Increment(ref m_numConnectedSockets);
            Console.WriteLine("Client connection accepted. There are {0} clients connected to the server", m_numConnectedSockets);
            // Get the socket for the accepted client connection and put it into the ReadEventArg object user token
            SocketAsyncEventArgs readEventArgs = m_readWritePool.Pop();
            ((AsyncUserToken)readEventArgs.UserToken).Socket = e.AcceptSocket;

            // As soon as the client is connected, post a receive to the connection
            willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
            if (!willRaiseEvent)
            {
                ProcessReceive(readEventArgs);
            }

            // Accept the next connection request
            StartAccept(e);
        }

        /*
         * This method is called whenever a receive or send operation is completed on a socket 
         * <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
         */
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        /*
         * This method is invoked when an asynchronous receive operation completes. 
         * If the remote host closed the connection, then the socket is closed. If data was received then the data is echoed back to the client.
         */
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent;
            // check if the remote host closed the connection
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                // increment the count of the total bytes receive by the server
                Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);
                Console.WriteLine("The server has read a total of {0} bytes", m_totalBytesRead);
                // echo the data received back to the client
                e.SetBuffer(e.Offset, e.BytesTransferred);
                willRaiseEvent = token.Socket.SendAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessSend(e);
                }

            }
            else
            {
                CloseClientSocket(e);
            }
        }

        /*
         * This method is invoked when an asynchronous send operation completes.  
         * The method issues another receive on the socket to read any additional data sent from the client.
         * <param name="e"></param>
         */
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent;

            if (e.SocketError == SocketError.Success)
            {
                // done echoing data back to the client
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                // read the next block of data send from the client
                willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            // close the socket associated with the client
            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception) { }
            token.Socket.Close();

            // decrement the counter keeping track of the total number of clients connected to the server
            Interlocked.Decrement(ref m_numConnectedSockets);

            // Free the SocketAsyncEventArg so they can be reused by another client
            m_readWritePool.Push(e);

            m_maxNumberAcceptedClients.Release();
            Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", m_numConnectedSockets);
        }

    }
}
