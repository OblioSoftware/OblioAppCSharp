
using OblioSoftware;
using System.Text.Json.Nodes;

class Program
{
    public static void Main(string[] args)
    {
        JsonObject env = GetEnv();
        try
        {
            string jsonString = @"{
                ""cif"": """",
                ""client"": {
                    ""cif"": ""19"",
                    ""name"": ""Bucur Obor SRL"",
                    ""phone"": ""0800900900""
                },
                ""seriesName"": """",
                ""language"": ""RO"",
                ""precision"": 2,
                ""currency"": ""RON"",
                ""collect"": {},
                ""referenceDocument"": {},
                ""products"": [
                    {
                        ""name""         : ""Abonament"",
                        ""code""         : """",
                        ""description""  : """",
                        ""price""        : ""100"",
                        ""measuringUnit"": ""buc"",
                        ""currency""     : ""RON"",
                        ""vatName""      : ""Normala"",
                        ""vatPercentage"": 19,
                        ""vatIncluded""  : true,
                        ""quantity""     : 2,
                        ""productType""  : ""Serviciu""
                    }
                ],
                ""issuerName"": ""Ion Popescu"",
                ""issuerId"": 1234567890123,
                ""noticeNumber"": """",
                ""internalNote"": ""Factura emisa din API"",
                ""deputyName"": ""George Popescu"",
                ""deputyIdentityCard"": ""ID 1234"",
                ""deputyAuto"": ""CT 12345"",
                ""selesAgent"": ""Marian Popescu"",
                ""mentions"": ""Factura de test emisa din API"",
                ""workStation"": ""Sediu"",
                ""useStock"": 0
            }";

            JsonNode? node = JsonNode.Parse(jsonString);
            if (node == null) {
                throw new Exception("Parse error jsonString");
            }
            JsonObject data = (JsonObject)node;
            data["cif"] = env["cif"]!.ToString();
            data["seriesName"] = env["seriesName"]!.ToString();

            OblioApi api = new OblioApi(env["email"]!.ToString(), env["secret"]!.ToString());
            api.SetCif(env["cif"]!.ToString());
            // JsonObject response = api.Nomenclature("products");
            JsonObject response = api.CreateDoc("invoice", data);
            Console.WriteLine(response.ToString());
            Console.WriteLine(response["data"]!["link"]!);
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }

    public static JsonObject GetEnv()
    {
        try {
            string jsonString = File.ReadAllText("storage/appsecrets.json");
            JsonNode? node = JsonNode.Parse(jsonString);
            if (node != null) {
                return (JsonObject)node;
            }
        }
        catch (Exception e) { }
        return new JsonObject();
    }
}