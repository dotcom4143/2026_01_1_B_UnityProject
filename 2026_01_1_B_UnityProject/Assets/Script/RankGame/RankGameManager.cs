using UnityEngine;
using System.Collections.Generic;

public class RankGameManager : MonoBehaviour
{
    public int gridWidth = 7;
    public int gridHeight = 7;

    public float CellSize = 1.3f;

    public GameObject cellPrefabs;
    public Transform gridContainer;

    public GameObject rankPrefab;
    public Sprite[] rankSprites;

    public int maxRankLevel = 7;

    public GridCell[,] grid;

    void InitializeGrid()
    {
        grid = new GridCell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(
                    x * CellSize - (gridWidth * CellSize / 2) + CellSize / 2,
                    y * CellSize - (gridWidth * CellSize / 2) + CellSize / 2,
                    1f
                );

                GameObject cellObj = Instantiate(cellPrefabs, position, Quaternion.identity, gridContainer);
                GridCell cell = cellObj.AddComponent<GridCell>();
                cell.Initialize(x, y);

                grid[x, y] = cell;
            }
        }
    }




    void Start()
    {
        InitializeGrid();

        for (int i = 0; i < 4; i++)
        {
            SpawnNewRank();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            SpawnNewRank();
        }
    }

    public DraggableRank createRankInCell(GridCell cell, int level)
    {
        if(cell == null || !cell.isEmpty()) return null;

        level = Mathf.Clamp(level, 1, maxRankLevel);

        Vector3 rankPosition = new Vector3(cell.transform.position.x, cell.transform.position.y, 0f);

        GameObject rankObj = Instantiate(rankPrefab, rankPosition, Quaternion.identity, gridContainer);
        rankObj.name = "Rank_Level_" + level;

        DraggableRank rank = rankObj.AddComponent<DraggableRank>();

        rank.SetRankLevel(level);

        cell.SetRank(rank);

        return rank;
    }

    private GridCell FineEmptyCell()
    {
        List<GridCell> emptyCells = new List<GridCell>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].isEmpty())
                {
                    emptyCells.Add(grid[x, y]);
                }
            }
        }
        if (emptyCells.Count == 0)
        {
            return null;
        }

        return emptyCells[Random.Range(0,emptyCells.Count)];
    }

    public bool SpawnNewRank()
    {
        GridCell emptyCell = FineEmptyCell();
        if(emptyCell == null) return false;

        int rankLevel = Random.Range(0, 100) < 80 ? 1 : 2;

        createRankInCell(emptyCell, rankLevel);

        return true;
    }

    public GridCell FindClosesteCell(Vector3 position)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {   
                if (grid[x, y].ContainsPosition(position))
                {
                    return grid[x, y];
                }
            }
        }

        GridCell closestCell = null;
        float closestDistance = float.MaxValue;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float distance = Vector3.Distance(position, grid[x, y].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCell = grid[x, y];
                }
            }
        }
        
        if (closestDistance > CellSize * 2)
        {
            return null;
        }
        return closestCell;
    }

    public void RemoveRank(DraggableRank rank)
    {
        if (rank == null) return;
        
        if (rank.currentCell != null)
        {
            rank.currentCell.currentRank = null;
        }

        Destroy(rank.gameObject);
    }

    public void MergeRanks(DraggableRank draggableRank, DraggableRank targetRank)
    {
        if (draggableRank == null || targetRank == null || draggableRank.rankLevel != targetRank.rankLevel)
        {
            if (draggableRank != null) draggableRank.ReturnToOriginalPosition();
            return;
        }

        int newLevel = targetRank.rankLevel + 1;
        if (newLevel > maxRankLevel)
        {
            RemoveRank(draggableRank);
            return;
        }

        targetRank.SetRankLevel(newLevel);
        RemoveRank(draggableRank);
    }


}
