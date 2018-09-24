using Microsoft.Xna.Framework;


namespace Nez.Farseer
{
	public class FSDistanceJoint : FSJoint
	{
		FSDistanceJointDef _jointDef = new FSDistanceJointDef();


		#region Configuration

		public FSDistanceJoint setFrequency( float frequency )
		{
			_jointDef.frequency = frequency;
			recreateJoint();
			return this;
		}


		public FSDistanceJoint setDampingRatio( float damping )
		{
			_jointDef.dampingRatio = damping;
			recreateJoint();
			return this;
		}


		public FSDistanceJoint setOwnerBodyAnchor( vec2 ownerBodyAnchor )
		{
			_jointDef.ownerBodyAnchor = ownerBodyAnchor;
			recreateJoint();
			return this;
		}


		public FSDistanceJoint setOtherBodyAnchor( vec2 otherBodyAnchor )
		{
			_jointDef.otherBodyAnchor = otherBodyAnchor;
			recreateJoint();
			return this;
		}

		#endregion


		internal override FSJointDef getJointDef()
		{
			initializeJointDef( _jointDef );
			if( _jointDef.bodyA == null || _jointDef.bodyB == null )
				return null;

			return _jointDef;
		}

	}
}
