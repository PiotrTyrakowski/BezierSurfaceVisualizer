using System.Drawing;
using System.Numerics;

namespace BezierSurfaceVisualizer.Textures
{
    public class TextureManager
    {
        // Tekstura obiektu
        public Bitmap ObjectTexture { get; set; }

        // Mapa wektorów normalnych
        public Bitmap NormalMap { get; set; }

        // Kolor obiektu (gdy nie używamy tekstury)
        public Vector3 ObjectColor { get; set; }

        // Konstruktor
        public TextureManager()
        {
            // Domyślny kolor obiektu
            ObjectColor = new Vector3(30.7f, 0.7f, 0.7f);
        }

        // Metoda do wczytywania tekstury z pliku
        public void LoadObjectTexture(string filePath)
        {
            ObjectTexture = new Bitmap(filePath);
        }

        // Metoda do wczytywania mapy wektorów normalnych z pliku
        public void LoadNormalMap(string filePath)
        {
            NormalMap = new Bitmap(filePath);
        }

        // Metoda do pobierania koloru tekstury dla danych parametrów u, v
        public Vector3 GetTextureColor(float u, float v)
        {
            if (ObjectTexture == null)
                return ObjectColor;

            int x = (int)(u * (ObjectTexture.Width - 1));
            int y = (int)(v * (ObjectTexture.Height - 1));

            // TODO: to something better
            if (x <  0) x = 0;
            if (y < 0) y = 0;

            Color color = ObjectTexture.GetPixel(x, y);

            return new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
        }

        // Metoda do pobierania wektora normalnego z mapy normalnych dla parametrów u, v
        public Vector3 GetNormalVector(float u, float v)
        {
            if (NormalMap == null)
                return new Vector3(0, 0, 1);

            int x = (int)(u * (NormalMap.Width - 1));
            int y = (int)(v * (NormalMap.Height - 1));

            Color color = NormalMap.GetPixel(x, y);

            // Konwersja koloru na wektor normalny
            Vector3 normal = new Vector3(
                (color.R / 255f) * 2 - 1,
                (color.G / 255f) * 2 - 1,
                (color.B / 255f) * 2 - 1);

            return Vector3.Normalize(normal);
        }
    }
}
