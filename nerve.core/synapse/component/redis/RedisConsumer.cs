using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nerve.core.synapse.componentbase.impl;
using nerve.core.synapse.dataobjects;
using StackExchange.Redis;

namespace nerve.core.synapse.component.redis
{
    public class RedisConsumer : PollingConsumer
    {
        private readonly RedisProcessor _redisProcessor;

        public override Exchange Poll()
        {
            Task.Factory.StartNew(PollingMessageConsumer);
            return null;
        }

        private void PollingMessageConsumer()
        {
            var configStr = _redisProcessor.UriInformation.GetUriProperty("config", "localhost:6739,allowadmin=true");
            var password = _redisProcessor.UriInformation.GetUriProperty("pwd", "");
            var config = ConnectionMultiplexer.Connect(configStr);

            config.ConfigurationChanged += config_ConfigurationChanged;
            config.ErrorMessage += config_ErrorMessage;
            var db = config.GetDatabase();
        }

        void config_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        void config_ConfigurationChanged(object sender, EndPointEventArgs e)
        {
            
        }

        public RedisConsumer(RedisProcessor redisProcessor)
        {
            _redisProcessor = redisProcessor;
        }
    }
}
