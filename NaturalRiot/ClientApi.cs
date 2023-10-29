using NaturalRiot.NaturalRiot.Endpoints;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

namespace NaturalRiot.NaturalRiot
{
    public class ClientApi
    {
        public static ClientApi instance;
        private HttpClient _baseClient;
        public ClientEndpoints Endpoints { get; }

        public ClientApi()
        {
            string[] data = GetActiveClient();
            _baseClient = BuildClient($"https://127.0.0.1:{data[0]}", "riot", data[1]);
            instance = this;
        }

        public async Task Test()
        {
            Console.WriteLine("HELLO");
            HttpResponseMessage responseMessage = await _baseClient.GetAsync("/lol-summoner/v1/current-summoner");
            Console.WriteLine(responseMessage);
        }


        private static HttpClient BuildClient(string baseUrl, string username, string token)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();

            // Set the ServerCertificateCustomValidationCallback to always return true
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            HttpClient client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri(baseUrl);

            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{token}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            return client;
        }

        private string[] GetActiveClient()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "wmic",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "PROCESS WHERE name='LeagueClientUx.exe' GET commandline"
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                // Process the output to extract port and token using regex
                // Regex pattern for port: --app-port=([0-9]*)
                // Regex pattern for token: --remoting-auth-token=([\w-]*)
                string portPattern = "--app-port=([0-9]*)";
                string tokenPattern = "--remoting-auth-token=([\\w-]*)";

                // Match port and token using regex
                Match portMatch = Regex.Match(output, portPattern);
                Match tokenMatch = Regex.Match(output, tokenPattern);

                if (portMatch.Success && tokenMatch.Success)
                {
                    string port = portMatch.Groups[1].Value;
                    string authToken = tokenMatch.Groups[1].Value;

                    Console.WriteLine("Client found!");

                    return new string[] { port, authToken };
                }

                return new string[] { };
            }
        }
    }
}