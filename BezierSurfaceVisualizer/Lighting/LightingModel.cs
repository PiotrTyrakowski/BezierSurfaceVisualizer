using System.Numerics;
using System.Drawing;
using BezierSurfaceVisualizer.Utils;
using System;

namespace BezierSurfaceVisualizer.Lighting
{
    public class LightingModel
    {
        public bool EnableLight { get; set; } = true;
        public bool EnableSpotlights { get; set; } = false;
        public float SpotlightFocus { get; set; } = 10.0f;

        // Positions, directions, and colors of the spotlights
        private Vector3[] SpotlightPositions;
        private Vector3[] SpotlightDirections;
        private Vector3[] SpotlightColors;

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
         
            Kd = 0.5f;
            Ks = 0.5f;
            M = 10;
            LightColor = new Vector3(1, 1, 1);
            LightPosition = new Vector3(0, 0, 20);

            SpotlightFocus = 10.0f;
            EnableLight = true;
            EnableSpotlights = false;

            float positionOffset = 400f;
            float zPosition = LightPosition.Z;

            SpotlightPositions = new Vector3[3];
            SpotlightDirections = new Vector3[3];
            SpotlightColors = new Vector3[3];

            // lewy górny róg (czerwony reflektor)
            SpotlightPositions[0] = new Vector3(-positionOffset, positionOffset, zPosition);
            SpotlightDirections[0] = Vector3.Normalize(new Vector3(0, 0, 20) - SpotlightPositions[0]);
            SpotlightColors[0] = new Vector3(1, 0, 0); 

            // lewy dolny róg (zielony reflektor)
            SpotlightPositions[1] = new Vector3(-positionOffset, -positionOffset, zPosition);
            SpotlightDirections[1] = Vector3.Normalize(new Vector3(0, 0, 20) - SpotlightPositions[1]);
            SpotlightColors[1] = new Vector3(0, 1, 0); // 

            // prawy dolny róg (niebieski reflektor)
            SpotlightPositions[2] = new Vector3(positionOffset, -positionOffset, zPosition);
            SpotlightDirections[2] = Vector3.Normalize(new Vector3(0, 0, 20) - SpotlightPositions[2]);
            SpotlightColors[2] = new Vector3(0, 0, 1); // 
        }
        public void SetLightPosition(Vector3 position)
        {
            LightPosition = position;
            SpotlightPositions[0] = new Vector3(position.X, position.Y, position.Z);
            SpotlightPositions[1] = new Vector3(position.X, position.Y, position.Z);
            SpotlightPositions[2] = new Vector3(position.X, position.Y, position.Z);
        }


        public Color CalculateColor(Vector3 normal, Vector3 objectColor, Vector3 position)
        {
            Vector3 N = Vector3.Normalize(normal);
            Vector3 V = new Vector3(0, 0, 1); // pozycja viewera

            Vector3 color = Vector3.Zero;

            if (EnableLight)
            {
                // Regular light
                Vector3 L = Vector3.Normalize(LightPosition - position);
                Vector3 R = Vector3.Reflect(-L, N);

                float cosNL = Math.Max(Vector3.Dot(N, L), 0);
                float cosVRm = (float)Math.Pow(Math.Max(Vector3.Dot(V, R), 0), M);

                Vector3 diffuse = Kd * LightColor * objectColor * cosNL;
                Vector3 specular = Ks * LightColor * objectColor * cosVRm;

                color += diffuse + specular;
            }

            if (EnableSpotlights)
            {
               
                for (int i = 0; i < 3; i++)
                {
                    Vector3 L = Vector3.Normalize(SpotlightPositions[i] - position);
                    Vector3 L_dir = SpotlightDirections[i];

                    float cosAlpha = Vector3.Dot(L, L_dir);

                    if (cosAlpha > 0)
                    {
                        float attenuation = (float)Math.Pow(cosAlpha, SpotlightFocus);

                        Vector3 R = Vector3.Reflect(-L, N);

                        float cosNL = Math.Max(Vector3.Dot(N, L), 0);
                        float cosVRm = (float)Math.Pow(Math.Max(Vector3.Dot(V, R), 0), M);

                        Vector3 diffuse = Kd * SpotlightColors[i] * objectColor * cosNL * attenuation;
                        Vector3 specular = Ks * SpotlightColors[i] * objectColor * cosVRm * attenuation;

                        color += diffuse + specular;
                    }
                }
            }

            color = Vector3.Clamp(color, Vector3.Zero, new Vector3(1, 1, 1));

            return Color.FromArgb(
                (int)(color.X * 255),
                (int)(color.Y * 255),
                (int)(color.Z * 255));
        }

    }
}
