using System.Numerics;
using BezierSurfaceVisualizer.Models;
using BezierSurfaceVisualizer.Utils;

namespace BezierSurfaceVisualizer.Transformations
{
    public class Transformer
    {
        // Kąty obrotu w stopniach
        private float alpha; // obrót wokół osi X
        private float beta;  // obrót wokół osi Z

        // Konstruktor
        public Transformer(float alpha, float beta)
        {
            this.alpha = alpha;
            this.beta = beta;
        }

        // Metoda do obracania wierzchołka
        public void RotateVertex(Vertex vertex)
        {
            Matrix4x4 rotationMatrix = GetRotationMatrix();

            // Obrót punktu
            Vector3 point = Vector3.Transform(vertex.PBefore, rotationMatrix);
            vertex.PAfter = point;

            // Obrót wektorów stycznych
            vertex.PuAfter = Vector3.TransformNormal(vertex.PuBefore, rotationMatrix);
            vertex.PvAfter = Vector3.TransformNormal(vertex.PvBefore, rotationMatrix);

            // Obrót wektora normalnego
            vertex.NAfter = Vector3.TransformNormal(vertex.NBefore, rotationMatrix);
        }

        // Metoda do tworzenia macierzy obrotu
        private Matrix4x4 GetRotationMatrix()
        {
            float alphaRad = MathHelper.DegreesToRadians(alpha);
            float betaRad = MathHelper.DegreesToRadians(beta);

            // there is no mistake
            Matrix4x4 rotationX = Matrix4x4.CreateRotationY(alphaRad);
            Matrix4x4 rotationZ = Matrix4x4.CreateRotationZ(betaRad); 
         

            return rotationX * rotationZ;
        }
    }
}
