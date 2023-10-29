using Newtonsoft.Json;

namespace NaturalRiot.Endpoints;

public  class SummonerEndpoints
{


    public async Task<Summoner> GetSummonerByName(string name)
    {
        var response = await RiotAPI.instance.BaseClient.GetAsync($"/lol/summoner/v4/summoners/by-name/{name}");
        return JsonConvert.DeserializeObject<Summoner>(await response.Content.ReadAsStringAsync());
    }
}