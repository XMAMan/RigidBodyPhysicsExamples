using RigidBodyPhysics.MathHelper;
using RigidBodyPhysics.RuntimeObjects.RigidBody;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gripper.Model
{
    class MoveableBody : IMoveable
    {
        private Shape uiElement;
        private IPublicRigidBody physicObject;
        private Vec2D centerOfMass;

        public MoveableBody(Shape uiElement, IPublicRigidBody physicObject, Vec2D localCenterOfMass)
        {
            this.uiElement = uiElement;
            this.physicObject = physicObject;    
            this.centerOfMass = localCenterOfMass; //points from (0,0) left top corner of a object to his center of Mass
        }

        public void Move()
        {
            TransformGroup transforms = new TransformGroup();
            transforms.Children.Add(new TranslateTransform(-this.centerOfMass.X, -this.centerOfMass.Y)); //Move center to origin
            transforms.Children.Add(new RotateTransform(this.physicObject.Angle / (float)Math.PI * 180)); //Rotate around center. Convert Angle in degrees

            transforms.Children.Add(new TranslateTransform(this.physicObject.Center.X, this.physicObject.Center.Y)); //Move origin to physic-center

            this.uiElement.RenderTransform = transforms;
        }
    }
}
