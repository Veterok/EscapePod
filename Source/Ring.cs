using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Zoolotac
{

	public class DockingRing : Part
	{
		private bool debugon = true;
		public List<Vessel> NearVessels;
		public List<DockingRing> NearDockingRings;

		public enum DOCKMODE {DOCKING,CLOSED,DOCKED};
		public DOCKMODE DockingMode = DOCKMODE.CLOSED; 
		//private AttachNode docking_node;
		public AttachNode Docking_Node {
			get {
			AttachNode[] nodes1;
			nodes1 = this.findAttachNodes("node_stack");
				foreach(AttachNode node in nodes1){
					if(node.attachedPart ==null) return node;
					else return null;
				}
				return null;

			}
		}
			protected override void onPartStart ()
		{
			base.onPartStart ();
			//this.stackIcon.SetIconColor(Color.grey);
			this.stackIcon.SetIcon(DefaultIcons.DECOUPLER_HOR);
		}

		protected override void onPartFixedUpdate ()
		{
			deltaT+= UnityEngine.Time.deltaTime;
			base.onPartFixedUpdate ();
			if (this.vessel.isActiveVessel) {
				switch (DockingMode) {
				case DOCKMODE.CLOSED:
					break;
				case DOCKMODE.DOCKED:
					break;
				case DOCKMODE.DOCKING:
					UpdateDock();
					break;
				}
			}
		}

		private void UpdateDock()
		{
			NearVessels = NearbyVessels ();
			if (NearVessels != null) {
				NearDockingRings  = DockingRings(NearVessels);
				if (NearDockingRings != null){
					foreach(DockingRing Ring in NearDockingRings)
					{
						tryToDock (this, Ring);
					}

				}
			
			}

		}

		private void tryToDock (DockingRing ThisOne, DockingRing ThatOne)
		{
			Vector3 ThisPosition = ThisOne.transform.position + ThisOne.vessel.transform.position + ThisOne.transform.position + ThisOne.Docking_Node.position;
			Vector3 ThatPosition = ThatOne.transform.position + ThatOne.vessel.transform.position + ThatOne.transform.position + ThatOne.Docking_Node.position;
			double DockingDistance = Mathf.Min (ThisOne.Docking_Node.radius, ThatOne.Docking_Node.radius);
			Vector3 Offset = ThisPosition - ThatPosition;
			float angle = Vector3.Angle (ThisOne.transform.up+ThisOne.vessel.transform.up,ThatOne.transform.up +ThatOne.vessel.transform.up);
			debugprint ("###Docking###");
			debugprint ("Angle  = " + angle.ToString ());
			debugprint ("Offset = " + Offset.magnitude.ToString ());
			if (Mathf.Abs (Offset.magnitude) < DockingDistance) {
				debugprint ("DOCK!");
				ThisOne.DockingMode = DOCKMODE.DOCKED;
				ThatOne.DockingMode = DOCKMODE.DOCKED;
		
				ThisOne.vessel.Translate (Offset);
				foreach (Part part in ThisOne.vessel.parts) {
					ThatOne.vessel.parts.Add (part);
					part.vessel = ThatOne.vessel;
					part.transform.position += ThisPosition - ThatPosition;
					part.transform.up = Vector3.Project (ThisOne.vessel.transform.up, ThatOne.vessel.transform.up); 
				}
				ThisOne.parent = ThatOne;
				if (!ThatOne.children.Contains (ThisOne))
					ThatOne.addChild (ThisOne);
				ThisOne.Docking_Node.attachMethod = AttachNodeMethod.FIXED_JOINT;
				ThatOne.Docking_Node.attachMethod = AttachNodeMethod.FIXED_JOINT;
				ThisOne.Docking_Node.attachedPart = ThatOne;
				ThatOne.Docking_Node.attachedPart = ThisOne;

	
			}
		}



		private List<Vessel> NearbyVessels ()
		{
			List<Vessel> nearbyvessels = new List<Vessel>();
			foreach (Vessel vessel in FlightGlobals.Vessels) {
				if(vessel.loaded && vessel != this.vessel) nearbyvessels.Add(vessel);
			}
			return nearbyvessels;

		}

		private List<DockingRing> DockingRings (List<Vessel> VesselList)
		{
			List<DockingRing> nearbydockingrings = new List<DockingRing> ();
			foreach (Vessel vessel in VesselList) {
			foreach (Part part in vessel.parts)
				{
					if (part is DockingRing)nearbydockingrings.Add ((DockingRing)part); 
				}
			}
			return nearbydockingrings;
		}

		private List<string> debugList;
		private float deltaT = 0f;
		private float debugTime = 1f;

		private void debugprint (string line)
		{
			if (debugon && this.vessel.isActiveVessel) {
				if (debugList == null)
					debugList = new List<string> ();
				debugList.Add (line);
				if (deltaT > debugTime) {
					foreach (string l in debugList) {
						print (l);
					}
					deltaT = 0;
					debugList.Clear ();
				}


			}
		}


	}
}

