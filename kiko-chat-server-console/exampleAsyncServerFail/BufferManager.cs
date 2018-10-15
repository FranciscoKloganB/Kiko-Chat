using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

// docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socketasynceventargs.setbuffer?view=netframework-4.7.2#System_Net_Sockets_SocketAsyncEventArgs_SetBuffer_System_Byte___System_Int32_System_Int32

namespace kiko_chat_server_console.src
{
    public class BufferManager
    {
        /*
         *  m_numBytes >> the total number of bytes controlled by the buffer pool
         *  m_buffer >> the underlying byte array maintained by the Buffer Manager
         */
        private int m_bufferSize;
        private int m_currentIndex;
        private int m_numBytes;
        private byte[] m_buffer;
        private Stack<int> m_freeIndexPool;

        /* Create an uninitialized BufferManager instance. To start using you need to allocate buffer space by calling InitBuffer method */
        public BufferManager(int totalBytes = 0, int bufferSize = 0)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        /* Allocates buffer space used by the buffer pool. Create one big large buffer and divide that out to each SocketAsyncEventArg object */
        public void InitBuffer()
        {
            m_buffer = new byte[m_numBytes];
        }

        /* 
         * Assigns a buffer from the buffer pool to the specified SocketAsyncEventArgs object
         * Each SocketAsyncEventArgs has a size of m_bufferSize taken from m_buffer.
         * The current index is updated on each pool.
         * <returns>true if the buffer was successfully set, else false</returns>
         */
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                {
                    return false;
                }
                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                m_currentIndex += m_bufferSize;
            }
            return true;
        }

        /* Removes the buffer from a SocketAsyncEventArg object. This frees the buffer back to the buffer pool */
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

    }
}
