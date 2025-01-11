public class Map
{
    public Cell[,] Cells { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    public Map(int width, int height)
    {
        Width = width;
        Height = height;
        
        Cells = new Cell[Height, Width];
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                Cells[y, x] = new Cell(x, y);
    }
    
    public void DeleteCell(Cell cell)
    {
        Cells[cell.Y, cell.X].Type = CellType.Blank; 
    }
}
