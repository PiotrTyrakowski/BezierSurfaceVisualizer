namespace BezierSurfaceVisualizer.Models
{
    public class Triangle
    {
        // Trzy wierzchołki trójkąta
        public Vertex Vertex1 { get; set; }
        public Vertex Vertex2 { get; set; }
        public Vertex Vertex3 { get; set; }

        // Konstruktor
        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            Vertex1 = v1;
            Vertex2 = v2;
            Vertex3 = v3;
        }
    }
}
