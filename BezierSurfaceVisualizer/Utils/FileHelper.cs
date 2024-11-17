using System.Numerics;
using System.IO;
using System.Globalization;
using System;

namespace BezierSurfaceVisualizer.Utils
{
    public static class FileHelper
    {
        // Metoda do wczytywania punktów kontrolnych z pliku
        public static Vector3[,] LoadControlPoints(string filePath)
        {
            Vector3[,] controlPoints = new Vector3[4, 4];
            string[] lines;
            try
            {
                string filePathPoints = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
                string projectPath = Path.GetFullPath(Path.Combine(filePathPoints, @"..\..\..", filePath));
                lines = File.ReadAllLines(projectPath);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Please change the path manually to the control_points.txt file in the Utils/FileHelper.cs");

                return null;
            }



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

                x = x * 200;
                y = y * 200;
                z = z * 200;


                controlPoints[i / 4, i % 4] = new Vector3(x, y, z);
            }

            return controlPoints;
        }
    }
}
