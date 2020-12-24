using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayFieldManager : MonoBehaviour
{

    public int maxRows;
    public int maxCols;
    public GameObject BlockManager;
    public ReplicaCube replicaCurbePrefab;
    public GameObject FrameCube;
    private const float ShiftAmount = 1.0f;
    private float yPos = 0.5f;
    public bool[,] valueGrid;
    public FloatVariable levelProgressPercentage;
    public ReplicaCubeObjectPool replicaCubeObjectPool;
    private int frameTrueCount;
    private int fieldEmptyCount;
    private const float LevelCompletePercentageForFalseCount = 0.1f;
    private float limitToFillCompletely;
    private int fillCounter;
    public bool shouldGenerateFrame;
    

    private void Awake()
    {
        GameEvents.OnChangeGridValue += OnChangeGridValue;
        GameEvents.OnFloodFill += PerformFloodFill;
        
        valueGrid = new bool[maxCols, maxRows];
        replicaCubeObjectPool.SpawnPool(); // Create an object pool for replica cubes.
    }

    private void Start()
    {
        if (shouldGenerateFrame)
            GenerateFrame(BlockManager);
        fieldEmptyCount = (maxCols * maxRows) - frameTrueCount; // subtract wall cube count from playfield size.
        levelProgressPercentage.value = 0;
        limitToFillCompletely = fieldEmptyCount * LevelCompletePercentageForFalseCount; // if player covers certain area of field. Level will be completed.
        fillCounter = 0;                                                                // Currently player has to fill %90 of the area.
    }

    private void OnChangeGridValue(int col, int row)
    {
        valueGrid[col, row] = true;
    }

    private void GenerateFrame(GameObject blockManager)
    {
        float currentSpawnX = blockManager.transform.position.x;
        float currentSpawnZ = blockManager.transform.position.z;

        for (int i = 0; i < maxRows; i++)
        {
            currentSpawnX = blockManager.transform.position.x;
            if (i == 0 || i == maxRows - 1)
            {
                for (int m = 0; m < maxCols; m++)
                {
                    GameObject newFrameCube = Instantiate(FrameCube, new Vector3(currentSpawnX, yPos, currentSpawnZ), Quaternion.identity);
                    newFrameCube.transform.SetParent(BlockManager.transform);
                    valueGrid[m, i] = true;
                    frameTrueCount++;
                    currentSpawnX += ShiftAmount;
                }
            }
            else
            {

                for (int m = 0; m < maxCols; m++)
                {
                    if (m == 0 || m == maxCols - 1)
                    {
                        GameObject newFrameCube = Instantiate(FrameCube, new Vector3(currentSpawnX, yPos, currentSpawnZ), Quaternion.identity);
                        newFrameCube.transform.SetParent(BlockManager.transform);
                        valueGrid[m, i] = true;
                        frameTrueCount++;
                    }
                    else
                    {
                        if (i == 14 && m >= 8 && m <= 10)
                        {
                            GameObject newFrameCube = Instantiate(FrameCube, new Vector3(currentSpawnX, yPos, currentSpawnZ), Quaternion.identity);
                            newFrameCube.transform.SetParent(BlockManager.transform);
                            valueGrid[m, i] = true;
                        }
                        else if (i == 5 && ((m >= 1 && m <= 4) || (m >= 14 && m <= 17)))
                        {
                            GameObject newFrameCube = Instantiate(FrameCube, new Vector3(currentSpawnX, yPos, currentSpawnZ), Quaternion.identity);
                            newFrameCube.transform.SetParent(BlockManager.transform);
                            frameTrueCount++;
                            valueGrid[m, i] = true;
                        }

                    }
                    currentSpawnX += ShiftAmount;
                }


            }
            currentSpawnZ -= ShiftAmount;
        }
    }

    /// <summary>
    /// Performs flood fill operation when player hits wall or a filled cube.
    /// </summary>
    private void PerformFloodFill()
    {
        fillCounter++; // Fill counter is kept to decide player should procede to bonus round or not. 
        GameEvents.Instance.FillCountChangeInvoker(fillCounter); // Currently player procedes to bonus round for a fillCount <= 10

        // Flood fill is performed 2 times because players can create multiple closed areas with a move.
        // In the second run, if still there is a closed are it is also filled.

        for (int i = 0; i < 2; i++) 
        {                           
            bool[,] gridCopy = new bool[maxCols, maxRows];
            bool[,] gridCopySecond = new bool[maxCols, maxRows];
            gridCopy = (bool[,])valueGrid.Clone();
            gridCopySecond = (bool[,])valueGrid.Clone();
            bool[,] gridToPrint = FloodFill(gridCopy, gridCopySecond);
            SetProgressBar(gridToPrint);
            valueGrid = (bool[,])gridToPrint.Clone();
        }        

        if (levelProgressPercentage.value == 1)
            GameEvents.Instance.LevelCompleteInvoker(fillCounter);
    }

    private void SetProgressBar(bool[,] grid)
    {
        int trueCountInGrid = GetTrueCountOfGrid(grid);
        trueCountInGrid -= frameTrueCount;
        levelProgressPercentage.value = (float)trueCountInGrid / fieldEmptyCount;
        GameEvents.Instance.ProgressChangeInvoker();
    }

    private int GetTrueCountOfGrid(bool[,] grid)
    {
        int trueCount = 0;
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = 0; row < maxRows; row++)
            {
                if (grid[col, row])
                    trueCount++;
            }
        }
        return trueCount;
    }

    /// <summary>
    /// Main function to perform flood fill.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="gridCopySecond"></param>
    /// <returns> A grid which is representing final decision of the algorithm to fill</returns>
    private bool[,] FloodFill(bool[,] grid, bool[,] gridCopySecond)
    {
        Queue<Point> q = new Queue<Point>();       
        int randIndex;
        List<Point> testPointList = new List<Point>();
        bool[,] gridCopy3 = (bool[,])grid.Clone();
        int falseCountBeforeFill = FindFalseCountandPositions(grid, ref testPointList, gridCopy3); // get empty square count and position list of them

        if(falseCountBeforeFill < limitToFillCompletely) // if %90 of the area is covered fill completely.
        {
            GetReplicaCubesFromPool(testPointList);
            return gridCopy3;
        }

        Point randPoint = new Point();
        do
        {
            randIndex = Random.Range(0, testPointList.Count); // choose a random false value point amongst false value positions.
            randPoint = testPointList[randIndex];
        } while (grid[randPoint.X, randPoint.Y]); // just to be sure selected point is representing a false value.

        // perform 4-way flood fill algorithm according to randomly selected point
        List<Point> firstTryPointList = new List<Point>();
        q.Enqueue(randPoint);
        int trueCount = 0;
        while (q.Count > 0)
        {
            Point n = q.Dequeue();
            if (grid[n.X, n.Y])
                continue;
            Point w = n, e = new Point(n.X + 1, n.Y);
            while ((w.X >= 0) && !grid[w.X, w.Y])
            {
                grid[w.X, w.Y] = true;
                firstTryPointList.Add(new Point(w.X, w.Y));
                trueCount++;
                if ((w.Y > 0) && !grid[w.X, w.Y - 1])
                    q.Enqueue(new Point(w.X, w.Y - 1));
                if ((w.Y < maxRows - 1) && !grid[w.X, w.Y + 1])
                    q.Enqueue(new Point(w.X, w.Y + 1));
                w.X--;
            }
            while ((e.X <= maxCols - 1) && !grid[e.X, e.Y])
            {
                grid[e.X, e.Y] = true;
                firstTryPointList.Add(new Point(e.X, e.Y));
                trueCount++;
                if ((e.Y > 0) && !grid[e.X, e.Y - 1])
                    q.Enqueue(new Point(e.X, e.Y - 1));
                if ((e.Y < maxRows - 1) && !grid[e.X, e.Y + 1])
                    q.Enqueue(new Point(e.X, e.Y + 1));
                e.X++;
            }
        }
        List<Point> secondTryPointList = new List<Point>();
        int falseCount = FindFalseCountandPositions(grid, ref secondTryPointList, gridCopySecond); // get empty square count after fill

        // compare filled area with unfilled area and if unfilled area is bigger,
        // spawn cubes at the locations specified in flood fill.
        if (falseCount > trueCount) 
        {
            GetReplicaCubesFromPool(firstTryPointList);
            return grid;
        }

        if (secondTryPointList.Count > 0) // otherwise spawn cubes at the other area.
            GetReplicaCubesFromPool(secondTryPointList);
        return gridCopySecond;
    }

    /// <summary>
    /// Get replica cubes from object pool for specified points.
    /// </summary>
    /// <param name="pointList"></param>
    private void GetReplicaCubesFromPool(List<Point> pointList)
    {
        foreach (Point point in pointList)
        {
            Vector3 repCubePos = FindTransformFromPoint(point);
            ReplicaCube replicaCube = replicaCubeObjectPool.GetObjectFromPool(repCubePos, Quaternion.identity); ;
            replicaCube.filled = true;
        }
    }

    /// <summary>
    /// Converts transform position to grid location.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private Vector3 FindTransformFromPoint(Point point)
    {
        return new Vector3((float)(point.X - 9.0f), 0.45f, (float)(14.0f - point.Y));        
    }

    /// <summary>
    /// Get false value count from grid array and keep points of them.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="pointList"></param>
    /// <param name="gridCopy2"></param>
    /// <returns></returns>
    private int FindFalseCountandPositions(bool[,] grid, ref List<Point> pointList, bool[,] gridCopy2) 
    {
        int falseCount = 0;
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = 0; row < maxRows; row++)
            {
                if (grid[col, row])
                    continue;
                falseCount++;
                gridCopy2[col, row] = true;
                pointList.Add(new Point(col, row));
            }
        }

        return falseCount;
    }

    private void OnDestroy()
    {
        GameEvents.OnChangeGridValue -= OnChangeGridValue;
        GameEvents.OnFloodFill -= PerformFloodFill;       
    }
}
