using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    // Singleton instance
    public static GridManager instance;

    // Variables to set up the Grid as a 2D array
    public int[,] Grid;
    public int Columns;
    public int Rows;

    // Offset used to manange tiles that have spawned in the negative x/z axis (A 2D array cant have negative positions)
    public int offset;

    public void Awake()
    {
        // Initialise Singleton
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
    }

    void Start()
    {
        // Initialises the grid as a 2D array of dimemsions defined in the inspector
        Grid = new int[Columns, Rows];

        // Sets each value in the array to a zero (Zero means the tile isnt occupied and a room can spawn)
        for(int i = 0; i < Columns; i += 1)
        {
            for (int j = 0; j < Rows; j ++)
            {
                Grid[i, j] = 0;
            }
        }
    }

    public void ResetGrid()
    {
        for (int i = 0; i < Columns; i += 1)
        {
            for (int j = 0; j < Rows; j++)
            {
                Grid[i, j] = 0;
            }
        }
    }
}
