﻿using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace Saxo.RedisCache
{
    internal class StackexchangeRedisImplementation : IRedisImplementation
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly bool _isServerAlive;

        public StackexchangeRedisImplementation(IRedisCacheSettings settings)
        {
            try
            {
                _connectionMultiplexer = ConnectionMultiplexer.Connect(settings.ServerAddress);
                _isServerAlive = true;
            }
            catch (Exception)
            {
                _isServerAlive = false;
            }
        }

        public IDatabase Database => _connectionMultiplexer.GetDatabase();

        public bool IsAlive()
        {
            return _isServerAlive && _connectionMultiplexer.IsConnected;
        }

        public void StringSet(RedisKey primaryKey, RedisValue value, TimeSpan? expire = null)
        {            
            Database.StringSet(primaryKey, value, expire);
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

        public void StringSet(KeyValuePair<RedisKey, RedisValue>[] keyValueArray, TimeSpan? expire = null)
        {
            Database.StringSet(keyValueArray);
            keyValueArray.ToList().ForEach(keyValue =>
            {
                Database.KeyExpire(keyValue.Key, expire);
            });            
        }

        public RedisValue StringGet(RedisKey primaryKey)
        {
            return Database.StringGet(primaryKey);
        }

        public RedisValue[] StringGet(RedisKey[] primaryKeys)
        {
            return Database.StringGet(primaryKeys);
        }

        public void KeyDelete(RedisKey primaryKey)
        {
            Database.KeyDelete(primaryKey);
        }

        public void KeyDelete(RedisKey[] primaryKeys)
        {
            Database.KeyDelete(primaryKeys);
        }
    }
}
