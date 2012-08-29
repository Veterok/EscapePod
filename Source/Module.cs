using System;
using UnityEngine;
namespace Zoolotac{
	class KillVessel : ZoolotacPart{
protected override bool onPartActivate ()
		{
			this.vessel.Unload();
			return base.onPartActivate ();
		}

		protected override void onActiveFixedUpdate ()
		{
			base.onActiveFixedUpdate ();
		}

	}
	class balloon : Parachutes{
		//1kg hydrogen
		public float STPDensity = .08999f;
		public float Pressure = 0f;
		public float gasTemp = 0f;
		public float moles = 0f;
		public float gasMass = 0f;
		public float volume = 0f;
		public float strechyness =1.01f;
		private Transform thecanopy;

		protected override void onPartFixedUpdate ()
		{
			base.onPartFixedUpdate ();
			//apply force
			
       
			base.rigidbody.AddForce(bouyantForce());
			thecanopy.rotation  = Quaternion.LookRotation(Vector3.Normalize((base.rigidbody.velocity.normalized*base.vessel.rigidbody.drag) - bouyantForce()),base.transform.forward);

			//apply parachute rotation

		}

		protected override void onPartLoad ()
		{
			thecanopy = this.FindModelTransform("canopy");
		
		}


		private Vector3 bouyantForce(){
			float density = (float)vessel.atmDensity;
			return -FlightGlobals.getGeeForceAtPosition(vessel.findWorldCenterOfMass()) * (density * volume -(gasMass+.3));
			//mass of displaced gas*volume of baloon - weight of balloon+gas

		}

		private void BalloonVolume ()
		{
			//PV=nRT
			//R=.082atm*L/mole*K
			//mass of gas * density
			float R = .082f;
			gasMass = 10*STPDensity;
			moles = gasMass*496;
			Pressure = (float)this.staticPressureAtm * strechyness;
			volume = (moles*R*(273+gasTemp))/Pressure;
		}
	
	}
}