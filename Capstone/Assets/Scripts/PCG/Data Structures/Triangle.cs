using System;
using System.Collections.Generic;
using PCG.Data_Structures;
using UnityEngine;

public class Triangle
{
    public Vertex A { get; }
    public Vertex B { get; }
    public Vertex C { get; }
    public List<Vertex> Vertices { get; }
    
    public Edge AB { get; }
    public Edge BC { get; }
    public Edge CA { get; }
    public List<Edge> Edges { get; }
    
    public Circle Circumcircle { get; }

    public Triangle(Vertex a, Vertex b, Vertex c)
    {
        A = a;
        B = b;
        C = c;
        Vertices = new List<Vertex> { A, B, C };
        
        AB = new Edge(A, B);
        BC = new Edge(B, C);
        CA = new Edge(C, A);
        Edges = new List<Edge> { AB, BC, CA };
        
        Circumcircle = CreateCircumcircle();
    }
    
    public bool IsPointInsideCircumcircle(Cell point)
    {
        var dx = point.X - Circumcircle.Center.X;
        var dy = point.Y - Circumcircle.Center.Y;
        var distanceSquared = dx * dx + dy * dy;
        
        var ret = distanceSquared <= Circumcircle.RadiusSquared;
        // Debug.Log($"Checking if point ({point.X}, {point.Y}) is inside circumcircle of triangle with vertices " +
        //           $"({A.X}, {A.Y}), ({B.X}, {B.Y}), ({C.X}, {C.Y})\n" +
        //           $"Circumcenter: ({Circumcircle.Center.X}, {Circumcircle.Center.Y}), radius squared: {Circumcircle.RadiusSquared}\n" +
        //           $"Result: " + ret);
        return ret;
    }

    public Circle CreateCircumcircle()
    {
        var centerNumeratorX = (A.X * A.X + A.Y * A.Y) * (B.Y - C.Y) +
                         (B.X * B.X + B.Y * B.Y) * (C.Y - A.Y) +
                         (C.X * C.X + C.Y * C.Y) * (A.Y - B.Y);
        var centerNumeratorY = (A.X * A.X + A.Y * A.Y) * (C.X - B.X) +
                                (B.X * B.X + B.Y * B.Y) * (A.X - C.X) +
                                (C.X * C.X + C.Y * C.Y) * (B.X - A.X);
        var centerDenominator = 2 * (A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y));
        var x = centerNumeratorX / centerDenominator;
        var y = centerNumeratorY / centerDenominator;
        var center = new Vertex(x, y);

        var radiusNumerator = ((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y)) *
                              ((B.X - C.X) * (B.X - C.X) + (B.Y - C.Y) * (B.Y - C.Y)) * 
                              ((C.X - A.X) * (C.X - A.X) + (C.Y - A.Y) * (C.Y - A.Y));
        var radiusDenominator = centerDenominator * centerDenominator;
        var radiusSquared = radiusNumerator / radiusDenominator;
        
        return new Circle(center, radiusSquared);
    }
    
    public override bool Equals(object obj)
    {
        if (obj is not Triangle other) return false;

        return (A.Equals(other.A) && B.Equals(other.B) && C.Equals(other.C)) ||
               (A.Equals(other.A) && B.Equals(other.C) && C.Equals(other.B)) ||
               (A.Equals(other.B) && B.Equals(other.A) && C.Equals(other.C)) ||
               (A.Equals(other.B) && B.Equals(other.C) && C.Equals(other.A)) ||
               (A.Equals(other.C) && B.Equals(other.A) && C.Equals(other.B)) ||
               (A.Equals(other.C) && B.Equals(other.B) && C.Equals(other.A));
    }
    
    public override int GetHashCode()
    {
        return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
    }

}
