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
        public bool RenderWireframe { get; set; }
        public bool RenderFilled { get; set; }

        private TextureManager textureManager;
        private LightingModel lightingModel;
        public bool useTexture;
        public bool modifyNormal;

        // changed 
        private float[,] zBuffer;

        // Konstruktor
        public Renderer(TextureManager textureManager, LightingModel lightingModel, bool useTexture, bool modifyNormal)
        {
            this.textureManager = textureManager;
            this.lightingModel = lightingModel;
            this.useTexture = useTexture;
            this.modifyNormal = modifyNormal;
        }

        // changed
        public void InitializeZBuffer(int width, int height)
        {
            zBuffer = new float[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    zBuffer[x, y] = float.MaxValue; 
                }
            }
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
            Point[] intPoints = new Point[3];
            for (int i = 0; i < 3; i++)
            {
                intPoints[i] = Point.Round(points[i]);
            }

            int minY = Math.Min(intPoints[0].Y, Math.Min(intPoints[1].Y, intPoints[2].Y));
            int maxY = Math.Max(intPoints[0].Y, Math.Max(intPoints[1].Y, intPoints[2].Y));

            int canvasW = (int)graphics.VisibleClipBounds.Width;
            int canvasH = (int)graphics.VisibleClipBounds.Height;

            // Scanline
            for (int y = minY; y <= maxY; y++)
            {
                List<float> intersections = new List<float>();
                for (int i = 0; i < 3; i++)
                {
                    int next = (i + 1) % 3;
                    if ((intPoints[i].Y <= y && intPoints[next].Y > y)
                        || (intPoints[next].Y <= y && intPoints[i].Y > y))
                    {
                        float dy = intPoints[next].Y - intPoints[i].Y;
                        float t = (y - intPoints[i].Y) / dy;
                        float ix = intPoints[i].X + t * (intPoints[next].X - intPoints[i].X);

                        intersections.Add(ix);
                    }
                }
                if (intersections.Count < 2)
                    continue;

                intersections.Sort();

                float xStart = intersections[0];
                float xEnd = intersections[1];

                if (xStart < 0) xStart = 0;
                if (xEnd >= canvasW) xEnd = canvasW - 1;

                for (int x = (int)Math.Ceiling(xStart); x <= (int)Math.Floor(xEnd); x++)
                {
                    Vector3 barycentric = MathHelper.ComputeBarycentricCoordinates(
                        new Point(x, y),
                        intPoints[0], intPoints[1], intPoints[2]
                    );

                    if (barycentric.X < 0 || barycentric.Y < 0 || barycentric.Z < 0)
                        continue;

                    Vector3 interpolatedPosition =
                        barycentric.X * vertices[0].PAfter +
                        barycentric.Y * vertices[1].PAfter +
                        barycentric.Z * vertices[2].PAfter;

                    float z = interpolatedPosition.Z;

                    if (z < zBuffer[x, y])
                    {
                        zBuffer[x, y] = z;

                        Vector3 interpolatedNormal =
                            barycentric.X * vertices[0].NAfter +
                            barycentric.Y * vertices[1].NAfter +
                            barycentric.Z * vertices[2].NAfter;
                        interpolatedNormal = Vector3.Normalize(interpolatedNormal);

                        if (modifyNormal)
                        {
                            float u = barycentric.X * vertices[0].U +
                                      barycentric.Y * vertices[1].U +
                                      barycentric.Z * vertices[2].U;
                            float v = barycentric.X * vertices[0].V +
                                      barycentric.Y * vertices[1].V +
                                      barycentric.Z * vertices[2].V;

                            Vector3 normalFromMap = textureManager.GetNormalVector(u, v);

                            
                            Matrix3x3 M = new Matrix3x3(
                                vertices[0].PuAfter,
                                vertices[0].PvAfter,
                                vertices[0].NAfter
                            );

                            Vector3 finalNormal = Matrix3x3.Transform(normalFromMap, M);
                            interpolatedNormal = Vector3.Normalize(finalNormal);
                        }

                        Vector3 objColor;
                        if (useTexture)
                        {
                            float u = barycentric.X * vertices[0].U +
                                      barycentric.Y * vertices[1].U +
                                      barycentric.Z * vertices[2].U;
                            float v = barycentric.X * vertices[0].V +
                                      barycentric.Y * vertices[1].V +
                                      barycentric.Z * vertices[2].V;

                            objColor = textureManager.GetTextureColor(u, v);
                        }
                        else
                        {
                            objColor = textureManager.ObjectColor;
                        }

                        Color pixelColor = lightingModel.CalculateColor(interpolatedNormal, objColor, interpolatedPosition);

                        graphics.FillRectangle(new SolidBrush(pixelColor), x, y, 1, 1);
                    }
                }
            }
        }
      

        
    }
}
