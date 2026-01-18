using ExitGames.Client.Photon;
using Photon.Realtime;

public static class RoomExtensions
{
    public static string GetRoomName(this RoomInfo room)
    {
        return (string)room.CustomProperties[Constants.Networking.ROOM_NAME];
    }
}
