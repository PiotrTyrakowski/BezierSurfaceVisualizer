using System.Numerics;
using System.Drawing;
using BezierSurfaceVisualizer.Utils;
using System;

namespace BezierSurfaceVisualizer.Lighting
{
    public class LightingModel
    {
        // Współczynniki
        public float Kd { get; set; }
        public float Ks { get; set; }
        public int M { get; set; }

        // Kolor światła
        public Vector3 LightColor { get; set; }

        // Pozycja światła
        public Vector3 LightPosition { get; set; }

        // Konstruktor
        public LightingModel()
        {
            // Domyślne wartości
            Kd = 0.5f;
            Ks = 0.5f;
            M = 10;
            LightColor = new Vector3(1, 1, 1);
            LightPosition = new Vector3(0, 0, 100);
        }
        public void SetLightPosition(Vector3 position)
        {
            LightPosition = position;
        }


        // Metoda do obliczania koloru wypełnienia dla danego wierzchołka
        public Color CalculateColor(Vector3 normal, Vector3 objectColor)
        {
            Vector3 N = Vector3.Normalize(normal);
            Vector3 L = Vector3.Normalize(LightPosition - new Vector3(0, 0, 0));
            Vector3 V = new Vector3(0, 0, 1);
            Vector3 R = Vector3.Reflect(-L, N);

            float cosNL = Math.Max(Vector3.Dot(N, L), 0); // Kosinus między normalną a kierunkiem światła
            float cosVRm = (float)Math.Pow(Math.Max(Vector3.Dot(V, R), 0), M); // Kosinus dla refleksji


            Vector3 diffuse = Kd * LightColor * objectColor * cosNL;
            Vector3 specular = Ks * LightColor * objectColor * cosVRm;

            Vector3 color = diffuse + specular;

            color = Vector3.Clamp(color, Vector3.Zero, new Vector3(1, 1, 1));
            return Color.FromArgb(
                (int)(color.X * 255),
                (int)(color.Y * 255),
                (int)(color.Z * 255));
        }
    }
}
