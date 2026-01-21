public class Constants 
{
    public static class Assets
    {
        public const string TARGET_SPRITE_PATH = "Sprites/Targets/{0}";
        public const string TARGET_PREFAB_FORMAT = "Target_{0}";
    }

    public static class Scenes
    {
        public const string GAME = "Game";
        public const string MENU = "Menu";
    }

    public static class Tags
    {
        public const string TARGET_TAG = "Target";
        public const string DEATH_PLANE_TAG = "Death Plane";
    }

    public static class Networking
    {
        public const int MAX_PLAYERS_IN_ROOM = 4;
        public const string PLAYER_PING = "ping";
        public const string PLAYER_READY = "ready";
        public const string ROOM_NAME = "name";
        public const string ROOM_IS_PLAYING = "is playing";
    }
}
