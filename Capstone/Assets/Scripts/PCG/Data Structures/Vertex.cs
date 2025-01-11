using UnityEngine;

public class Vertex
{
    public float X { get; }
    public float Y { get; }

    public Vertex(float x, float y)
    {
        X = x;
        Y = y;
    }

    // Override Equals to check for equality based on coordinates
    public override bool Equals(object obj)
    {
        if (obj is not Vertex other) return false;
        
        return Mathf.Approximately(X, other.X) && 
               Mathf.Approximately(Y, other.Y);
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }
    
    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}