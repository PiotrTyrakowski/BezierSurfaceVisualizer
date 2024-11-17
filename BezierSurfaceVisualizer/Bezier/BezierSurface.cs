using System;
using System.Numerics;

namespace BezierSurfaceVisualizer.Bezier
{
    public class BezierSurface
    {
        // Macierz punktów kontrolnych (4x4)
        private Vector3[,] controlPoints = new Vector3[4, 4];

        // Konstruktor
        public BezierSurface(Vector3[,] controlPoints)
        {
            this.controlPoints = controlPoints;
        }

        // Metoda do obliczania punktu na powierzchni dla parametrów u, v
        public Vector3 CalculatePoint(float u, float v)
        {
            Vector3 point = Vector3.Zero;
            int n = 3;
            int m = 3;

            for (int i = 0; i <= n; i++)
            {
                float bernsteinU = Bernstein(n, i, u);
                for (int j = 0; j <= m; j++)
                {
                    float bernsteinV = Bernstein(m, j, v);
                    point += controlPoints[i, j] * bernsteinU * bernsteinV;
                }
            }

            return point;
        }

        // Metoda do obliczania wektora stycznego względem u
        public Vector3 CalculateTangentU(float u, float v)
        {
            Vector3 tangent = Vector3.Zero;
            int n = 3;
            int m = 3;

            for (int i = 0; i < n; i++)
            {
                float bernsteinU = Bernstein(n - 1, i, u);
                for (int j = 0; j <= m; j++)
                {
                    float bernsteinV = Bernstein(m, j, v);
                    Vector3 delta = controlPoints[i + 1, j] - controlPoints[i, j];
                    tangent += delta * bernsteinU * bernsteinV * n;
                }
            }

            return tangent;
        }

        // Metoda do obliczania wektora stycznego względem v
        public Vector3 CalculateTangentV(float u, float v)
        {
            Vector3 tangent = Vector3.Zero;
            int n = 3;
            int m = 3;

            for (int i = 0; i <= n; i++)
            {
                float bernsteinU = Bernstein(n, i, u);
                for (int j = 0; j < m; j++)
                {
                    float bernsteinV = Bernstein(m - 1, j, v);
                    Vector3 delta = controlPoints[i, j + 1] - controlPoints[i, j];
                    tangent += delta * bernsteinU * bernsteinV * m;
                }
            }

            return tangent;
        }

        // Metoda do obliczania wektora normalnego
        public Vector3 CalculateNormal(float u, float v)
        {
            Vector3 Pu = CalculateTangentU(u, v);
            Vector3 Pv = CalculateTangentV(u, v);
            Vector3 normal = Vector3.Cross(Pu, Pv);
            return Vector3.Normalize(normal);
        }

        // Funkcja Bernsteina
        private float Bernstein(int n, int i, float t)
        {
            return BinomialCoefficient(n, i) * (float)Math.Pow(t, i) * (float)Math.Pow(1 - t, n - i);
        }

        // Współczynnik dwumianowy
        private int BinomialCoefficient(int n, int k)
        {
            int result = 1;

            for (int i = 1; i <= k; i++)
            {
                result *= n--;
                result /= i;
            }

            return result;
        }
    }
}
