using System;

/// <summary>
/// Model representation of a tile.
/// </summary>
[Serializable]
public class TileData
{
    public int X, Y;
    public string Color;

    public TileData(int x, int y, string color) {
        X = x;
        Y = y;
        Color = color;
    }
}
