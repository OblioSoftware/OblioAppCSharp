using System.IO;

namespace OblioSoftware;

class AccessTokenHandlerFileStorage : AccessTokenHandlerInterface
{
    protected string _filepath = "storage/access_token.json";

    public AccessTokenHandlerFileStorage(string? filepath = null)
    {
        if (filepath != null) {
            _filepath = filepath;
        }
    }

    public AccessToken? Get()
    {
        try {
            string jsonString = File.ReadAllText(_filepath);
            AccessToken accessToken = new AccessToken()
                .FromJsonString(jsonString);

            long unixTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            if (accessToken.request_time + accessToken.expires_in > unixTime) {
                return accessToken;
            }
        } catch (Exception e) { }
        return null;
    }

    public void Set(AccessToken accessToken)
    {
        string directory = Path.GetDirectoryName(_filepath);
        if (directory != null && !Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(_filepath, accessToken.ToString());
    }
}