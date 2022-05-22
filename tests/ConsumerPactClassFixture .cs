using System;
using System.Collections.Generic;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace tests;

// This class is responsible for setting up a shared
// mock server for Pact used by all the tests.
// XUnit can use a Class Fixture for this.
// See: https://goo.gl/hSq4nv
public class ConsumerPactClassFixture : IDisposable
{
    private const string PACT_WITH = "product-service";

    public IPactBuilder PactBuilder { get; private set; }
    public IMockProviderService MockProviderService { get; private set; }

    public int MockServerPort
    {
        get { return 9222; }
    }
    public string MockProviderServiceBaseUri
    {
        get { return String.Format("http://localhost:{0}", MockServerPort); }
    }

    public ConsumerPactClassFixture()
    {
        var pactDir = Environment.GetEnvironmentVariable("PACT_DIR") ?? "./pacts";
        var serviceConsumer = Environment.GetEnvironmentVariable("PACTICIPANT") ?? "api-client";
        // Using Spec version 3.0.0 more details at https://goo.gl/UrBSRc
        var pactConfig = new PactConfig
        {
            SpecificationVersion = "3.0.0",
            PactDir = pactDir,
            LogDir = @".\pact_logs"
        };

        PactBuilder = new PactBuilder(pactConfig);

        PactBuilder.ServiceConsumer(serviceConsumer).HasPactWith(PACT_WITH);

        MockProviderService = PactBuilder.MockService(MockServerPort);
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // This will save the pact file once finished.
                PactBuilder.Build();
            }

            disposedValue = true;
        }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
    }
    #endregion
}
