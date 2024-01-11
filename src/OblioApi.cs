using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OblioSoftware;

class OblioApi
{
    protected string _cif       = "";
    protected string _email     = "";
    protected string _secret    = "";
    protected string _baseURL   = "https://www.oblio.eu";
    protected AccessTokenHandlerInterface _accessTokenHandler;

    public OblioApi(string email, string secret, AccessTokenHandlerInterface? accessTokenHandler = null)
    {
        _email = email;
        _secret = secret;
        if (accessTokenHandler == null) {
            _accessTokenHandler = new AccessTokenHandlerFileStorage();
        }
    }

    public void SetCif(string cif)
    {
        _cif = cif;
    }

    public string GetCif()
    {
        if (_cif == "") {
            throw new Exception("Empty cif");
        }
        return _cif;
    }

    public JsonObject Nomenclature(string type, Dictionary<string, string>? filters = null)
    {
        string cif = "";
        switch (type) {
            case "companies":
                break;
            case "vat_rates":
            case "products":
            case "clients":
            case "series":
            case "languages":
            case "management":
                cif = GetCif();
                break;
            default:
                throw new Exception("Type not implemented");
        }

        if (filters == null) {
            filters = new Dictionary<string, string>();
        }
        if (cif != "") {
            filters.Add("cif", cif);
        }
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        foreach (var value in filters) {
            queryString.Add(value.Key, value.Value);
        }

        HttpClient httpClient = BuildRequest();
        HttpResponseMessage response = httpClient.GetAsync("/api/nomenclature/" + type + "?" + queryString.ToString()).Result;

        return GetResult(response);
    }

    public JsonObject CreateDoc(string type, JsonObject data)
    {
        CheckType(type);
        StringContent content = new StringContent(data.ToJsonString(), Encoding.UTF8, "application/json");
        HttpClient httpClient = BuildRequest();
        HttpResponseMessage response = httpClient.PostAsync("/api/docs/" + type, content).Result;

        return GetResult(response);
    }

    public HttpClient BuildRequest()
    {
        AccessToken accessToken = GetAccessToken();
        HttpClient httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_baseURL);
        httpClient.DefaultRequestHeaders.Add("Authorization", accessToken.token_type + " " + accessToken.access_token);
        return httpClient;
    }

    public AccessToken GetAccessToken()
    {
        AccessToken? accessToken = _accessTokenHandler.Get();
        if (accessToken == null) {
            accessToken = GenerateAccessToken();
            _accessTokenHandler.Set(accessToken);
        }
        return accessToken;
    }

    protected AccessToken GenerateAccessToken()
    {
        JsonObject payload = new JsonObject();
        payload["client_id"] = _email;
        payload["client_secret"] = _secret;
        payload["grant_type"] = "client_credentials";

        StringContent data = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json");
        HttpClient httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_baseURL);
        HttpResponseMessage response = httpClient.PostAsync("/api/authorize/token", data).Result;
        CheckResponse(response);

        string json = response.Content.ReadAsStringAsync().Result;

        return new AccessToken().FromJsonString(json);
    }

    protected void CheckType(string type)
    {
        string[] array = { "invoice", "proforma", "notice", "receipt" };
        if (!array.Contains(type)) {
            throw new Exception("Type not supported");
        }
    }

    protected void CheckResponse(HttpResponseMessage response)
    {
        if (((int)response.StatusCode) < 200 || ((int)response.StatusCode) >= 300) {

            string json = response.Content.ReadAsStringAsync().Result;
            JsonNode? node = JsonNode.Parse(json);
            JsonObject result = new JsonObject();
            if (node != null) {
                result = (JsonObject)node;
            }
            if (result["statusMessage"] is null) {
                result["statusMessage"] = string.Format("Error! HTTP response status: %d", ((int)response.StatusCode));
            }
            throw new Exception(result["statusMessage"]!.ToString());
        }
    }

    protected JsonObject GetResult(HttpResponseMessage response)
    {
        CheckResponse(response);
        string json = response.Content.ReadAsStringAsync().Result;

        JsonNode? node = JsonNode.Parse(json);
        if (node == null) {
            return new JsonObject();
        }
        return (JsonObject)node;
    }
}