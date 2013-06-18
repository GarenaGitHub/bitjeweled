using UnityEngine;

/// <summary>
/// Grid positions.
/// Utility class.
/// </summary>
public static class GridPositions
{
    private static Vector2[,] positions = new Vector2[10, 10];
	
	/// <summary>
	/// Initializes the <see cref="GridPositions"/> class.
	/// </summary>
    static GridPositions()
    {
        initAll();
    }
	
	/// <summary>
	/// Gets the vector from the grid position.
	/// </summary>
	/// <returns>
	/// The vector.
	/// </returns>
	/// <param name='x'>
	/// X.
	/// </param>
	/// <param name='y'>
	/// Y.
	/// </param>
    public static Vector2 GetVector(int x, int y)
    {
        return positions[x, y];
    }

    public static void Init()
    {
        initAll();
    }
	
	/// <summary>
	/// Gets the grid position.
	/// </summary>
	/// <returns>
	/// The grid position.
	/// </returns>
	/// <param name='worldCoords'>
	/// World coords.
	/// </param>
    public static GridPoint GetGridPosition(Vector2 worldCoords)
    {
        GridPoint gp = new GridPoint();
        gp.x = -1;
        gp.y = -1;
        for (int y = 0; y < Board.Instance.rows; y++)
        {
            for (int x = 0; x < Board.Instance.columns; x++)
            {
                if (Board.gridDescriptor[x, y] != (int)TileType.NoTile && Board.gridDescriptor[x, y] != (int)TileType.BlockedTile && Mathf.Abs(Vector2.Distance(worldCoords, positions[x, y])) <= Board.halfStep)
                {
                    gp.x = x;
                    gp.y = y;
                    return gp;
                }
            }
        }
        return gp;
    }
	
	/// <summary>
	/// Sets the position.
	/// </summary>
	/// <param name='x'>
	/// X.
	/// </param>
	/// <param name='y'>
	/// Y.
	/// </param>
	/// <param name='xp'>
	/// Xp.
	/// </param>
	/// <param name='yp'>
	/// Yp.
	/// </param>
    public static void SetPosition(int x, int y, float xp, float yp)
    {
        positions[x, y] = new Vector2(xp, yp);
    }
	
	/// <summary>
	/// Inits all.
	/// </summary>
    private static void initAll()
    {
        positions = new Vector2[Board.Instance.columns, Board.Instance.rows];
        for (int y = 0; y < Board.Instance.rows; y++)
        {
            for (int x = 0; x < Board.Instance.columns; x++)
            {
                positions[x, y] = new Vector2(0, 0);
            }
        }
    }
}
