﻿using System.Net.Http;

namespace Amockibia.Extensions
{
    public class When
    {
        public static RequestRuleBuilder Receive(HttpMethod httpMethod, string uri)
        {
            return new RequestRuleBuilder(httpMethod, uri);
        }

        public static RequestRuleBuilder Get(string uri)
        {
            return Receive(HttpMethod.Get, uri);
        }
        
        public static RequestRuleBuilder Post(string uri)
        {
            return Receive(HttpMethod.Post, uri);
        }
        
        public static RequestRuleBuilder Put(string uri)
        {
            return Receive(HttpMethod.Put, uri);
        }
        
        public static RequestRuleBuilder Delete(string uri)
        {
            return Receive(HttpMethod.Delete, uri);
        }
    }
}
