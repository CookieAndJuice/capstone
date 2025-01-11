using PCG.Data_Structures;

public class Circle
{
    public Vertex Center { get; }
    public float RadiusSquared { get; }

    public Circle(Vertex center, float radiusSquared)
    {
        Center = center;
        RadiusSquared = radiusSquared;
    }
}