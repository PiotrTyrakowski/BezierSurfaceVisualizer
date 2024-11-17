using System;
using System.Drawing;
using System.Numerics;

namespace BezierSurfaceVisualizer.Utils
{
    public static class MathHelper
    {
        // Konwersja stopni na radiany
        public static float DegreesToRadians(float degrees)
        {
            return (float)(degrees * Math.PI / 180.0);
        }

        // Obliczanie współrzędnych barycentrycznych
        public static Vector3 ComputeBarycentricCoordinates(Point p, Point a, Point b, Point c)
        {
            // Helper function to calculate the area of a triangle
            float RotatedTriangleArea(Point p1, Point p2, Point p3)
            {
                return          (p1.X * (p2.Y - p3.Y) +
                                 p2.X * (p3.Y - p1.Y) +
                                 p3.X * (p1.Y - p2.Y)) / 2.0f;
            }

            // Total area of the triangle ABC
            float totalArea = Math.Abs(RotatedTriangleArea(a, b, c));

            if (totalArea == 0)
                return new Vector3(1.0f/3.0f, 1.0f/3.0f, 1.0f/3.0f);

            // Area of sub-triangles
            float areaPBC = RotatedTriangleArea(p, b, c);
            float areaPCA = RotatedTriangleArea(p, c, a);
            float areaPAB = RotatedTriangleArea(p, a, b);

            // Compute barycentric coordinates
            float w1 = areaPBC / totalArea; // Weight for vertex a
            float w2 = areaPCA / totalArea; // Weight for vertex b
            float w3 = areaPAB / totalArea; // Weight for vertex c

            return new Vector3(w1, w2, w3);
        }


    }
}
