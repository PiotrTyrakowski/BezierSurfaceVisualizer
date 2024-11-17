using System.Numerics;
//using Accord.Math;

namespace BezierSurfaceVisualizer.Models
{
    public class Vertex
    {
        // Punkt P przed obrotem
        public Vector3 PBefore { get; set; }

        // Punkt P po obrocie
        public Vector3 PAfter { get; set; }

        // Wektory styczne Pu przed i po obrocie
        public Vector3 PuBefore { get; set; }
        public Vector3 PuAfter { get; set; }

        // Wektory styczne Pv przed i po obrocie
        public Vector3 PvBefore { get; set; }
        public Vector3 PvAfter { get; set; }

        // Wektory normalne N przed i po obrocie
        public Vector3 NBefore { get; set; }
        public Vector3 NAfter { get; set; }

        // Parametry u i v
        public float U { get; set; }
        public float V { get; set; }

        // Konstruktor
        public Vertex(float u, float v)
        {
            U = u;
            V = v;
        }
    }
}
