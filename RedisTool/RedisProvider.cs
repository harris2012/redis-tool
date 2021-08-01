using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTool
{
    static class RedisProvider
    {
        static readonly object lockObject = new object();

        static readonly ConcurrentDictionary<string, ConnectionMultiplexer> connectionMultiplexerMap = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        public static ConnectionMultiplexer GetConnectionMultiplexer(string providerName)
        {
            ConnectionMultiplexer connectionMultiplexer = null;

            if (connectionMultiplexerMap.TryGetValue(providerName, out connectionMultiplexer))
            {
                return connectionMultiplexer;
            }

            lock (lockObject)
            {
                if (connectionMultiplexerMap.TryGetValue(providerName, out connectionMultiplexer))
                {
                    return connectionMultiplexer;
                }

                connectionMultiplexer = ConnectionMultiplexer.Connect($"{providerName}:6379");
                if (connectionMultiplexer != null)
                {
                    connectionMultiplexerMap.TryAdd(providerName, connectionMultiplexer);
                }

                return connectionMultiplexer;
            }
        }
    }
}
