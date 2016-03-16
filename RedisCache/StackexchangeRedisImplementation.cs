﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisCache;
using StackExchange.Redis;

namespace RedisCache
{
    internal class StackexchangeRedisImplementation :IRedisImplementation
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public StackexchangeRedisImplementation(IRedisCacheSettings settings)
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect(settings.ServerAddress);
        }

        public IDatabase Database => _connectionMultiplexer.GetDatabase();
        public void StringSet(string primaryKey, string value)
        {
            Database.StringSet(primaryKey, value);
        }

        public void Clear()
        {
            var endpoints = _connectionMultiplexer.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = _connectionMultiplexer.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }

        public void StringSet(KeyValuePair<RedisKey, RedisValue>[] keyValueArray)
        {
            Database.StringSet(keyValueArray);
        }

        public string StringGet(string primaryKey)
        {
            return Database.StringGet(primaryKey);
        }

        public RedisValue[] StringGet(RedisKey[] primaryKeys)
        {
            return Database.StringGet(primaryKeys);
        }

        public void KeyDelete(string primaryKey)
        {
            Database.KeyDelete(primaryKey);
        }

        public void KeyDelete(RedisKey[] primaryKeys)
        {
            Database.KeyDelete(primaryKeys);
        }
    }
}
