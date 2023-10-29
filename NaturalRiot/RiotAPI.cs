using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using NaturalRiot.Endpoints;
using Newtonsoft.Json;
using NaturalRiot.Endpoints;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NaturalRiot
{

    public class RiotAPI
    {
        public static RiotAPI instance;
        private HttpClient _baseClient = new HttpClient();
        public Endpoints.Endpoints Endpoints { get; }


        public RiotAPI(string apiKey)
        {
            _baseClient.BaseAddress = new Uri("https://na1.api.riotgames.com/");
            _baseClient.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);
            instance = this;
            Endpoints = new Endpoints.Endpoints();
        }

        public HttpClient BaseClient
        {
            get => _baseClient;
        }
    }
}