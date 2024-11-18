using System.Collections.Generic;

namespace BezierSurfaceVisualizer.Models
{
    public class Mesh
    {
        public List<Triangle> Triangles { get; set; } = new List<Triangle>();

        public void AddTriangle(Triangle triangle)
        {
            Triangles.Add(triangle);
        }

       
        
    }
}
