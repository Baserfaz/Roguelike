using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour {

    // Create a sprite on top of the tile 
    // and modify the tile to be wall.

    // how this works:
    // 1. add this script to a tile gameobject.
    // 2. call createIceblock function. 

    private GameObject iceBlockGo;
    private Tile.TileType originalTileType;
    private int timeToLive = 0;

    public void CreateIceBlock(int ttl, Vector2 pos, Tile.TileType tileType)
    {
        // update information about the state of the tile.
        timeToLive = ttl;
        originalTileType = tileType;

        // get the tile we are on.
        GameObject go = DungeonGenerator.instance.GetTileAtPos(pos);
        Tile tile = go.GetComponent<Tile>();

        // change the tile to be a wall.
        tile.myType = Tile.TileType.Wall;

        // create ice block.
        iceBlockGo = new GameObject("IceBlock");
        
        // parent the block
        iceBlockGo.transform.SetParent(tile.transform);

        // position the block
        iceBlockGo.transform.localPosition = new Vector3(0f, 0f, GameMaster.instance.itemZLevel - 1);

        // add sprite renderer
        SpriteRenderer sr = iceBlockGo.AddComponent<SpriteRenderer>();
        
        // set the sprite.
        sr.sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.IceBlock);

        // set the material.
        sr.material = MaterialManager.instance.mySpriteMaterial;

    }

    // called from GameMaster.EndTurn().
    public void Tick()
    {
        timeToLive--;

        if (timeToLive == 0)
        {
            // reset the tile settings
            // and destroy this script.
            GameObject go = DungeonGenerator.instance.GetTileAtPos(GetComponent<Tile>().position);
            go.GetComponent<Tile>().myType = originalTileType;

            Destroy(iceBlockGo);
            Destroy(this);
        }
    }


}
