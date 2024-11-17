using System.Numerics;

namespace BezierSurfaceVisualizer.Utils
{
    public struct Matrix3x3
    {
        public Vector3 Column1 { get; }
        public Vector3 Column2 { get; }
        public Vector3 Column3 { get; }

        public Matrix3x3(Vector3 column1, Vector3 column2, Vector3 column3)
        {
            Column1 = column1;
            Column2 = column2;
            Column3 = column3;
        }

        public static Vector3 Transform(Vector3 vector, Matrix3x3 matrix)
        {
            return new Vector3(
                Vector3.Dot(vector, new Vector3(matrix.Column1.X, matrix.Column2.X, matrix.Column3.X)),
                Vector3.Dot(vector, new Vector3(matrix.Column1.Y, matrix.Column2.Y, matrix.Column3.Y)),
                Vector3.Dot(vector, new Vector3(matrix.Column1.Z, matrix.Column2.Z, matrix.Column3.Z))
            );
        }
    }
}
