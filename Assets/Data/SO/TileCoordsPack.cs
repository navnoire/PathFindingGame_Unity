using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TileTexturecoordsPack")]
public class TileCoordsPack : ScriptableObject
{
    [System.Serializable]
    public class TextureRect
    {
        /*
         * x = x coordinate in texture
         * y = y coordinate of ordinary texture
         * z = y coordinate of texture with emission (if any)
         */
        public TileType type;
        public Vector3Int ld;
        public Vector3Int rd;
        public Vector3Int lt;
        public Vector3Int rt;

    }

    public TextureRect[] coordArray;


}

public enum TileType
{
    Crook,
    Fourcorner,
    Line,
    Start,
    Background
}
