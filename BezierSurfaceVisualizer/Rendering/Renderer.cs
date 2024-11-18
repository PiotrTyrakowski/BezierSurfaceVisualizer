using System.Drawing;
using BezierSurfaceVisualizer.Models;
using BezierSurfaceVisualizer.Lighting;
using BezierSurfaceVisualizer.Textures;
using System.Collections.Generic;
using System.Numerics;
using BezierSurfaceVisualizer.Utils;
using System;
//using Accord.Math;


namespace BezierSurfaceVisualizer.Rendering
{
    public class Renderer
    {
        // Opcje renderowania
        public bool RenderWireframe { get; set; }
        public bool RenderFilled { get; set; }

        // Kolor obiektu lub tekstura
        private TextureManager textureManager;
        private LightingModel lightingModel;
        public bool useTexture;
        public bool modifyNormal;

        // Konstruktor
        public Renderer(TextureManager textureManager, LightingModel lightingModel, bool useTexture, bool modifyNormal)
        {
            this.textureManager = textureManager;
            this.lightingModel = lightingModel;
            this.useTexture = useTexture;
            this.modifyNormal = modifyNormal;
        }

        // Metoda do renderowania siatki na canvasie
        public void RenderMesh(Graphics graphics, Mesh mesh, int canvasWidth, int canvasHeight)
        {
            foreach (var triangle in mesh.Triangles)
            {
                List<Vertex> vertices = new List<Vertex> { triangle.Vertex1, triangle.Vertex2, triangle.Vertex3 };

                // Rzutowanie punktów na płaszczyznę XY
                PointF[] projectedPoints = new PointF[3];
                for (int i = 0; i < 3; i++)
                {
                    projectedPoints[i] = ProjectToCanvas(vertices[i].PAfter, canvasWidth, canvasHeight);
                }

                if (RenderFilled)
                {
                    // Wypełnianie trójkąta
                    FillTriangle(graphics, projectedPoints, vertices);
                }

                if (RenderWireframe)
                {
                    // Rysowanie krawędzi trójkąta
                    graphics.DrawPolygon(Pens.Black, projectedPoints);
                }
            }
        }

        // Metoda do rzutowania punktu na canvas
        private PointF ProjectToCanvas(Vector3 point, int canvasWidth, int canvasHeight)
        {
            float x = point.X + canvasWidth / 2;
            float y = -point.Y + canvasHeight / 2; // Odwrócenie osi Y

            return new PointF(x, y);
        }

        // Metoda do wypełniania trójkąta
        private void FillTriangle(Graphics graphics, PointF[] points, List<Vertex> vertices)
        {
            // Przygotowanie punktów i kolorów
            Point[] intPoints = new Point[3];
            for (int i = 0; i < 3; i++)
            {
                intPoints[i] = Point.Round(points[i]);
            }

            int minY = (int)Math.Min(intPoints[0].Y, Math.Min(intPoints[1].Y, intPoints[2].Y));
            int maxY = (int)Math.Max(intPoints[0].Y, Math.Max(intPoints[1].Y, intPoints[2].Y));

            
            
            for (int y = minY; y <= maxY; y++)
            {
              
                List<int> intersections = new List<int>();
                for (int i = 0; i < 3; i++)
                {
                    int next = (i + 1) % 3;
                    if ((intPoints[i].Y <= y && intPoints[next].Y > y) || (intPoints[next].Y <= y && intPoints[i].Y > y))
                    {
                        float x = intPoints[i].X + (float)(y - intPoints[i].Y) / (intPoints[next].Y - intPoints[i].Y) * (intPoints[next].X - intPoints[i].X);
                        intersections.Add((int)x);
                    }
                }
                if (intersections.Count < 2)
                    continue; 

                intersections.Sort();


                float xStart = intersections[0];
                float xEnd = intersections[1];

                // Clamp to canvas boundaries
                xStart = Math.Max(xStart, 0);
                xEnd = Math.Min(xEnd, graphics.VisibleClipBounds.Width - 1);

                // Wypełnianie linii między parzystymi przecięciami
               
                for (int x = (int)Math.Ceiling(xStart); x <= (int)Math.Floor(xEnd); x++)
                {
                    // Obliczanie współrzędnych barycentrycznych
                    Vector3 barycentric = MathHelper.ComputeBarycentricCoordinates(new Point(x, y), intPoints[0], intPoints[1], intPoints[2]);

                    if (barycentric.X < 0 || barycentric.Y < 0 || barycentric.Z < 0)
                        continue;

                    // Interpolacja wektora normalnego i współrzędnej z
                    Vector3 interpolatedNormal = barycentric.X * vertices[0].NAfter + barycentric.Y * vertices[1].NAfter + barycentric.Z * vertices[2].NAfter;
                       
                    interpolatedNormal = Vector3.Normalize(interpolatedNormal);

                   
                    Vector3 interpolatedPosition =
                        barycentric.X * vertices[0].PAfter +
                        barycentric.Y * vertices[1].PAfter +
                        barycentric.Z * vertices[2].PAfter;

                    //interpolatedPosition = Vector3.Normalize(interpolatedPosition);



                    

                    Vector3 objectColor;
                    if (useTexture)
                    {          
                        objectColor = textureManager.GetTextureColor(
                            barycentric.X * vertices[0].U + barycentric.Y * vertices[1].U + barycentric.Z * vertices[2].U,
                            barycentric.X * vertices[0].V + barycentric.Y * vertices[1].V + barycentric.Z * vertices[2].V);
                    }
                    else
                    {
                        objectColor = textureManager.ObjectColor;
                    }

                    if (modifyNormal)
                    {
                        // Modyfikacja wektora normalnego na podstawie mapy normalnych
                        Vector3 Ntexture = textureManager.GetNormalVector(
                            barycentric.X * vertices[0].U + barycentric.Y * vertices[1].U + barycentric.Z * vertices[2].U,
                            barycentric.X * vertices[0].V + barycentric.Y * vertices[1].V + barycentric.Z * vertices[2].V);

                        Matrix3x3 M = new Matrix3x3(
                            vertices[0].PuAfter, vertices[0].PvAfter, vertices[0].NAfter);

                        interpolatedNormal = Matrix3x3.Transform(Ntexture, M);
                        interpolatedNormal = Vector3.Normalize(interpolatedNormal);
                    }

                    Color color = lightingModel.CalculateColor(interpolatedNormal, objectColor, interpolatedPosition);

                    // Rysowanie piksela
                    graphics.FillRectangle(new SolidBrush(color), x, y, 1, 1);
                }
                
            }
        }
    }
}
