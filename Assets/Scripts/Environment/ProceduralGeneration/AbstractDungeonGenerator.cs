using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] protected Vector3Int startPosition = Vector3Int.zero;

    public void GenerateDungeon()
    {
        RunProceduralGeneration();
    }

    public void ClearDungeon()
    {
        RunClearDungeon();
    }

    protected abstract void RunProceduralGeneration();
    protected abstract void RunClearDungeon();
}
