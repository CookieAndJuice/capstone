using System;

public enum CellType
{
    Blank,
    Room,
    Wall,
    Corridor
}

public class Cell
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public CellType Type { get; set; }
    
    public Cell(int x, int y, CellType type=CellType.Blank)
    {
        X = x;
        Y = y;
        Type = type;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Cell other = (Cell)obj;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

}
