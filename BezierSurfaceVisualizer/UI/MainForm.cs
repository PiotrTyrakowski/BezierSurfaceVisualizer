using System;
using System.Windows.Forms;
using BezierSurfaceVisualizer.Models;
using BezierSurfaceVisualizer.Bezier;
using BezierSurfaceVisualizer.Triangulation;
using BezierSurfaceVisualizer.Transformations;
using BezierSurfaceVisualizer.Rendering;
using BezierSurfaceVisualizer.Lighting;
using BezierSurfaceVisualizer.Textures;
using BezierSurfaceVisualizer.Utils;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace BezierSurfaceVisualizer.UI
{
    public class MainForm : Form
    {
        // Kontrolki interfejsu użytkownika
        private TrackBar divisionTrackBar;
        private TrackBar alphaTrackBar;
        private TrackBar betaTrackBar;
        private TrackBar kdTrackBar;
        private TrackBar ksTrackBar;
        private TrackBar mTrackBar;
        private CheckBox modifyNormalCheckBox;
        private RadioButton colorRadioButton;
        private RadioButton textureRadioButton;
        private Button startAnimationButton;
        private Button stopAnimationButton;
        private TrackBar lightZTrackBar;
        private PictureBox canvas;
        private Button loadTextureButton;
        private Button loadNormalMapButton;
        private Timer animationTimer;

        // Inne pola
        private Mesh mesh;
        private BezierSurface bezierSurface;
        private Triangulator triangulator;
        private Transformer transformer;
        public Renderer renderer;
        private LightingModel lightingModel;
        private TextureManager textureManager;
        private bool isAnimating = false;
        private float lightAngle = 0;

        // Konstruktor
        public MainForm()
        {
            // Inicjalizacja interfejsu użytkownika
            InitializeComponent();
            InitializeData();
        }

        private void InitializeComponent()
        {
            // Ustawienia okna
            this.Text = "Bezier Surface Visualizer";
            this.Width = 1000;
            this.Height = 800;

            // Inicjalizacja kontrolek
            canvas = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(800, 600),
                BorderStyle = BorderStyle.FixedSingle
            };
            canvas.Paint += Canvas_Paint;
            this.Controls.Add(canvas);

            // Suwak podziałów
            divisionTrackBar = new TrackBar
            {
                Location = new Point(820, 30),
                Minimum = 1,
                Maximum = 50,
                Value = 10,
                TickFrequency = 5,
                Orientation = Orientation.Horizontal,
                Width = 150
            };
            divisionTrackBar.ValueChanged += OnDivisionChanged;
            this.Controls.Add(divisionTrackBar);
            Label divisionLabel = new Label { Location = new Point(820, 10), Text = "Gęstość siatki" };
            this.Controls.Add(divisionLabel);

            // Suwak kąta alfa
            alphaTrackBar = new TrackBar
            {
                Location = new Point(820, 100),
                Minimum = -45,
                Maximum = 45,
                Value = 0,
                TickFrequency = 5,
                Orientation = Orientation.Horizontal,
                Width = 150
            };
            alphaTrackBar.ValueChanged += OnAlphaChanged;
            this.Controls.Add(alphaTrackBar);
            Label alphaLabel = new Label { Location = new Point(820, 80), Text = "Kąt alfa" };
            this.Controls.Add(alphaLabel);

            // Suwak kąta beta
            betaTrackBar = new TrackBar
            {
                Location = new Point(820, 170),
                Minimum = 0,
                Maximum = 10,
                Value = 0,
                TickFrequency = 1,
                Orientation = Orientation.Horizontal,
                Width = 150
            };
            betaTrackBar.ValueChanged += OnBetaChanged;
            this.Controls.Add(betaTrackBar);
            Label betaLabel = new Label { Location = new Point(820, 150), Text = "Kąt beta" };
            this.Controls.Add(betaLabel);

            // Suwaki kd, ks, m
            kdTrackBar = new TrackBar
            {
                Location = new Point(820, 240),
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                TickFrequency = 10,
                Orientation = Orientation.Horizontal,
                Width = 150
            };
            kdTrackBar.ValueChanged += OnKdChanged;
            this.Controls.Add(kdTrackBar);
            Label kdLabel = new Label { Location = new Point(820, 220), Text = "Współczynnik kd" };
            this.Controls.Add(kdLabel);

            ksTrackBar = new TrackBar
            {
                Location = new Point(820, 310),
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                TickFrequency = 10,
                Orientation = Orientation.Horizontal,
                Width = 150
            };
            ksTrackBar.ValueChanged += OnKsChanged;
            this.Controls.Add(ksTrackBar);
            Label ksLabel = new Label { Location = new Point(820, 290), Text = "Współczynnik ks" };
            this.Controls.Add(ksLabel);

            mTrackBar = new TrackBar
            {
                Location = new Point(820, 380),
                Minimum = 1,
                Maximum = 100,
                Value = 10,
                TickFrequency = 10,
                Orientation = Orientation.Horizontal,
                Width = 150
            };
            mTrackBar.ValueChanged += OnMChanged;
            this.Controls.Add(mTrackBar);
            Label mLabel = new Label { Location = new Point(820, 360), Text = "Współczynnik m" };
            this.Controls.Add(mLabel);

            // Checkbox modyfikacji wektora normalnego
            modifyNormalCheckBox = new CheckBox
            {
                Location = new Point(820, 450),
                Text = "Modyfikuj wektor normalny",
                Checked = false
            };
            modifyNormalCheckBox.CheckedChanged += OnModifyNormalChanged;
            this.Controls.Add(modifyNormalCheckBox);

            // Radiobuttons dla wyboru koloru lub tekstury
            colorRadioButton = new RadioButton
            {
                Location = new Point(820, 480),
                Text = "Stały kolor",
                Checked = true
            };
            colorRadioButton.CheckedChanged += OnColorOptionChanged;
            this.Controls.Add(colorRadioButton);

            textureRadioButton = new RadioButton
            {
                Location = new Point(820, 500),
                Text = "Tekstura"
            };
            textureRadioButton.CheckedChanged += OnColorOptionChanged;
            this.Controls.Add(textureRadioButton);

            // Przyciski wczytywania tekstury i mapy normalnych
            loadTextureButton = new Button
            {
                Location = new Point(820, 530),
                Text = "Wczytaj teksturę",
                Width = 150
            };
            loadTextureButton.Click += OnLoadTextureClicked;
            this.Controls.Add(loadTextureButton);

            loadNormalMapButton = new Button
            {
                Location = new Point(820, 560),
                Text = "Wczytaj mapę normalnych",
                Width = 150
            };
            loadNormalMapButton.Click += OnLoadNormalMapClicked;
            this.Controls.Add(loadNormalMapButton);

            // Suwak z dla animacji światła
            lightZTrackBar = new TrackBar
            {
                Location = new Point(820, 630),
                Minimum = -100,
                Maximum = 100,
                Value = 50,
                TickFrequency = 10,
                Orientation = Orientation.Horizontal,
                Width = 150
            };
            lightZTrackBar.ValueChanged += OnLightZChanged;
            this.Controls.Add(lightZTrackBar);
            Label lightZLabel = new Label { Location = new Point(820, 610), Text = "Poziom Z światła" };
            this.Controls.Add(lightZLabel);

            // Przyciski animacji
            startAnimationButton = new Button
            {
                Location = new Point(820, 670),
                Text = "Start animacji",
                Width = 70
            };
            startAnimationButton.Click += OnStartAnimationClicked;
            this.Controls.Add(startAnimationButton);

            stopAnimationButton = new Button
            {
                Location = new Point(900, 670),
                Text = "Stop animacji",
                Width = 70
            };
            stopAnimationButton.Click += OnStopAnimationClicked;
            this.Controls.Add(stopAnimationButton);

            // Timer animacji
            animationTimer = new Timer();
            animationTimer.Interval = 50; // co 50 ms
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void InitializeData()
        {
            // Wczytanie punktów kontrolnych z pliku
            Vector3[,] controlPoints = FileHelper.LoadControlPoints("control_points.txt");

            // Inicjalizacja powierzchni Beziera
            bezierSurface = new BezierSurface(controlPoints);

            // Inicjalizacja triangulatora
            triangulator = new Triangulator(divisionTrackBar.Value);

            // Inicjalizacja transformacji
            transformer = new Transformer(alphaTrackBar.Value, betaTrackBar.Value);

            // Inicjalizacja modelu oświetlenia
            lightingModel = new LightingModel
            {
                Kd = kdTrackBar.Value / 100f,
                Ks = ksTrackBar.Value / 100f,
                M = mTrackBar.Value
            };

            // Inicjalizacja managera tekstur
            textureManager = new TextureManager();

            // Inicjalizacja renderera
            renderer = new Renderer(textureManager, lightingModel, textureRadioButton.Checked, modifyNormalCheckBox.Checked);

            // Ustawienie opcji renderowania
            renderer.RenderFilled = true;    // TODO
            renderer.RenderWireframe = true;

            // Generowanie siatki
            mesh = triangulator.GenerateMesh(bezierSurface);

            // Transformacja wierzchołków
            foreach (var triangle in mesh.Triangles)
            {
                transformer.RotateVertex(triangle.Vertex1);
                transformer.RotateVertex(triangle.Vertex2);
                transformer.RotateVertex(triangle.Vertex3);
            }
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            renderer.RenderMesh(e.Graphics, mesh, canvas.Width, canvas.Height);
        }

        private void OnDivisionChanged(object sender, EventArgs e)
        {
            triangulator = new Triangulator(divisionTrackBar.Value);
            mesh = triangulator.GenerateMesh(bezierSurface);
            TransformAndRender();
        }

        private void OnAlphaChanged(object sender, EventArgs e)
        {
            transformer = new Transformer(alphaTrackBar.Value, betaTrackBar.Value);
            TransformAndRender();
        }

        private void OnBetaChanged(object sender, EventArgs e)
        {
            transformer = new Transformer(alphaTrackBar.Value, betaTrackBar.Value);
            TransformAndRender();
        }

        private void OnKdChanged(object sender, EventArgs e)
        {
            lightingModel.Kd = kdTrackBar.Value / 100f;
            canvas.Invalidate();
        }

        private void OnKsChanged(object sender, EventArgs e)
        {
            lightingModel.Ks = ksTrackBar.Value / 100f;
            canvas.Invalidate();
        }

        private void OnMChanged(object sender, EventArgs e)
        {
            lightingModel.M = mTrackBar.Value;
            canvas.Invalidate();
        }

        private void OnModifyNormalChanged(object sender, EventArgs e)
        {
            renderer.modifyNormal = modifyNormalCheckBox.Checked;
            canvas.Invalidate();
        }

        private void OnColorOptionChanged(object sender, EventArgs e)
        {
            renderer.useTexture = textureRadioButton.Checked;
            canvas.Invalidate();
        }

        private void OnLoadTextureClicked(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textureManager.LoadObjectTexture(openFileDialog.FileName);
                canvas.Invalidate();
            }
        }

        private void OnLoadNormalMapClicked(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textureManager.LoadNormalMap(openFileDialog.FileName);
                canvas.Invalidate();
            }
        }

        private void OnLightZChanged(object sender, EventArgs e)
        {
            lightingModel.LightPosition = new Vector3(lightingModel.LightPosition.X, lightingModel.LightPosition.Y, lightZTrackBar.Value);
            canvas.Invalidate();
        }

        private void OnStartAnimationClicked(object sender, EventArgs e)
        {
            if (!isAnimating)
            {
                animationTimer.Start();
                isAnimating = true;
            }
        }

        private void OnStopAnimationClicked(object sender, EventArgs e)
        {
            if (isAnimating)
            {
                animationTimer.Stop();
                isAnimating = false;
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            lightAngle += 0.1f;
            float radius = 100;
            lightingModel.LightPosition = new Vector3(
                radius * (float)Math.Cos(lightAngle),
                radius * (float)Math.Sin(lightAngle),
                lightZTrackBar.Value);

            canvas.Invalidate();
        }

        private void TransformAndRender()
        {
            // Transformacja wierzchołków
            foreach (var triangle in mesh.Triangles)
            {
                transformer.RotateVertex(triangle.Vertex1);
                transformer.RotateVertex(triangle.Vertex2);
                transformer.RotateVertex(triangle.Vertex3);
            }

            // Odświeżenie rysowania
            canvas.Invalidate();
        }
    }
}
