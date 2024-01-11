using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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
            access_token = (string)(result["access_token"] ?? "");
            expires_in = Int32.Parse((result["expires_in"] ?? "0").ToString());
            request_time = Int32.Parse((result["request_time"] ?? "0").ToString());
            token_type = (string)(result["token_type"] ?? "");
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