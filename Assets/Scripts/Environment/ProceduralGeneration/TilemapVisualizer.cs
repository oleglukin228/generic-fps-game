using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilemapVisualizer : MonoBehaviour
{
    public Transform floorMeshParent;
    public GameObject[] wallVariants, wallDecorationVariants, floorDecorationVariants;

    public void PaintFloorTiles(IEnumerable<Vector3Int> floorPositions)
    {
        //PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector3Int> positions, Tilemap tilemap, GameObject tile)
    {
        var tileBase = ScriptableObject.CreateInstance<Tile>();
        tileBase.gameObject = tile;
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tileBase, position);
        }
    }

    private void CreateFloorMesh()
    {

    }

    internal void PaintSingleBasicWall(Vector3Int position, string binaryType, int gridSize, Transform parent = null)
    {
        //Debug.Log(position + " type: " + binaryType);
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        int wallIndex = Random.Range(0, wallVariants.Length);

        if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            var gO = Instantiate(wallVariants[wallIndex], position, Quaternion.identity);
            gO.name = "wallTop";
            gO.transform.SetParent(parent);

            gO = Instantiate(wallVariants[wallIndex], position + new Vector3(0, 0, gridSize), Quaternion.AngleAxis(90f, Vector3.up));
            gO.name = "wallRight";
            gO.transform.SetParent(parent);

            gO = Instantiate(wallVariants[wallIndex], position + new Vector3(gridSize, 0, 0), Quaternion.AngleAxis(-90f, Vector3.up));
            gO.name = "wallLeft";
            gO.transform.SetParent(parent);

            gO = Instantiate(wallVariants[wallIndex], position + new Vector3(gridSize, 0, gridSize), Quaternion.AngleAxis(180f, Vector3.up));
            gO.name = "wallBottom";
            gO.transform.SetParent(parent);

            return;
        }
        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            //tileBase.gameObject = wallSideRight;
            var gO = Instantiate(wallVariants[wallIndex], position, Quaternion.identity);
            gO.name = "wallTop";
            gO.transform.SetParent(parent);
        }
        if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            //tileBase.gameObject = wallSideRight;
            var gO = Instantiate(wallVariants[wallIndex], position + new Vector3(0, 0, gridSize), Quaternion.AngleAxis(90f, Vector3.up));
            gO.name = "wallRight";
            gO.transform.SetParent(parent);
        }
        if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            //tileBase.gameObject = wallSideLeft;
            var gO = Instantiate(wallVariants[wallIndex], position + new Vector3(gridSize, 0, 0), Quaternion.AngleAxis(-90f, Vector3.up));
            gO.name = "wallLeft";
            gO.transform.SetParent(parent);
        }
        if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            //tileBase.gameObject = wallSideBottom;
            var gO = Instantiate(wallVariants[wallIndex], position + new Vector3(gridSize, 0, gridSize), Quaternion.AngleAxis(180f, Vector3.up));
            gO.name = "wallBottom";
            gO.transform.SetParent(parent);
        }
        //if (tileBase.gameObject != null) PaintSingleTile(wallTilemap, tileBase, position);
    }

    internal void PaintSingleBackgroundWall(Vector3Int position, string binaryType, int gridSize, Transform parent = null)
    {
        int wallIndex = Random.Range(0, wallDecorationVariants.Length);
        var gO = Instantiate(wallDecorationVariants[wallIndex], position + new Vector3(gridSize / 2, 0, gridSize / 2),
            Quaternion.Euler(0, Random.Range(0f, 360f), 0), parent);
        gO.name = "backgroundWall";
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tileBase, Vector3Int position)
    {
        var tilePosition = tilemap.WorldToCell(position);
        tilemap.SetTile(tilePosition, tileBase);
    }

    /*internal void PaintSingleCornerWall(Vector3Int position, string binaryType, Transform wallsParent, int gridSize)
    {
        //Debug.Log(position + " type: " + binaryType);
        int typeASInt = Convert.ToInt32(binaryType, 2);
        //var tileBase = ScriptableObject.CreateInstance<Tile>();
        
        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeASInt))
        {
            //tileBase.gameObject = wallInnerCornerDownLeft;
            var gO = new GameObject("wallInnerCornerDownLeft");
            gO.transform.parent = wallsParent;
            Instantiate(wallVariants, position + new Vector3Int(gridSize, 0, gridSize), Quaternion.AngleAxis(180f, Vector3.up), gO.transform);
            Instantiate(wallVariants, position + new Vector3Int(gridSize, 0, 0), Quaternion.AngleAxis(-90f, Vector3.up), gO.transform);
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeASInt))
        {
            //tileBase.gameObject = wallInnerCornerDownRight;
            var gO = new GameObject("wallInnerCornerDownRight");
            gO.transform.parent = wallsParent;
            Instantiate(wallVariants, position + new Vector3Int(gridSize, 0, gridSize), Quaternion.AngleAxis(180f, Vector3.up), gO.transform);
            Instantiate(wallVariants, position + new Vector3Int(0, 0, gridSize), Quaternion.AngleAxis(90f, Vector3.up), gO.transform);
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeASInt))
        {
            //tileBase.gameObject = wallDiagonalCornerDownLeft;
            var gO = new GameObject("wallDiagonalCornerDownLeft");
            gO.transform.parent = wallsParent;
            Instantiate(wallVariants, position + new Vector3Int(0, 0, 0), Quaternion.AngleAxis(-180f, Vector3.up), gO.transform);
            Instantiate(wallVariants, position + new Vector3Int(0, 0, gridSize), Quaternion.AngleAxis(90f, Vector3.up), gO.transform);
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeASInt))
        {
            //tileBase.gameObject = wallDiagonalCornerDownRight;
            var gO = new GameObject("wallDiagonalCornerDownRight");
            gO.transform.parent = wallsParent;
            Instantiate(wallVariants, position + new Vector3Int(0, 0, 0), Quaternion.AngleAxis(-180f, Vector3.up), gO.transform);
            Instantiate(wallVariants, position + new Vector3Int(gridSize, 0, 0), Quaternion.AngleAxis(-90f, Vector3.up), gO.transform);
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeASInt))
        {
            //tileBase.gameObject = wallDiagonalCornerUpRight;
            //var gO = new GameObject("wallDiagonalCornerUpRight");
            //gO.transform.parent = wallsParent;
            //Instantiate(wallTop, position + new Vector3Int(0, 0, gridSize), Quaternion.identity, gO.transform);
            //Instantiate(wallTop, position + new Vector3Int(gridSize, 0, gridSize), Quaternion.AngleAxis(90f, Vector3.up), gO.transform);
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeASInt))
        {
            //tileBase.gameObject = wallDiagonalCornerUpLeft;
            //var gO = new GameObject("wallDiagonalCornerUpLeft");
            //gO.transform.parent = wallsParent;
            //Instantiate(wallTop, position + new Vector3Int(0, 0, 0), Quaternion.identity, gO.transform);
            //Instantiate(wallTop, position + new Vector3Int(gridSize, 0, 0), Quaternion.AngleAxis(-90f, Vector3.up), gO.transform);
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeASInt))
        {
            //tileBase.gameObject = wallSideBottom;
            //return position;
        }
        //if (tileBase.gameObject != null) PaintSingleTile(wallTilemap, tileBase, position);
    }*/
}
