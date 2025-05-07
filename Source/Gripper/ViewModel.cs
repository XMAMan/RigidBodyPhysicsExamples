using GameHelper.Simulation;
using Gripper.Model;
using PhysicGlobal;
using RigidBodyPhysics.RuntimeObjects.Joints;
using RigidBodyPhysics.RuntimeObjects.RigidBody;
using System.Windows.Media;

namespace Gripper
{
    class ViewModel
    {
        private const string DataFolder = @"..\..\..\..\..\Data\";

        private GameSimulator simulator;                            //Comes from XMAMan.PhysicEngine
        private System.Windows.Threading.DispatcherTimer timer;
        private System.Windows.Controls.Canvas canvas;
        private List<IMoveable> moveables = new List<IMoveable>();

        public ViewModel(System.Windows.Controls.Canvas canvas)
        {
            this.canvas = canvas;

            //This is needet to update the camera position/zoom when the window is resized
            this.canvas.SizeChanged += (s, e) =>
            {
                this.simulator?.PanelSizeChangedHandler((int)canvas.ActualWidth, (int)canvas.ActualHeight);
            };

            this.timer = new System.Windows.Threading.DispatcherTimer();
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 30);//30 ms
            this.timer.Tick += Timer_Tick;

            Load();

            this.timer.Start();
        }

        private void Load()
        {
            this.simulator = new GameSimulator(DataFolder + "Gripper.txt", (float)this.timer.Interval.TotalMilliseconds);

            foreach (var physicRec in this.simulator.GetAllBodiesOfType<IPublicRigidRectangle>())
            {
                System.Windows.Shapes.Rectangle uiRect = new System.Windows.Shapes.Rectangle();
                uiRect.Width = physicRec.Size.X;
                uiRect.Height = physicRec.Size.Y;
                uiRect.Fill = System.Windows.Media.Brushes.Red;
                uiRect.Stroke = System.Windows.Media.Brushes.Black;

                this.canvas.Children.Add(uiRect);

                this.moveables.Add(new MoveableBody(uiRect, physicRec, new Vec2D(physicRec.Size.X / 2, physicRec.Size.Y / 2)));
            }

            foreach (var physicCircle in this.simulator.GetAllBodiesOfType<IPublicRigidCircle>())
            {
                System.Windows.Shapes.Ellipse uiCircle = new System.Windows.Shapes.Ellipse();
                uiCircle.Width = physicCircle.Radius * 2;
                uiCircle.Height = physicCircle.Radius * 2;
                uiCircle.Fill = System.Windows.Media.Brushes.Blue;
                uiCircle.Stroke = System.Windows.Media.Brushes.Black;

                this.canvas.Children.Add(uiCircle);

                this.moveables.Add(new MoveableBody(uiCircle, physicCircle, new Vec2D(physicCircle.Radius, physicCircle.Radius)));
            }

            foreach (var physicPolygon in this.simulator.GetAllBodiesOfType<IPublicRigidPolygon>())
            {
                System.Windows.Shapes.Polygon uiPolygon = new System.Windows.Shapes.Polygon();

                float minX = physicPolygon.Vertex.Min(x => x.X);
                float minY = physicPolygon.Vertex.Min(x => x.Y);

                //Point (0,0) is the left top corner from the polygon
                foreach (var point in physicPolygon.Vertex)
                {
                    uiPolygon.Points.Add(new System.Windows.Point(point.X - minX, point.Y - minY));
                }
                uiPolygon.Fill = System.Windows.Media.Brushes.Green;
                uiPolygon.Stroke = System.Windows.Media.Brushes.Black;

                this.canvas.Children.Add(uiPolygon);

                this.moveables.Add(new MoveableBody(uiPolygon, physicPolygon, physicPolygon.Center - new Vec2D(minX, minY)));
            }

            foreach (var distanceJoint in this.simulator.GetAllJointsOfType<IPublicDistanceJoint>())
            {
                System.Windows.Shapes.Line uiLine = new System.Windows.Shapes.Line();
                uiLine.Stroke = System.Windows.Media.Brushes.Black;
                uiLine.StrokeThickness = 2;

                this.canvas.Children.Add(uiLine);
                this.moveables.Add(new MoveableDistanceJoint(uiLine, distanceJoint));
            }
        }

        private System.Windows.Media.Transform GetCameraTransform()
        {
            TransformGroup transforms = new TransformGroup();
            var cameraPosition = this.simulator.GetCameraPosition();
            float cameraScale = this.simulator.GetCameraScaleFactor();
            transforms.Children.Add(new TranslateTransform(-cameraPosition.X, -cameraPosition.Y)); //Move center to origin
            transforms.Children.Add(new System.Windows.Media.ScaleTransform(cameraScale, cameraScale));
            return transforms;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            //Move all objects
            this.simulator.MoveOneStep((float)this.timer.Interval.TotalMilliseconds);

            //Update objectpositions from UI-Elements
            foreach (var moveable in this.moveables)
            {
                moveable.Move();
            }

            //Update camera-Position from Canvas
            this.canvas.RenderTransform = GetCameraTransform();
        }

        public void HandleKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat) return; //So verhindere ich, dass bei gedrückter Taste der Handler mehrmals aufgerufen wird

            this.simulator.HandleKeyDown(e.Key);
        }

        public void HandleKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat) return;

            this.simulator.HandleKeyUp(e.Key);
        }
    }
}
