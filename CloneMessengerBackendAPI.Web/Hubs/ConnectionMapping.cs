using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicChat
{
    public class ConnectionMapping<T>
    {
    /// <summary>
    /// Key : UserId
    /// Data : List connectionIds do Hub cap case login nhieu cho
    /// </summary>
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }
        public IEnumerable<string> GetConnectionsByKeys(List<T> keys)
        {
            List<string> connections = _connections.Where(i => keys.Contains(i.Key)).SelectMany(i=> i.Value).ToList();

            return connections;
        }
        public IEnumerable<Guid> GetUserOnlines(List<T> userIds)
        {
            List<Guid> userConnectingIds = _connections.Where(i => userIds.Contains(i.Key)).Select(i => Guid.Parse(i.Key.ToString())).ToList();

            return userConnectingIds;
        }
        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}