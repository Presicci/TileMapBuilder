using System;
using UnityEngine;

/// <summary>
/// Model representation of a map.
/// </summary>
[Serializable]
public class MapData
{
    public string Name;
    public int XLength;
    public int YLength;
    public TileData[] Tiles;

    public MapData(string name, int xLength, int yLength, MapBuilder_Tile[] mbTiles) {
        Name = name;
        XLength = xLength;
        YLength = yLength;
        Tiles = new TileData[mbTiles.Length];
        // Conver MapBuilder_Tile array into TileData array
        for (int index = 0; index < mbTiles.Length; index++) {
            MapBuilder_Tile mbTile = mbTiles[index];
            Tiles[index] = new TileData(mbTile.X, mbTile.Y, "#" + ColorUtility.ToHtmlStringRGB(mbTile.SpriteRenderer.color));
        }
    }
}
