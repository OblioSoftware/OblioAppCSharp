using System;
using System.Text.Json.Nodes;

namespace OblioSoftware;

public class AccessToken
{
    public int request_time     = 0;
    public int expires_in       = 0;
    public string token_type    = "";
    public string access_token  = "";

    public AccessToken FromJsonString(string json)
    {
        JsonNode? result = JsonNode.Parse(json);
        if (result != null)
        {
            access_token = (result["access_token"]!).ToString();
            expires_in = Int32.Parse((result["expires_in"] ?? "0").ToString());
            request_time = Int32.Parse((result["request_time"] ?? "0").ToString());
            token_type = (result["token_type"]!).ToString();
        }
        return this;
    }

    public override string ToString()
    {
        JsonObject data = new JsonObject();
        data["access_token"] = access_token;
        data["expires_in"] = expires_in;
        data["request_time"] = request_time;
        data["token_type"] = token_type;
        return data.ToJsonString();
    }
}