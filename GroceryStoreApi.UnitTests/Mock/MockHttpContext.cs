using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace GroceryStoreApi.UnitTests.Mock
{
    /// 
    /// </summary>
    public class MockHttpContext : HttpContext, IDisposable
    {
        private readonly MockHttpRequest httpRequest;
        private readonly MockHttpResponse httpResponse;
        private readonly ConnectionInfo connectionInfo;
        private readonly FeatureCollection features;

        public MockHttpContext(IServiceProvider serviceProvider)
        {
            RequestAborted = new CancellationToken();
            TraceIdentifier = $"UT-{Guid.NewGuid()}";
            RequestServices = serviceProvider;
            Items = new Dictionary<object, object>();

            this.features = new FeatureCollection();
            this.httpRequest = new MockHttpRequest(this)
            {
                Host = new HostString("localhost", 1),
                IsHttps = true,
                Method = "POST",
                Protocol = "HTTPS",
                Path = "/payments",
                PathBase = new PathString("/payments"),
                QueryString = new QueryString(),
                Scheme = "HTTP/1.1",
                Form = new FormCollection(new Dictionary<string, StringValues>()),
                Cookies = new MockRequestCookieCollection(),
                Query = new QueryCollection()
            };

            this.connectionInfo = BuildConnectionInfo();
            this.httpResponse = new MockHttpResponse(this);
        }

        public override CancellationToken RequestAborted { get; set; }

        public override HttpRequest Request => httpRequest;

        public override HttpResponse Response => httpResponse;

        public override IServiceProvider RequestServices { get; set; }

        public override string TraceIdentifier { get; set; }

        public override ClaimsPrincipal User { get; set; }

        public override IDictionary<object, object> Items { get; set; }

        public override ConnectionInfo Connection => connectionInfo;

        public override IFeatureCollection Features => features;

        public override ISession Session { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override WebSocketManager WebSockets => throw new NotImplementedException();

        public override void Abort()
        {
            RequestAborted = new CancellationToken(true);
        }

        private ConnectionInfo BuildConnectionInfo()
        {
            return new MockConnectionInfo()
            {
                LocalIpAddress = new IPAddress(0),
                RemoteIpAddress = new IPAddress(0),
                Id = Guid.NewGuid().ToString(),
                LocalPort = 0,
                RemotePort = 0
            };
        }

        public void Dispose()
        {
            this.httpRequest.Dispose();
            this.httpResponse.Dispose();
        }
    }
}
