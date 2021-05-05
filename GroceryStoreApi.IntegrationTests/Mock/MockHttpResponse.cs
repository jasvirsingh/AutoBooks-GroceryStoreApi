using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStoreApi.IntegrationTests.Mock
{
    /// <summary>
    /// 
    /// </summary>
    public class MockHttpResponse : HttpResponse, IDisposable
    {
        private readonly HeaderDictionary headers;
        private readonly HttpContext httpContext;
        private IResponseCookies cookies;
        private IFormCollection form;
        private Stream stream;

        public MockHttpResponse(HttpContext context)
        {
            this.headers = new HeaderDictionary();
            this.cookies = new MockResponseCookieCollection();
            this.form = new FormCollection(new Dictionary<string, StringValues>());
            this.stream = new MemoryStream();
            this.httpContext = context;
        }

        public override IHeaderDictionary Headers => this.headers;

        public override IResponseCookies Cookies => this.cookies;

        public override Stream Body { get => this.stream; set => this.stream = value; }

        public override long? ContentLength { get; set; }

        public override string ContentType { get; set; }

        public override int StatusCode { get; set; }

        public override bool HasStarted => false;

        public override HttpContext HttpContext => this.httpContext;

        public void Dispose()
        {
            if (this.stream != null)
            {
                this.stream.Dispose();
            }
        }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
        }

        public override void Redirect(string location, bool permanent)
        {
        }
    }
}
