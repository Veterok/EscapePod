/* Name: Zoolotac: Escape Pod
   Version: 1.1
   Author: Zool
   Based on the Relay Network & Debris automation for KSP. by JDP

   This code is free to use and modify as long as the original authors are credited.

Change Log
1.2:
*fixed* command pod decouple

1.1: 
Stopped deleting decoupler
Copy Staging right before decoupling instead of just on part start.

 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace Zoolotac
{

	class Stage
	{
		public string UID;
		public int originalStage;
		public int inStageIndex;
		public int manualStageOffset;
		public int inverseStage;
	}
		class StageList 
	{
		public List<Stage> list;
		 public void init()
		{
			list = new List<Stage>();

		}

	}
	class EscapePod : Part
	{
		VInfoBox thisIndicator;
		public int rotPower = 1;
		public int linPower = 1;
		public double Kp = 1.0;
		public double kp = 1.0;
		public float maxTorque = 5f;
		private bool disconnected = false;
		private bool applyDecpoupleForce = false;
		private float decoupleForce = 1f;
		private Vector3 vdecoupleForce;

		List<Stage>  stageList;
		List<Stage> stageList2;
		string namesuffix = "Pod";
//			private void OrganizeTree (Part root)
//		{
//			Vessel ship = root.vessel;
//			List<Part> NewTree = new List<Part> ();
//
//			OrganizeTreeRecursive (NewTree, root);
//			ship.parts.Clear ();
//			foreach (Part part in NewTree) {
//				print ("ADDED : "+part.name);
//				this.vessel.parts.Add (part);
//			}
//			print ("Tree Restructured");
//			}
//
//		private void OrganizeTreeRecursive (List<Part> Tree, Part ThisPart)
//		{
//			Tree.Add (ThisPart);
//			foreach (Part child in ThisPart.children) {
//				OrganizeTreeRecursive (Tree, child);
//			}
//
//		}
		private void rebuildTree (Part Root)
		{
			//vesselTransform(Root);
			print ("*Tree Rebuid*");
			if (Root.parent != null) {
				Root.addChild (Root.parent);
				rebuildTreeRecursive(Root.parent,Root);
					//Root.parent = null;
			}
			print ("*Tree Rebuilt*");

		}

		private void rebuildTreeRecursive (Part ThisPart, Part PrevPart)
		{
			if (ThisPart == this.vessel.rootPart) {
				PrevPart.removeChild (ThisPart);
				foreach (Part child in ThisPart.children) {
					ThisPart.removeChild (child);
				}
				ThisPart.addChild (this);
			} else {

				//print ("thispart : "+ ThisPart.name);
				if (ThisPart.potentialParent != null)
					print ("potential parent : " + ThisPart.potentialParent.name);

				ThisPart.children.Remove (PrevPart);
				if (ThisPart.parent != null) {
					//print ("parent : "+ThisPart.parent.name);
					ThisPart.addChild (ThisPart.parent);
					rebuildTreeRecursive (ThisPart.parent, ThisPart);
				}
				ThisPart.parent = PrevPart;
				if (ThisPart.parent == null) {
					//print (ThisPart.name + " parent should =  " + PrevPart);
					//print (ThisPart.name + " parent = " + ThisPart.parent);
				}
			}

		}


		private int fhighestStage ()
		{
			int highestStage = 0;
			int LowestStage = 99;
			foreach (Part p in this.vessel.parts) {
				if (p.manualStageOffset > highestStage)
					highestStage = p.manualStageOffset;
				if (p.manualStageOffset < LowestStage)
					LowestStage = p.manualStageOffset;
			}
			return highestStage;
		}
		private int fLowestStage ()
		{
			int highestStage = 0;
			int LowestStage = 99;
			foreach (Part p in this.vessel.parts) {
				if (p.manualStageOffset > highestStage) highestStage = p.manualStageOffset;
				if (p.manualStageOffset < LowestStage) LowestStage = p.manualStageOffset;

			}
		return LowestStage;
		}

		 public void ShowIndicator()
    {

            thisIndicator = this.stackIcon.DisplayInfo();
            thisIndicator.SetMessage("SAS force");
            thisIndicator.SetMsgBgColor(XKCDColors.DarkAqua );
            thisIndicator.SetMsgTextColor(XKCDColors.BrightCyan);
            thisIndicator.SetProgressBarBgColor(XKCDColors.DarkAqua );
            thisIndicator.SetProgressBarColor(XKCDColors.BrightCyan);
           
    }
		public void HideIndicator ()
		{
			this.stackIcon.RemoveInfo(thisIndicator);
			thisIndicator = null;
		}

		protected override void onCtrlUpd (FlightCtrlState s)
		{

			if (this.vessel.rootPart is CommandPod) {
				base.onCtrlUpd (s);
			}
			else {
				this.stackIcon.SetIconColor(Color.white);
				this.vessel.rigidbody.AddRelativeTorque (s.pitch * -rotPower, s.roll * -rotPower, s.yaw * -rotPower);
				if (FlightInputHandler.state.killRot) {
					Vector3 torque;
					Vector3 vel = -this.vessel.angularVelocity;
					torque = new Vector3 (Mathf.Clamp (vel.x, -this.maxTorque, this.maxTorque), Mathf.Clamp (vel.y, -this.maxTorque, this.maxTorque), Mathf.Clamp (vel.z, -this.maxTorque, this.maxTorque));
					base.Rigidbody.AddRelativeTorque (torque, ForceMode.Force);

					if (thisIndicator == null)
						ShowIndicator ();
					thisIndicator.SetValue (Mathf.Max (Mathf.Abs (torque.x), Mathf.Abs (torque.y), Mathf.Abs (torque.z)) / maxTorque);
				} else 
				if (thisIndicator != null)
					HideIndicator ();
			}
		}
		 protected override void onPartStart()
        {
            base.onPartStart();
            this.stackIcon.SetIconColor(Color.grey);
          	this.stackIcon.SetIcon(DefaultIcons.COMMAND_POD);


        }
		 protected override void onPartFixedUpdate ()
		{
			base.onPartFixedUpdate ();

			if (this.isConnected) {
				if (this.vessel.rootPart is Decoupler || this.vessel.rootPart is RadialDecoupler || this.vessel.rootPart is DecouplerGUI) {
					//this.vessel.rootPart.force_activate();
					//force rebuild?
//					foreach(Part child in this.vessel.rootPart.children)
//						this.vessel.rootPart.removeChild(child);
//					this.vessel.rootPart.addChild(this);
//					this.parent = this.vessel.rootPart;		
//					//
					//rebuildTree(this);
					//Part x;
					//x = this.vessel.rootPart;
					//float decoupleForce = 1f;
					this.vessel.orbit.objectType = Orbit.ObjectType.SPACE_DEBRIS;
					this.vessel.vesselName = this.vessel.vesselName.Substring(0,vessel.vesselName.Length - namesuffix.Length ) + " Debris";

					if(this.vessel.rootPart.GetType().ToString() == "ErkleWarpClamp")
					print ("TYPE: "+this.vessel.rootPart.GetType().ToString());
					else
					this.vessel.rootPart.children[0].decouple(0f);

					applyDecpoupleForce =true;
					vdecoupleForce = new Vector3(0,0,0);
					vdecoupleForce = (base.transform.up * -decoupleForce);
					//base.rigidbody.AddForce (base.transform.up *

					//x.rigidbody.AddForce(base.transform.up * decoupleForce * 0.5f, ForceMode.Force);
					//this.vessel.rootPart.Die ();
//					this.vessel.transform.up = this.transform.up;
//					this.vessel.transform.forward =this.transform.forward;
//					this.vessel.transform.right = this.transform.right;
				}
				else{
					if (applyDecpoupleForce){
						base.rigidbody.AddForce(vdecoupleForce, ForceMode.Force);
						applyDecpoupleForce =false;
					}
				}

			}

			if (this.vessel.orbit.objectType != Orbit.ObjectType.VESSEL) {

				this.vessel.orbit.objectType = Orbit.ObjectType.VESSEL;
				this.vessel.ClearStaging ();

				if (vessel.vesselName.EndsWith (" Debris") && !vessel.vesselName.EndsWith (" " + namesuffix)) {
					vessel.vesselName = vessel.vesselName.Substring (0, vessel.vesselName.Length - 7);
					if (!vessel.vesselName.EndsWith (" " + namesuffix)) {
						vessel.vesselName = vessel.vesselName + " " + namesuffix;
					}
				}

			}


			if (disconnected) {
				disconnected = false; 
				int highestStage = 0;
				int LowestStage = 99;
				this.stackIcon.SetIconColor (Color.white);
				stageList2 = new List<Stage> ();
				List<bool> MissingStageList = new List<bool> ();

				foreach (Part p in this.vessel.parts) {

					p.isConnected = true;
					foreach (Stage s in stageList) {
						if (s.UID == p.UID) {
							p.manualStageOffset = s.manualStageOffset;
							p.inverseStage = s.inverseStage;
						}

					}
				}
				highestStage = fhighestStage ();
				LowestStage = fLowestStage ();

				for (int i = 0; i<=highestStage; i++) {
					MissingStageList.Add (false); //false is missing 
				}
				foreach (Part p in this.vessel.parts) {
					if (MissingStageList [p.manualStageOffset] == false)
						MissingStageList [p.manualStageOffset] = true;
				}

				foreach (Part p in this.vessel.parts) {
					int adjustDown = 0;

					for (int i = 0; i<=p.manualStageOffset; i++) {
						if (MissingStageList [i] == false)
							adjustDown += 1;
					}
		
					p.manualStageOffset = p.manualStageOffset - adjustDown;
					p.inverseStage = p.manualStageOffset;
				}
				highestStage = fhighestStage ();
				LowestStage = fLowestStage ();

				this.vessel.currentStage = highestStage+1;
				MakeStageList ();
				this.vessel.FeedInputFeed (new FlightCtrlState ());
			}
			if (!this.isConnected) {
				disconnected = true; //wait a frame for all fuel lines and struts to disconnect
			}
		}

		protected override void onDisconnect ()
		{
			MakeStageList();
			base.onDisconnect ();
		}

		protected void MakeStageList ()
		{
			stageList = new List<Stage> ();
			foreach (Part p in this.vessel.parts) {
				Stage s = new Stage();
				s.UID = p.UID;
				s.inStageIndex = p.inStageIndex;
				s.originalStage = p.originalStage;
				s.manualStageOffset =p.manualStageOffset;
				s.inverseStage = p.inverseStage;
				stageList.Add (s);
			}

		}
		 protected override void onFlightStart ()
		{
			//MakeStageList ();
            base.onFlightStart();

        }

	}
 


}