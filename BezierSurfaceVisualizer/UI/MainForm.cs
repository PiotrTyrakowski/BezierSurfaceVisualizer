﻿using System;
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
using System.IO;

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
        private RadioButton noFillRadioButton; //
        private Button startAnimationButton;
        private Button stopAnimationButton;
        private TrackBar lightZTrackBar;
        private PictureBox canvas;
        private Button loadTextureButton;
        private Button loadNormalMapButton;
        private Timer animationTimer;
        private CheckBox enableLightCheckBox;
        private CheckBox enableSpotlightsCheckBox;
        private NumericUpDown spotlightFocusNumericUpDown;


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

        public CheckBox wireframeCheckBox { get; private set; }
        public Button changeObjectColorButton { get; private set; }
        public Button changeLightColorButton { get; private set; }

        // Konstruktor
        public MainForm()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeComponent()
        {
            this.Text = "Bezier Surface Visualizer";
            this.Width = 1000;
            this.Height = 800;

            canvas = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(800, 600),
                BorderStyle = BorderStyle.FixedSingle
            };
            canvas.Paint += Canvas_Paint;
            this.Controls.Add(canvas);

            int controlX = 820;
            int controlY = 10;
            int controlWidth = 150;
            int controlHeight = 30;
            int controlSpacing = 65;
            int boxControlSpacing = 40;

            animationTimer = new Timer();
            animationTimer.Interval = 1; // co 50 ms
            animationTimer.Tick += AnimationTimer_Tick;

            // Suwak podziałów
            Label divisionLabel = new Label { Location = new Point(controlX, controlY), Text = "Gęstość siatki", Width = controlWidth };
            this.Controls.Add(divisionLabel);
            divisionTrackBar = new TrackBar
            {
                Location = new Point(controlX, controlY + 20),
                Minimum = 1,
                Maximum = 30,
                Value = 10,
                TickFrequency = 5,
                Orientation = Orientation.Horizontal,
                Width = controlWidth
            };
            divisionTrackBar.ValueChanged += OnDivisionChanged;
            this.Controls.Add(divisionTrackBar);
            controlY += controlSpacing;

            // Suwak kąta alfa
            Label alphaLabel = new Label { Location = new Point(controlX, controlY), Text = "Kąt alfa (45, 45)", Width = controlWidth };
            this.Controls.Add(alphaLabel);
            alphaTrackBar = new TrackBar
            {
                Location = new Point(controlX, controlY + 20),
                Minimum = -45,
                Maximum = 45,
                Value = 0,
                TickFrequency = 5,
                Orientation = Orientation.Horizontal,
                Width = controlWidth
            };
            alphaTrackBar.ValueChanged += OnAlphaChanged;
            this.Controls.Add(alphaTrackBar);
            controlY += controlSpacing;

            // Suwak kąta beta
            Label betaLabel = new Label { Location = new Point(controlX, controlY), Text = "Kąt beta (-45, 45)", Width = controlWidth };
            this.Controls.Add(betaLabel);
            betaTrackBar = new TrackBar
            {
                Location = new Point(controlX, controlY + 20),
                Minimum = -45-90,
                Maximum = 45-90,
                Value = -90,
                TickFrequency = 1,
                Orientation = Orientation.Horizontal,
                Width = controlWidth
            };
            betaTrackBar.ValueChanged += OnBetaChanged;
            this.Controls.Add(betaTrackBar);
            controlY += controlSpacing;

            // Suwaki kd, ks, m
            Label kdLabel = new Label { Location = new Point(controlX, controlY), Text = "Współczynnik kd", Width = controlWidth };
            this.Controls.Add(kdLabel);
            kdTrackBar = new TrackBar
            {
                Location = new Point(controlX, controlY + 20),
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                TickFrequency = 10,
                Orientation = Orientation.Horizontal,
                Width = controlWidth
            };
            kdTrackBar.ValueChanged += OnKdChanged;
            this.Controls.Add(kdTrackBar);
            controlY += controlSpacing;

            Label ksLabel = new Label { Location = new Point(controlX, controlY), Text = "Współczynnik ks", Width = controlWidth };
            this.Controls.Add(ksLabel);
            ksTrackBar = new TrackBar
            {
                Location = new Point(controlX, controlY + 20),
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                TickFrequency = 10,
                Orientation = Orientation.Horizontal,
                Width = controlWidth
            };
            ksTrackBar.ValueChanged += OnKsChanged;
            this.Controls.Add(ksTrackBar);
            controlY += controlSpacing;

            Label mLabel = new Label { Location = new Point(controlX, controlY), Text = "Współczynnik m", Width = controlWidth };
            this.Controls.Add(mLabel);
            mTrackBar = new TrackBar
            {
                Location = new Point(controlX, controlY + 20),
                Minimum = 1,
                Maximum = 100,
                Value = 10,
                TickFrequency = 10,
                Orientation = Orientation.Horizontal,
                Width = controlWidth
            };
            mTrackBar.ValueChanged += OnMChanged;
            this.Controls.Add(mTrackBar);
            controlY += controlSpacing;

            // Checkbox modyfikacji wektora normalnego
            modifyNormalCheckBox = new CheckBox
            {
                Location = new Point(controlX, controlY),
                Text = "Modyfikuj mape norm",
                Checked = false,
                Width = controlWidth
            };
            modifyNormalCheckBox.CheckedChanged += OnModifyNormalChanged;
            this.Controls.Add(modifyNormalCheckBox);
            controlY += controlHeight;

            // Checkbox rysowania krawędzi
            wireframeCheckBox = new CheckBox
            {
                Location = new Point(controlX, controlY),
                Text = "Rysuj krawędzie",
                Checked = true,
                Width = controlWidth
            };
            wireframeCheckBox.CheckedChanged += OnWireframeChanged;
            this.Controls.Add(wireframeCheckBox);
            controlY += controlHeight;

            // Radiobuttons dla wyboru koloru lub tekstury
            colorRadioButton = new RadioButton
            {
                Location = new Point(controlX, controlY),
                Text = "Stały kolor",
                Checked = true,
                Width = controlWidth
            };
            colorRadioButton.CheckedChanged += OnColorOptionChanged;
            this.Controls.Add(colorRadioButton);
            controlY += controlHeight;

            textureRadioButton = new RadioButton
            {
                Location = new Point(controlX, controlY),
                Text = "Tekstura",
                Width = controlWidth
            };
            textureRadioButton.CheckedChanged += OnColorOptionChanged;
            this.Controls.Add(textureRadioButton);
            controlY += controlHeight;

            noFillRadioButton = new RadioButton
            {
                Location = new Point(controlX, controlY),
                Text = "brak wypełnienia",
                Width = controlWidth
            };
            textureRadioButton.CheckedChanged += OnColorOptionChanged;
            this.Controls.Add(noFillRadioButton);
            controlY += controlHeight;



            // Przyciski zmiany koloru i wczytywania tekstur
            changeObjectColorButton = new Button
            {
                Location = new Point(controlX, controlY),
                Text = "Zmień kolor obiektu",
                Width = controlWidth
            };
            changeObjectColorButton.Click += OnChangeObjectColorClicked;
            this.Controls.Add(changeObjectColorButton);
            controlY += controlHeight;

            loadTextureButton = new Button
            {
                Location = new Point(controlX, controlY),
                Text = "Wczytaj teksturę",
                Width = controlWidth
            };
            loadTextureButton.Click += OnLoadTextureClicked;
            this.Controls.Add(loadTextureButton);
            controlY += controlHeight;

            changeLightColorButton = new Button
            {
                Location = new Point(controlX, controlY),
                Text = "Zmień kolor światła",
                Width = controlWidth
            };
            changeLightColorButton.Click += OnChangeLightColorClicked;
            this.Controls.Add(changeLightColorButton);
            controlY += controlHeight;

            loadNormalMapButton = new Button
            {
                Location = new Point(controlX, controlY),
                Text = "Wczytaj mapę normalnych",
                Width = controlWidth
            };
            loadNormalMapButton.Click += OnLoadNormalMapClicked;
            this.Controls.Add(loadNormalMapButton);
            controlY += controlHeight;

            // Suwak z dla animacji światła
            Label lightZLabel = new Label { Location = new Point(controlX, controlY), Text = "Poziom Z światła", Width = controlWidth };
            this.Controls.Add(lightZLabel);
            lightZTrackBar = new TrackBar
            {
                Location = new Point(controlX, controlY + 20),
                Minimum = -100,
                Maximum = 100,
                Value = 20,
                TickFrequency = 10,
                Orientation = Orientation.Horizontal,
                Width = controlWidth
            };
            lightZTrackBar.ValueChanged += OnLightZChanged;
            this.Controls.Add(lightZTrackBar);
            controlY += 65;

            // Przyciski animacji
            startAnimationButton = new Button
            {
                Location = new Point(controlX, controlY),
                Text = "Start animacji",
                Width = 70
            };
            startAnimationButton.Click += OnStartAnimationClicked;
            this.Controls.Add(startAnimationButton);

            stopAnimationButton = new Button
            {
                Location = new Point(controlX + 80, controlY),
                Text = "Stop animacji",
                Width = 70
            };
            stopAnimationButton.Click += OnStopAnimationClicked;
            this.Controls.Add(stopAnimationButton);

            enableLightCheckBox = new CheckBox
            {
                Location = new Point(controlX-800, controlY),
                Text = "Włącz światło",
                Checked = true,
                Width = controlWidth
            };
            enableLightCheckBox.CheckedChanged += OnEnableLightChanged;
            this.Controls.Add(enableLightCheckBox);

            enableSpotlightsCheckBox = new CheckBox
            {
                Location = new Point(controlX-600, controlY),
                Text = "Włącz reflektory",
                Checked = false,
                Width = controlWidth
            };
            enableSpotlightsCheckBox.CheckedChanged += OnEnableSpotlightsChanged;
            this.Controls.Add(enableSpotlightsCheckBox);
            

            Label spotlightFocusLabel = new Label
            {
                Location = new Point(controlX-400, controlY-30),
                Text = "Skupienie reflektorów",
                Width = controlWidth + controlWidth
            };
            this.Controls.Add(spotlightFocusLabel);

            spotlightFocusNumericUpDown = new NumericUpDown
            {
                Location = new Point(controlX-400, controlY),
                Minimum = 0,
                Maximum = 3,
                Value = 1,
                DecimalPlaces = 1,
                Increment = 0.1M,
                Width = 100
            };
            spotlightFocusNumericUpDown.ValueChanged += OnSpotlightFocusChanged;
            this.Controls.Add(spotlightFocusNumericUpDown);
            //controlY += controlSpacing;
        }




        private void InitializeData()
        {
            
            Vector3[,] controlPoints = FileHelper.LoadControlPoints("control_points.txt");

            bezierSurface = new BezierSurface(controlPoints);

            triangulator = new Triangulator(divisionTrackBar.Value);

            transformer = new Transformer(alphaTrackBar.Value, betaTrackBar.Value);

           
            lightingModel = new LightingModel
            {
                Kd = kdTrackBar.Value / 100f,
                Ks = ksTrackBar.Value / 100f,
                M = mTrackBar.Value
            };

            textureManager = new TextureManager();

            renderer = new Renderer(textureManager, lightingModel, textureRadioButton.Checked, modifyNormalCheckBox.Checked);

            renderer.RenderFilled = true;    
            renderer.RenderWireframe = true;

            mesh = triangulator.GenerateMesh(bezierSurface);

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

        
            DrawLightSymbol(e.Graphics);
        }

        private void DrawLightSymbol(Graphics graphics)
        {
         
            PointF lightPosition = ProjectToCanvas(lightingModel.LightPosition, canvas.Width, canvas.Height);

       
            float size = 10;
            graphics.FillEllipse(Brushes.Yellow, lightPosition.X - size / 2, lightPosition.Y - size / 2, size, size);
        }

        private PointF ProjectToCanvas(Vector3 point, int canvasWidth, int canvasHeight)
        {
            float x = point.X + canvasWidth / 2;
            float y = -point.Y + canvasHeight / 2; 

            return new PointF(x, y);
        }

        // new events
        private void OnEnableLightChanged(object sender, EventArgs e)
        {
            lightingModel.EnableLight = enableLightCheckBox.Checked;
            canvas.Invalidate();
        }

        private void OnEnableSpotlightsChanged(object sender, EventArgs e)
        {
            lightingModel.EnableSpotlights = enableSpotlightsCheckBox.Checked;
            canvas.Invalidate();
        }

        private void OnSpotlightFocusChanged(object sender, EventArgs e)
        {
            lightingModel.SpotlightFocus = (float)spotlightFocusNumericUpDown.Value;
            canvas.Invalidate();
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
            renderer.RenderFilled = !noFillRadioButton.Checked;
            canvas.Invalidate();
        }

        
        private void OnLoadTextureClicked(object sender, EventArgs e)
        {
            string filePathPoints = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            string projectPath = Path.GetFullPath(Path.Combine(filePathPoints, @"..\.."));
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = projectPath,
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All Files|*.*"
            };
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textureManager.LoadObjectTexture(openFileDialog.FileName);
                canvas.Invalidate();
            }
        }

        private void OnLoadNormalMapClicked(object sender, EventArgs e)
        {
            string filePathPoints = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            string projectPath = Path.GetFullPath(Path.Combine(filePathPoints, @"..\.."));
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = projectPath, 
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All Files|*.*" 
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textureManager.LoadNormalMap(openFileDialog.FileName);
                canvas.Invalidate();
            }
        }

        private void OnLightZChanged(object sender, EventArgs e)
        {
           
            lightingModel.SetLightPosition(new Vector3(lightingModel.LightPosition.X, lightingModel.LightPosition.Y, lightZTrackBar.Value));

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

        private void OnChangeObjectColorClicked(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    textureManager.ObjectColor = new Vector3(
                        colorDialog.Color.R / 255f,
                        colorDialog.Color.G / 255f,
                        colorDialog.Color.B / 255f);
                    canvas.Invalidate();
                }
            }
        }

        private void OnChangeLightColorClicked(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    lightingModel.LightColor = new Vector3(
                        colorDialog.Color.R / 255f,
                        colorDialog.Color.G / 255f,
                        colorDialog.Color.B / 255f);
                    canvas.Invalidate();
                }
            }
        }

        private void OnWireframeChanged(object sender, EventArgs e)
        {
            renderer.RenderWireframe = wireframeCheckBox.Checked;
            canvas.Invalidate();
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
            foreach (var triangle in mesh.Triangles)
            {
                transformer.RotateVertex(triangle.Vertex1);
                transformer.RotateVertex(triangle.Vertex2);
                transformer.RotateVertex(triangle.Vertex3);
            }

            canvas.Invalidate();
        }
    }
}
