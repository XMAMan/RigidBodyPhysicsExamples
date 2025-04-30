using RigidBodyPhysics.RuntimeObjects.Joints;
using System.Windows.Shapes;

namespace Gripper.Model
{
    class MoveableDistanceJoint : IMoveable
    {
        private Line uiLine;
        private IPublicDistanceJoint distanceJoint;

        public MoveableDistanceJoint(Line uiLine, IPublicDistanceJoint distanceJoint)
        {
            this.uiLine = uiLine;
            this.distanceJoint = distanceJoint;
        }

        public void Move()
        {
            this.uiLine.X1 = this.distanceJoint.Anchor1.X;
            this.uiLine.Y1 = this.distanceJoint.Anchor1.Y;
            this.uiLine.X2 = this.distanceJoint.Anchor2.X;
            this.uiLine.Y2 = this.distanceJoint.Anchor2.Y;
        }
    }
}
