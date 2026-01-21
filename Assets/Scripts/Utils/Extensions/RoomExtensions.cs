using ExitGames.Client.Photon;
using Photon.Realtime;

public static class RoomExtensions
{
    public static void SetCustomProperty(this Room room, string key, object value)
    {
        var table = room.CustomProperties;
        if (table.ContainsKey(key))
        {
            table[key] = value;
        }
        else
        {
            table.Add(key, value);
        }
        room.SetCustomProperties(table);
    }
}
