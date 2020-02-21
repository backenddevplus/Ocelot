namespace Ocelot.AcceptanceTests
{
    using Microsoft.AspNetCore.Http;
    using Ocelot.Configuration.File;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using TestStack.BDDfy;
    using Xunit;

    public class HttpTests : IDisposable
    {
        private readonly Steps _steps;
        private readonly ServiceHandler _serviceHandler;

        public HttpTests()
        {
            _serviceHandler = new ServiceHandler();
            _steps = new Steps();
        }

        [Fact]
        public void should_return_response_200_when_using_http_one()
        {
            const int port = 53219;

            var configuration = new FileConfiguration
            {
                ReRoutes = new List<FileReRoute>
                {
                    new FileReRoute
                    {
                        DownstreamPathTemplate = "/{url}",
                        DownstreamScheme = "https",
                        UpstreamPathTemplate = "/{url}",
                        UpstreamHttpMethod = new List<string> { "Get" },
                        DownstreamHostAndPorts = new List<FileHostAndPort>
                        {
                            new FileHostAndPort
                            {
                                Host = "localhost",
                                Port = port,
                            },
                        },
                        DownstreamHttpMethod = "POST",
                        DownstreamHttpVersion = "1.0",
                        DangerousAcceptAnyServerCertificateValidator = true
                    },
                },
            };

            this.Given(x => x.GivenThereIsAServiceRunningOn($"http://localhost:{port}/", "/", port, HttpProtocols.Http1))
                .And(x => _steps.GivenThereIsAConfiguration(configuration))
                .And(x => _steps.GivenOcelotIsRunning())
                .When(x => _steps.WhenIGetUrlOnTheApiGateway("/"))
                .Then(x => _steps.ThenTheStatusCodeShouldBe(HttpStatusCode.OK))
                .BDDfy();
        }

        [Fact]
        public void should_return_response_200_when_using_http_one_point_one()
        {
            const int port = 53279;

            var configuration = new FileConfiguration
            {
                ReRoutes = new List<FileReRoute>
                    {
                        new FileReRoute
                        {
                            DownstreamPathTemplate = "/{url}",
                            DownstreamScheme = "https",
                            UpstreamPathTemplate = "/{url}",
                            UpstreamHttpMethod = new List<string> { "Get" },
                            DownstreamHostAndPorts = new List<FileHostAndPort>
                            {
                                new FileHostAndPort
                                {
                                    Host = "localhost",
                                    Port = port,
                                },
                            },
                            DownstreamHttpMethod = "POST",
                            DownstreamHttpVersion = "1.1",
                            DangerousAcceptAnyServerCertificateValidator = true
                        },
                    },
            };

            this.Given(x => x.GivenThereIsAServiceRunningOn($"http://localhost:{port}/", "/", port, HttpProtocols.Http1))
                .And(x => _steps.GivenThereIsAConfiguration(configuration))
                .And(x => _steps.GivenOcelotIsRunning())
                .When(x => _steps.WhenIGetUrlOnTheApiGateway("/"))
                .Then(x => _steps.ThenTheStatusCodeShouldBe(HttpStatusCode.OK))
                .BDDfy();
        }

        [Fact]
        public void should_return_response_200_when_using_http_two_point_zero()
        {
            const int port = 53675;

            var configuration = new FileConfiguration
            {
                ReRoutes = new List<FileReRoute>
                {
                    new FileReRoute
                    {
                        DownstreamPathTemplate = "/{url}",
                        DownstreamScheme = "https",
                        UpstreamPathTemplate = "/{url}",
                        UpstreamHttpMethod = new List<string> { "Get" },
                        DownstreamHostAndPorts = new List<FileHostAndPort>
                        {
                            new FileHostAndPort
                            {
                                Host = "localhost",
                                Port = port,
                            },
                        },
                        DownstreamHttpMethod = "POST",
                        DownstreamHttpVersion = "2.0",
                        DangerousAcceptAnyServerCertificateValidator = true
                    },
                },
            };

            const string expected = "here is some content";
            var httpContent = new StringContent(expected);

            this.Given(x => x.GivenThereIsAServiceRunningOn($"http://localhost:{port}/", "/", port, HttpProtocols.Http2))
                .And(x => _steps.GivenThereIsAConfiguration(configuration))
                .And(x => _steps.GivenOcelotIsRunning())
                .When(x => _steps.WhenIGetUrlOnTheApiGateway("/", httpContent))
                .Then(x => _steps.ThenTheStatusCodeShouldBe(HttpStatusCode.OK))
                .And(_ => _steps.ThenTheResponseBodyShouldBe(expected))
                .BDDfy();
        }

        [Fact]
        public void should_return_response_500_when_using_http_one_to_talk_to_server_running_http_two()
        {
            const int port = 53677;

            var configuration = new FileConfiguration
            {
                ReRoutes = new List<FileReRoute>
                {
                    new FileReRoute
                    {
                        DownstreamPathTemplate = "/{url}",
                        DownstreamScheme = "https",
                        UpstreamPathTemplate = "/{url}",
                        UpstreamHttpMethod = new List<string> { "Get" },
                        DownstreamHostAndPorts = new List<FileHostAndPort>
                        {
                            new FileHostAndPort
                            {
                                Host = "localhost",
                                Port = port,
                            },
                        },
                        DownstreamHttpMethod = "POST",
                        DownstreamHttpVersion = "1.1",
                        DangerousAcceptAnyServerCertificateValidator = true
                    },
                },
            };

            const string expected = "here is some content";
            var httpContent = new StringContent(expected);

            this.Given(x => x.GivenThereIsAServiceRunningOn($"http://localhost:{port}/", "/", port, HttpProtocols.Http2))
                .And(x => _steps.GivenThereIsAConfiguration(configuration))
                .And(x => _steps.GivenOcelotIsRunning())
                .When(x => _steps.WhenIGetUrlOnTheApiGateway("/", httpContent))
                .Then(x => _steps.ThenTheStatusCodeShouldBe(HttpStatusCode.InternalServerError))
                .BDDfy();
        }

        [Fact]
        public void should_return_response_200_when_using_http_two_to_talk_to_server_running_http_one_point_one()
        {
            const int port = 53679;

            var configuration = new FileConfiguration
            {
                ReRoutes = new List<FileReRoute>
                {
                    new FileReRoute
                    {
                        DownstreamPathTemplate = "/{url}",
                        DownstreamScheme = "https",
                        UpstreamPathTemplate = "/{url}",
                        UpstreamHttpMethod = new List<string> { "Get" },
                        DownstreamHostAndPorts = new List<FileHostAndPort>
                        {
                            new FileHostAndPort
                            {
                                Host = "localhost",
                                Port = port,
                            },
                        },
                        DownstreamHttpMethod = "POST",
                        DownstreamHttpVersion = "2.0",
                        DangerousAcceptAnyServerCertificateValidator = true
                    },
                },
            };

            const string expected = "here is some content";
            var httpContent = new StringContent(expected);

            this.Given(x => x.GivenThereIsAServiceRunningOn($"http://localhost:{port}/", "/", port, HttpProtocols.Http1))
                .And(x => _steps.GivenThereIsAConfiguration(configuration))
                .And(x => _steps.GivenOcelotIsRunning())
                .When(x => _steps.WhenIGetUrlOnTheApiGateway("/", httpContent))
                .Then(x => _steps.ThenTheStatusCodeShouldBe(HttpStatusCode.OK))
                .And(_ => _steps.ThenTheResponseBodyShouldBe(expected))
                .BDDfy();
        }

        private void GivenThereIsAServiceRunningOn(string baseUrl, string basePath, int port, HttpProtocols protocols)
        {
            _serviceHandler.GivenThereIsAServiceRunningOn(baseUrl, basePath, async context =>
            {
                context.Response.StatusCode = 200;
                var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                await context.Response.WriteAsync(body);
            }, port, protocols);
        }

        public void Dispose()
        {
            _serviceHandler.Dispose();
            _steps.Dispose();
        }
    }
}
