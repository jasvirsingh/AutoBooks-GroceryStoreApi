namespace GroceryStoreApi.UnitTests.Mock
{
    using Microsoft.AspNetCore.Http;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;

    public class MockConnectionInfo : ConnectionInfo
    {
        private X509Certificate2 certificate;
        private string id;
        private IPAddress localIpAddress;
        private IPAddress remoteIpAddress;
        private int localPort;
        private int remotePort;

        public override X509Certificate2 ClientCertificate { get => certificate; set => certificate = value; }

        public override string Id { get => id; set => id = value; }

        public override IPAddress LocalIpAddress { get => localIpAddress; set => localIpAddress = value; }

        public override int LocalPort { get => localPort; set => localPort = value; }

        public override IPAddress RemoteIpAddress { get => remoteIpAddress; set => remoteIpAddress = value; }

        public override int RemotePort { get => remotePort; set => remotePort = value; }

        public override Task<X509Certificate2> GetClientCertificateAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(certificate);
        }
    }
}
