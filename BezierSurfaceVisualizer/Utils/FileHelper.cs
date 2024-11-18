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

            // find the biggest and the smallest value and scale it to fit the screen
            float maxX = controlPoints[0, 0].X;
            float maxY = controlPoints[0, 0].Y;
            float maxZ = controlPoints[0, 0].Z;
            float minX = controlPoints[0, 0].X;
            float minY = controlPoints[0, 0].Y;
            float minZ = controlPoints[0, 0].Z;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (controlPoints[i, j].X > maxX)
                    {
                        maxX = controlPoints[i, j].X;
                    }
                    if (controlPoints[i, j].Y > maxY)
                    {
                        maxY = controlPoints[i, j].Y;
                    }
                    if (controlPoints[i, j].Z > maxZ)
                    {
                        maxZ = controlPoints[i, j].Z;
                    }
                    if (controlPoints[i, j].X < minX)
                    {
                        minX = controlPoints[i, j].X;
                    }
                    if (controlPoints[i, j].Y < minY)
                    {
                        minY = controlPoints[i, j].Y;
                    }
                    if (controlPoints[i, j].Z < minZ)
                    {
                        minZ = controlPoints[i, j].Z;
                    }
                }
            }

            float scaleX = 1;
            float scaleY = 1;
            float scaleZ = 1;

            if (maxX - minX > 0)
            {
                scaleX = 200 / (maxX - minX);
            }
            if (maxY - minY > 0)
            {
                scaleY = 200 / (maxY - minY);
            }

            if (maxZ - minZ > 0)
            {
                scaleZ = 200 / (maxZ - minZ);
            }

            if (minZ < 0)
            {
                scaleZ = -scaleZ;
            }
            if (maxZ < 0)
            {
                scaleZ = -scaleZ;
            }
            if (minZ > 0)
            {
                scaleZ = scaleZ;
            }
           

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    controlPoints[i, j] = new Vector3(controlPoints[i, j].X * scaleX * 2, controlPoints[i, j].Y * scaleY * 2, controlPoints[i, j].Z * scaleZ * 2);
                }
            }

            return controlPoints;
        }
    }
}
