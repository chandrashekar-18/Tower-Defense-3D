namespace TowerDefense.Grid
{
    /// <summary>
    /// Enum for different cell types in the grid
    /// </summary>
    public enum CellType
    {
        Empty,      // Can place towers
        Path,       // Enemy path
        SpawnPoint, // Enemy spawn point
        ExitPoint,  // Enemy exit point
        Obstacle    // Cannot place towers
    }
}