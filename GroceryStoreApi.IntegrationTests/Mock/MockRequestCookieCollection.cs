using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;

namespace GroceryStoreApi.IntegrationTests.Mock
{
    public class MockRequestCookieCollection : IRequestCookieCollection
    {
        private readonly Dictionary<string, string> cookies;

        public MockRequestCookieCollection()
        {
            cookies = new Dictionary<string, string>();
        }

        public string this[string key] => cookies[key];

        public int Count => cookies.Count;

        public ICollection<string> Keys => cookies.Keys;

        public bool ContainsKey(string key)
        {
            return cookies.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return cookies.GetEnumerator();
        }

        public bool TryGetValue(string key, out string value)
        {
            return cookies.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return cookies.GetEnumerator();
        }
    }
}
