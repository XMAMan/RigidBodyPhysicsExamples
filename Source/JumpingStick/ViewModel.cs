using GameHelper.Simulation;
using RigidBodyPhysics.CollisionDetection;
using RigidBodyPhysics;
using PhysicGlobal;

namespace JumpingStick
{
    class ViewModel
    {
        private const string DataFolder = @"..\..\..\..\..\Data\";

        private IDrawingPanel panel;                                //Comes from XMAMan.PhysicEngine.GrxExtension (graphical output)        

        private GameSimulator simulator;                            //Comes from XMAMan.PhysicEngine (physic simulation)
        private System.Windows.Threading.DispatcherTimer timer;
        private bool stickIsTouchingTheFigure = false;

        public ViewModel(DrawingPanel.DrawingPanel panel)
        {
            this.panel = panel;
            panel.SizeChanged += Panel_SizeChanged;

            this.timer = new System.Windows.Threading.DispatcherTimer();
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 30);//30 ms
            this.timer.Tick += Timer_Tick;

            this.simulator = new GameSimulator(DataFolder + "StickPlant.txt", (float)this.timer.Interval.TotalMilliseconds);

            this.simulator.CollisonOccured += Simulator_CollisonOccured;

            this.timer.Start();            
        }

        private void Simulator_CollisonOccured(PhysicScene sender, PublicRigidBodyCollision[] collisions)
        {
            foreach (var collision in collisions)
            {
                //Way 1 for collisionhandling: Use the tagColor
                byte color1 = this.simulator.GetTagDataFromBody(collision.Body1).Color;
                byte color2 = this.simulator.GetTagDataFromBody(collision.Body2).Color;

                //Color-Values:
                //0 = Ground
                //1 = Stick
                //2 = Stoneman

                bool stickIsTouchingGround1 = (color1 == 0 && color2 == 1) || (color1 == 1 && color2 == 0);
                this.stickIsTouchingTheFigure = (color1 == 1 && color2 == 2) || (color1 == 2 && color2 == 1);


                //Way 2 for collisionhandling: Use the tagName
                string name1 = this.simulator.GetTagDataFromBody(collision.Body1).Names.FirstOrDefault();
                string name2 = this.simulator.GetTagDataFromBody(collision.Body2).Names.FirstOrDefault();

                bool stickIsTouchingGround2 = (name1 == "ground" && name2 == "stick") || (name1 == "stick" && name2 == "ground");
            }
        }

        private void Panel_SizeChanged(object? sender, EventArgs e)
        {
            this.simulator?.PanelSizeChangedHandler(panel.Width, panel.Height);
            Refresh();
        }

        private void Refresh()
        {
            this.simulator.Draw(this.panel);
            this.panel.SetTransformationMatrixToIdentity();
            this.panel.DrawString(30, 10, Color.Black, 20, "Press UP/DOWN to move the stick");
            if (this.stickIsTouchingTheFigure)
            {
                this.panel.DrawString(30, 40, Color.Red, 20, "Stick is touching the figure");
            }
            this.panel.FlipBuffer();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            this.simulator.MoveOneStep((float)this.timer.Interval.TotalMilliseconds);

            Refresh();
        }

        public void HandleKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat) return; //This prevents the handler from being called multiple times when the key is pressed

            this.simulator.HandleKeyDown(e.Key);
        }

        public void HandleKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat) return;

            this.simulator.HandleKeyUp(e.Key);
        }
    }
}
