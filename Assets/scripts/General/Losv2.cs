using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Losv2 : MonoBehaviour {

    List<Tile> listOfTilesUp = new List<Tile>();
    List<Tile> listOfTilesDown = new List<Tile>();
    List<Tile> listOfTilesLeft = new List<Tile>();
    List<Tile> listOfTilesRight = new List<Tile>();

    public enum Direction { Up, Down, Right, Left, UpLeft, UpRight, DownLeft, DownRight };

    private bool CheckGap(Tile tile, Direction dir)
    {

        /* Checks if the line goes through a gap.
         * -> if it does then we don't want to add those tiles to the list.
         * --> because we don't want that the player can see around the corner.
         * 
         * ex.
         * 
         *  - "i" = vision 
         *  - "x" = wall
         *  - "\" and "/" = checkGap function which checks the walls.
         *  
         *   xxxxxxxx
         *   x   i  x
         *   x   i  x
         *   x   i  x
         *   xxxxixxx
         *      \i/
         *      \i/
         *       
         */ 

        bool areBothwalls = false;
        GameObject aGo = null;
        GameObject bGo = null;
        Tile a = null;
        Tile b = null;

        switch (dir)
        {
            case Direction.Down:

                aGo = DungeonGenerator.instance.GetTileAtPos(tile.position + new Vector2(-1, -1));
                bGo = DungeonGenerator.instance.GetTileAtPos(tile.position + new Vector2(1, -1));

                if (aGo == null || bGo == null)
                {
                    return false;
                }

                a = aGo.GetComponent<Tile>();
                b = bGo.GetComponent<Tile>();

                if (a.myType == Tile.TileType.Wall || b.myType == Tile.TileType.Wall) areBothwalls = true;
                break;
            case Direction.Up:

                aGo = DungeonGenerator.instance.GetTileAtPos(tile.position + new Vector2(-1, 1));
                bGo = DungeonGenerator.instance.GetTileAtPos(tile.position + new Vector2(1, 1));

                if (aGo == null || bGo == null)
                {
                    return false;
                }

                a = aGo.GetComponent<Tile>();
                b = bGo.GetComponent<Tile>();

                if (a.myType == Tile.TileType.Wall || b.myType == Tile.TileType.Wall) areBothwalls = true;
                break;
            case Direction.Right:

                aGo = DungeonGenerator.instance.GetTileAtPos(tile.position + new Vector2(1, 1));
                bGo = DungeonGenerator.instance.GetTileAtPos(tile.position + new Vector2(1, -1));

                if (aGo == null || bGo == null)
                {
                    return false;
                }

                a = aGo.GetComponent<Tile>();
                b = bGo.GetComponent<Tile>();

                if (a.myType == Tile.TileType.Wall || b.myType == Tile.TileType.Wall) areBothwalls = true;
                break;
            case Direction.Left:

                aGo = DungeonGenerator.instance.GetTileAtPos(tile.position + new Vector2(-1, 1));
                bGo = DungeonGenerator.instance.GetTileAtPos(tile.position + new Vector2(-1, -1));

                if (aGo == null || bGo == null)
                {
                    return false;
                }

                a = aGo.GetComponent<Tile>();
                b = bGo.GetComponent<Tile>();

                if (a.myType == Tile.TileType.Wall || b.myType == Tile.TileType.Wall) areBothwalls = true;
                break;
        }

        return areBothwalls;
    }

    private void CastVisionline(Direction dir, Tile startTile, bool addToList = true)
    {
        int offset = 0;
        bool preventTilesToBeAddedToList = false;

        while (true)
        {

            Tile t = null;

            switch (dir)
            {

                    // check whether the line goes through a gap between two walls.
                    // -> don't add tiles to list (from now on).
                    // --> because otherwise player could see behind corners.
                case Direction.Down:
                    t = DungeonGenerator.instance.GetTileAtPos(startTile.position - new Vector2(0f, offset)).GetComponent<Tile>();

                    //if(preventTilesToBeAddedToList == false) preventTilesToBeAddedToList = CheckGap(t, Direction.Down);

                    break;
                case Direction.Up:
                    t = DungeonGenerator.instance.GetTileAtPos(startTile.position + new Vector2(0f, offset)).GetComponent<Tile>();

                    //if (preventTilesToBeAddedToList == false) preventTilesToBeAddedToList = CheckGap(t, Direction.Up);

                    break;
                case Direction.Left:
                    t = DungeonGenerator.instance.GetTileAtPos(startTile.position - new Vector2(offset, 0f)).GetComponent<Tile>();

                    //if (preventTilesToBeAddedToList == false) preventTilesToBeAddedToList = CheckGap(t, Direction.Left);

                    break;
                case Direction.Right:
                    t = DungeonGenerator.instance.GetTileAtPos(startTile.position + new Vector2(offset, 0f)).GetComponent<Tile>();

                    //if (preventTilesToBeAddedToList == false) preventTilesToBeAddedToList = CheckGap(t, Direction.Right);

                    break;
                    // ----------->


                case Direction.DownLeft:
                    t = DungeonGenerator.instance.GetTileAtPos(startTile.position + new Vector2(-offset, -offset)).GetComponent<Tile>();
                    break;
                case Direction.DownRight:
                    t = DungeonGenerator.instance.GetTileAtPos(startTile.position + new Vector2(offset, -offset)).GetComponent<Tile>();
                    break;
                case Direction.UpLeft:
                    t = DungeonGenerator.instance.GetTileAtPos(startTile.position + new Vector2(-offset, offset)).GetComponent<Tile>();
                    break;
                case Direction.UpRight:
                    t = DungeonGenerator.instance.GetTileAtPos(startTile.position + new Vector2(offset, offset)).GetComponent<Tile>();
                    break;
            }

            // set the tile visible.
            // and also discover it.
            t.isVisible = true;
            t.isDiscovered = true;

            // if tile is wall or an enemy -> break
            if (GameMaster.instance.enemiesBlockLoS)
            {
                if (t.actor != null)
                {
                    if (t.actor.GetComponent<Enemy>() != null) break;
                }
            }

            if (t.myType == Tile.TileType.Wall || t.myType == Tile.TileType.OuterWall || t.myType == Tile.TileType.DoorClosed)
            {
                break;
            }

            // add tile to a certain list.
            if (addToList && preventTilesToBeAddedToList == false)
            {
                switch (dir)
                {
                    case Direction.Down:
                        listOfTilesDown.Add(t);
                        break;
                    case Direction.Up:
                        listOfTilesUp.Add(t);
                        break;
                    case Direction.Left:
                        listOfTilesLeft.Add(t);
                        break;
                    case Direction.Right:
                        listOfTilesRight.Add(t);
                        break;
                }
            }

            // increment offset.
            offset++;
        }
    }

    private void ResetVision()
    {
        foreach (GameObject g in DungeonGenerator.instance.GetTiles())
        {
            Tile tile = g.GetComponent<Tile>();
            tile.GetComponent<Tile>().isVisible = false;
        }
    }

    private void ActivateEnemies(Tile startTile)
    {
        foreach (GameObject g in DungeonGenerator.instance.GetTiles())
        {
            Tile tile = g.GetComponent<Tile>();

            if (tile.isVisible == false) continue;

            // calculate the distance
            int distance = Mathf.FloorToInt(Mathf.Abs(startTile.position.x - tile.position.x) + Mathf.Abs(startTile.position.y - tile.position.y));

            // if the enemy is further than aggro range, just continue.
            if (distance > GameMaster.instance.enemyAggroRange) continue;

            if (tile.actor != null)
            {
                if (tile.actor.GetComponent<Enemy>() != null)
                {
                    Enemy enemy = tile.actor.GetComponent<Enemy>();

					if(enemy.isActive) {

						// only update the targetPosition.
						enemy.targetPosition = startTile.position;

					} else {

						// this is the first time this enemy is
						// activated ("aggroed").

						enemy.targetPosition = startTile.position;
						enemy.isActive = true;

						// create GUI pop up about this event.
						GUIManager.instance.CreatePopUpEntry("<color=#FF0000>Aggro</color>",
							enemy.position, GUIManager.PopUpType.Other);
					}
                }
            }
        }
    }

    public void CalculateLoS()
    {
        Player player = GetComponent<Player>();
        Tile startTile = DungeonGenerator.instance.GetTileAtPos(player.position).GetComponent<Tile>();

        // reset vision.
        ResetVision();

        // our position is visible too
        startTile.isVisible = true;

        // cast linear lines.
        CastVisionline(Direction.Down, startTile);
        CastVisionline(Direction.Up, startTile);
        CastVisionline(Direction.Right, startTile);
        CastVisionline(Direction.Left, startTile);

        // cast diagonal lines.
        CastVisionline(Direction.DownLeft, startTile, false);
        CastVisionline(Direction.DownRight, startTile, false);
        CastVisionline(Direction.UpLeft, startTile, false);
        CastVisionline(Direction.UpRight, startTile, false);

        // CastVisionline adds all found tiles to a list
        // now we need to go through that list and cast lines 
        // from all of those tiles diagonally.

        foreach (Tile t in listOfTilesDown)
        {
            CastVisionline(Direction.DownLeft, t, false);
            CastVisionline(Direction.DownRight, t, false);
        }

        foreach (Tile t in listOfTilesLeft)
        {
            CastVisionline(Direction.UpLeft, t, false);
            CastVisionline(Direction.DownLeft, t, false);
        }

        foreach (Tile t in listOfTilesRight)
        {
            CastVisionline(Direction.UpRight, t, false);
            CastVisionline(Direction.DownRight, t, false);
        }

        foreach (Tile t in listOfTilesUp)
        {
            CastVisionline(Direction.UpLeft, t, false);
            CastVisionline(Direction.UpRight, t, false);
        }

        // activate enemies
        ActivateEnemies(startTile);

        // reset lists.
        listOfTilesDown.Clear();
        listOfTilesLeft.Clear();
        listOfTilesRight.Clear();
        listOfTilesUp.Clear();
    }
}
