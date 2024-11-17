using System;
using BezierSurfaceVisualizer.Models;
using BezierSurfaceVisualizer.Bezier;

namespace BezierSurfaceVisualizer.Triangulation
{
    public class Triangulator
    {
        // Dokładność triangulacji (liczba podziałów w u i v)
        private int divisions;

        // Konstruktor
        public Triangulator(int divisions)
        {
            this.divisions = divisions;
        }

        // Metoda do generowania siatki trójkątów na podstawie powierzchni Beziera
        public Mesh GenerateMesh(BezierSurface surface)
        {
            Mesh mesh = new Mesh();
            float step = 1.0f / divisions;

            for (int i = 0; i < divisions; i++)
            {
                float u1 = i * step;
                float u2 = (i + 1) * step;

                for (int j = 0; j < divisions; j++)
                {
                    float v1 = j * step;
                    float v2 = (j + 1) * step;

                    // Wierzchołki kwadratu
                    Vertex v00 = CreateVertex(surface, u1, v1);
                    Vertex v10 = CreateVertex(surface, u2, v1);
                    Vertex v11 = CreateVertex(surface, u2, v2);
                    Vertex v01 = CreateVertex(surface, u1, v2);

                    // Tworzenie dwóch trójkątów z kwadratu
                    mesh.AddTriangle(new Triangle(v00, v10, v11));
                    mesh.AddTriangle(new Triangle(v00, v11, v01));
                }
            }

            return mesh;
        }

        // Metoda pomocnicza do tworzenia wierzchołka
        private Vertex CreateVertex(BezierSurface surface, float u, float v)
        {
            Vertex vertex = new Vertex(u, v);
            vertex.PBefore = surface.CalculatePoint(u, v);
            vertex.PuBefore = surface.CalculateTangentU(u, v);
            vertex.PvBefore = surface.CalculateTangentV(u, v);
            vertex.NBefore = surface.CalculateNormal(u, v);
            return vertex;
        }
    }
}
