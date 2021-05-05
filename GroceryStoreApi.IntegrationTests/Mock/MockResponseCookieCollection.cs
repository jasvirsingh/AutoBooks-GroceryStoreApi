namespace GroceryStoreApi.IntegrationTests.Mock
{
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Net;

    public class MockResponseCookieCollection : IResponseCookies
    {
        private readonly Dictionary<string, Cookie> cookies;

        public MockResponseCookieCollection()
        {
            cookies = new Dictionary<string, Cookie>();
        }

        public void Append(string key, string value)
        {
            cookies.Add(key, new Cookie(key, value));
        }

        public void Append(string key, string value, CookieOptions options)
        {
            cookies.Add(key, new Cookie(key, value, options.Path, options.Domain));
        }

        public void Delete(string key)
        {
            if (cookies.ContainsKey(key))
            {
                cookies.Remove(key);
            }
        }

        public void Delete(string key, CookieOptions options)
        {
            if (cookies.ContainsKey(key))
            {
                cookies.Remove(key);
            }
        }
    }
}
