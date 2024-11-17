using System.Numerics;
using System.IO;
using System.Globalization;

namespace BezierSurfaceVisualizer.Utils
{
    public static class FileHelper
    {
        // Metoda do wczytywania punktów kontrolnych z pliku
        public static Vector3[,] LoadControlPoints(string filePath)
        {
            Vector3[,] controlPoints = new Vector3[4, 4];

            string[] lines = File.ReadAllLines("C:\\Users\\Lenovo T590\\source\\repos\\BezierSurfaceVisualizer\\BezierSurfaceVisualizer\\control_points.txt");

            for (int i = 0; i < 16; i++)
            {
                string[] parts = lines[i].Split(' ', ',', '\t');
                float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
                float y, z;

                if (parts.Length == 3)
                {
                    y = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    z = float.Parse(parts[2], CultureInfo.InvariantCulture);
                }
                else
                {
                    y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                    z = float.Parse(parts[4], CultureInfo.InvariantCulture);
                }
        

                controlPoints[i / 4, i % 4] = new Vector3(x, y, z);
            }

            return controlPoints;
        }
    }
}
