using System.Collections.Generic;

public class GameScreenController : ScreenController
{
    public readonly int PlayerMaxLives;
    public readonly List<SliceArea> PlayerSliceAreas = new List<SliceArea>();

    public GameScreenController(int playerMaxLives, List<SliceArea> sliceAreas)
    {
        PlayerMaxLives = playerMaxLives;
        PlayerSliceAreas = sliceAreas;
    }

}
