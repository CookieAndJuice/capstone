public class Path
{
    public Vertex A { get; set; }
    public Vertex B { get; set; }
    
    public Path(Vertex a, Vertex b)
    {
        A = a;
        B = b;
    }
    
    public override string ToString()
    {
        return $"{A} - {B}";
    }
}
