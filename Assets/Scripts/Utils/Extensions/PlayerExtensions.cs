using ExitGames.Client.Photon;
using Photon.Realtime;

public static class PlayerExtensions
{
    public static void CreateCustomProperties(this Player player, float ping, bool ready)
    {
        Hashtable customProperties = new Hashtable
        {
            { Constants.Networking.PLAYER_PING, ping },
            { Constants.Networking.PLAYER_READY, ready }
        };
        player.SetCustomProperties(customProperties);
    }

    public static float GetPing(this Player player)
    {
        float ping = (float)player.CustomProperties[Constants.Networking.PLAYER_PING];
        return ping;
    }

    public static void SetCustomProperty(this Player player, string key, object value)
    {
        Hashtable properties = player.CustomProperties;
        properties[key] = value;
        player.SetCustomProperties(properties);
    }
}
