using System.IO;
using PactNet;
using Xunit.Abstractions;
using Xunit;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Consumer;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService;
using System;
using PactNet.Mocks.MockHttpService.Models;
using System.Threading.Tasks;

namespace tests;

public class ApiTest : IClassFixture<ConsumerPactClassFixture>
{
    private readonly ApiClient _apiClient;
    private readonly int _port;
    private readonly IMockProviderService _mockProviderService;
    private static List<object> products = new List<object>()
    {
        new { id = 9, type = "CREDIT_CARD", name = "GEM Visa", version = "v2" },
        new { id = 10, type = "CREDIT_CARD", name = "28 Degrees", version = "v1" }
    };

    public ApiTest(ConsumerPactClassFixture fixture)
    {
        _port = fixture.MockServerPort;
        _apiClient = new ApiClient(new Uri(fixture.MockProviderServiceBaseUri));

        _mockProviderService = fixture.MockProviderService;
        //NOTE: Clears any previously registered interactions before the test is run
        _mockProviderService.ClearInteractions();
    }

    [Fact]
    public async Task GetAllProducts()
    {
        // Given
        _mockProviderService
            .Given("products exist")
            .UponReceiving("A valid request for all products")
            .With(
                new PactNet.Mocks.MockHttpService.Models.ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/api/products",
                }
            )
            // .WithRequest(HttpMethod.Get, "/api/products")
            .WillRespondWith(
                new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, object>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = products
                }
            );
        // .WillRespond()
        // .WithStatus(HttpStatusCode.OK)
        // .WithHeader("Content-Type", "application/json; charset=utf-8")
        // .WithJsonBody(new TypeMatcher(products));

        // When
        var result = await _apiClient.GetAllProducts();

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        // await pact.VerifyAsync(
        //     async ctx =>
        //     {
        //         var response = await _apiClient.GetAllProducts();
        //         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //     }
        // );
    }

    [Fact]
    public async void GetProduct()
    {
        // Given
        _mockProviderService
            .Given("product with ID 10 exists")
            .UponReceiving("A valid request for a product")
            .With(
                new PactNet.Mocks.MockHttpService.Models.ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/api/product/10",
                }
            )
            // .WithRequest(HttpMethod.Get, "/api/products")
            .WillRespondWith(
                new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, object>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = products[1]
                }
            );
        // pact.UponReceiving("A valid request for a product")
        //     .Given("product with ID 10 exists")
        //     .WithRequest(HttpMethod.Get, "/api/products/10")
        //     .WillRespond()
        //     .WithStatus(HttpStatusCode.OK)
        //     .WithHeader("Content-Type", "application/json; charset=utf-8")
        //     .WithJsonBody(new TypeMatcher(products[1]));

        // When
        var result = await _apiClient.GetProduct(10);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        // await pact.VerifyAsync(
        //     async ctx =>
        //     {
        //         var response = await _apiClient.GetProduct(10);
        //         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //     }
        // );
    }
}
