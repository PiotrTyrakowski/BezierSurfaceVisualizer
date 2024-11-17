using System.Collections.Generic;

namespace BezierSurfaceVisualizer.Models
{
    public class Mesh
    {
        // Lista trójkątów w siatce
        public List<Triangle> Triangles { get; set; } = new List<Triangle>();

        // Metoda dodająca trójkąt do siatki
        public void AddTriangle(Triangle triangle)
        {
            Triangles.Add(triangle);
        }

       
        
    }
}
