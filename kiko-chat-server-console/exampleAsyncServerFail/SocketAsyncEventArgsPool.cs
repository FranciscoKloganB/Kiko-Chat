using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

// docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socketasynceventargs.-ctor?view=netframework-4.7.2
namespace kiko_chat_server_console.src
{
    // Represents a collection of reusable SocketAsyncEventArgs objects.
    public class SocketAsyncEventArgsPool
    {
        Stack<SocketAsyncEventArgs> m_pool;

        /* 
         * Initializes the object pool to the specified size.
         * capacity parameter >> maximum number of SocketAsyncEventArgs objects the pool can hold 
         */
        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /* Add a SocketAsyncEventArg instance to the pool.
         * item parameter >> SocketAsyncEventArgs instance to add to the pool 
         */
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }

        /* Removes a SocketAsyncEventArgs instance from the pool  and returns the object removed from the pool */
        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        /* The number of SocketAsyncEventArgs instances in the pool */
        public int Count
        {
            get { return m_pool.Count; }
        }

    }
}
