using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
namespace Zoolotac
{
	public class ZoolotacPart : Part
	{
		private List<string> debugList;
		private float deltaT = 0f;
		private float debugTime = 1f;
		public bool debugon = true;
		private Part oldRoot;
		protected override void onPartFixedUpdate ()
		{
			//deltaT+= UnityEngine.Time.deltaTime;
			base.onPartFixedUpdate();
		}
		public void debugprint (string line)
		{
			if (debugon ){//&& this.vessel.isActiveVessel) {
				if (debugList == null){
					print ("null");
					debugList = new List<string> ();
				}
				debugList.Add (line);
				//debugTxt(line);
				printdebugs();
			}
		}
		public void printdebugs ()
		{
					if (true) {
					//print ("for each");
					foreach (string l in debugList) {
						print (l);
					}
					deltaT = 0;
					debugList.Clear ();
				}

		}
		protected void rebuildComponentTree (Part Root)
		{
			Part[] PartArray;

			PartArray = vessel.gameObject.GetComponents<Part>();
			//vessel.gameObject.GetComponent().
			foreach (Part part in PartArray)
				print("part : "+part.transform.GetInstanceID().ToString());


		}


			protected void rebuildTree (Part Root)
		{
			//vesselTransform(Root);
			debugprint ("*Tree Rebuid*");
			//vesselInfo ();
			//debugprint("Vessel Position1 : "+this.vessel.transform.position);
			Vector3 Offset = -(this.vessel.rootPart.transform.position - this.transform.position);
			//this.transform.position+=Offset;
			if (Root.parent != null) {
				Root.addChild (Root.parent);
				rebuildTreeRecursive (Root.parent, Root, Offset);
				//Root.parent = null;
			}
			this.transform.parent = null;
			//this.vessel.parts.
			print ("*Tree Rebuilt*");

			OrganizeTree (Root);
			//vesselInfo ();
			//
			//this.transform.position += Offset;
			//this.vessel.transform.Translate(-Offset);
			print ("Vessel Transform Child Count : " + vessel.transform.childCount);
			print ("vessel.translate");
			//int i = 0;


			//RebootVessel();

			//this.vessel.transform 
//			debugprint ("Reflection Voodoo");
//			FieldInfo[] myFieldInfo;
//			Type myType = typeof(Vessel);
//			myFieldInfo = myType.GetFields (BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
//			for (int i= 0; i < myFieldInfo.Length; i++) {
//			print("Name            : "+myFieldInfo[i].Name);
				//if(myFieldInfo[i].FieldType !=null)
				//	print("Field :       "+myFieldInfo[i].FieldType.ToString());
				//print("public : "+myFieldInfo[i].IsPublic.ToString());  
//				if(myFieldInfo[i].GetValue(vessel) != null)
//			print("Value          : "+myFieldInfo[i].GetValue(vessel).ToString()) ;
//			
//			}

			//FieldInfo field = typeof(Vessel).GetField("transform",BindingFlags.Instance|BindingFlags.NonPublic);
			//field.SetValue(Root.vessel, Root.transform);
			//debugprint(field.GetType().ToString()); 
			//debugprint(field.ReflectedType.GetType().ToString());
//			debugprint ("that U do");
//			print(vessel.transform.gameObject.name);
//			vessel.transform.gameObject.
//			vesselInfo ();
		//this.vessel.transform

			//this.vessel.transform.position += Offset;
		

		}



		private void rebuildTreeRecursive (Part ThisPart, Part PrevPart, Vector3 Offset)
		{
			//List<Part> partlist = new List<Part>();

			//print ("thispart : "+ ThisPart.name);
			if (ThisPart.potentialParent != null)
				debugprint ("potential parent : " + ThisPart.potentialParent.name);

			ThisPart.children.Remove (PrevPart);
			//ThisPart.transform.position = ThisPart.transform.position + Offset;
			if (ThisPart.parent != null) {
				ThisPart.addChild (ThisPart.parent);
				rebuildTreeRecursive (ThisPart.parent, ThisPart,Offset);
			}
			if (ThisPart.parent == null) {
				print ("end of tree null parent");
			}
			ThisPart.parent = PrevPart;


		}

		private void OrganizeTree (Part root)
		{
			Vessel ship = root.vessel;
			List<Part> NewTree = new List<Part> ();

			OrganizeTreeRecursive (NewTree, root);
			ship.parts.Clear ();
			//Transform x = root.transform;
			//Transform y = x;
			//ship. = root.transform;
			//ship.transform.DetachChildren();
			//ship.transform = root.transform;

			foreach (Part part in NewTree) {
				print ("ADDED : "+part.name);
				ship.parts.Add (part);
				//if (part != root) part.transform.parent = ship.transform;

			}
			print ("Tree Restructured");
			}
		private void OrganizeTreeRecursive (List<Part> Tree, Part ThisPart)
		{
			Tree.Add (ThisPart);
			foreach (Part child in ThisPart.children) {
				OrganizeTreeRecursive (Tree, child);
			}

		}

		public void printTransform (Vessel ship)
		{

			foreach(Part part in ship.parts){
				debugprint (" Part  : "+part.name);

			}
		}
		public void vesselInfo (string message ="")
		{
			if (message != "")
				debugprint (message);
			debugprint ("Vessel : " + this.vessel.vesselName);
			debugprint ("Alt : " + this.vessel.altitude);
			debugprint ("Lat : " + this.vessel.latitude);
			debugprint ("Long : " + this.vessel.longitude);
			debugprint ("x : " + this.vessel.transform.position.x);
			debugprint ("y : " + this.vessel.transform.position.y);
			debugprint ("z : " + this.vessel.transform.position.z);
			debugprint ("Transform ID : " + this.vessel.transform.GetInstanceID ().ToString ());
			TransformChilds(this.vessel.transform);
			if (vessel.transform.parent != null && vessel.transform.parent.name != null)
			debugprint("transform parent: "+vessel.transform.parent.name);
			if(vessel.landedAt !=null)
			debugprint("Landed At : "+vessel.landedAt);
			debugprint ("GameOb ID : "+vessel.gameObject.GetInstanceID().ToString());
			foreach (Part part in this.vessel.parts) {
				debugprint (" Part  : "+part.name);
				debugprint (part.State.ToString());
				debugprint ("GameOb ID : "+part.gameObject.GetInstanceID().ToString());
				debugprint ("Transform ID : "+part.transform.GetInstanceID().ToString());
				if (part.transform.parent != null)
				debugprint("transform parent: "+part.transform.parent.GetInstanceID().ToString());
				debugprint ("x : "+part.transform.position.x.ToString());
				debugprint ("y : "+part.transform.position.y.ToString());
				debugprint ("z : "+part.transform.position.z.ToString());

				foreach(AttachNode node in part.attachNodes){
					if(node.id != null)
					debugprint ("  Node : "+ node.id);
					if(node.attachedPart != null)
					debugprint ("   Attached to : "+node.attachedPart.name);
					if(node.attachedPartId  != null)
					debugprint ("   Attached to : "+ node.attachedPartId);
					//if(node.attachMethod  != null)
					debugprint ("   With : "+node.attachMethod.ToString());

				}
			}
		}
		private void TransformChilds (Transform input, string tabs = "")
		{
			for (int i =0; i<input.childCount; i++) {
				if (input.childCount == i)
					break;
				if (input.GetChild (i).name != null)
					debugprint (tabs+"Child: " + input.GetChild (i).name);
					debugprint (tabs+"ChildID : " + input.GetChild (i).GetInstanceID ().ToString ());
				TransformChilds (input.GetChild(i),tabs+"|");
			}


		}
		private void RebootVessel(){
			ProtoVessel x;
			//Vessel savedVessel = new Vessel();
		    x = vessel.BackupVessel();
		//	vessel.Unload();
			vessel.StartFromBackup(x);
			//vessel.Load();

//			savedVessel.acceleration = vessel.acceleration;
//			savedVessel.altitude = vessel.altitude;
//			savedVessel.angularMomentum = vessel.angularMomentum;
//			savedVessel.angularVelocity = vessel.angularVelocity;


		}

		private void debugTxt(string line)
		{
			string file = "debugList.txt";
			if(KSP.IO.File.Exists<EscapePod>(file,null) == false)
				KSP.IO.File.CreateText<EscapePod>(file,null);
		  	
				KSP.IO.File.AppendAllText<EscapePod>(line+"\n",file,null);

		}
	
		protected override void onDisconnect ()
		{
			//KSP.IO.File.CreateText("x",vessel);
			//vesselInfo("onDisconnect");
			base.onDisconnect ();
			//vesselInfo("Base.onDisconnect");


		}
//		protected void print(string s){
//			debugTxt(s);
//			MonoBehaviour.print(s);
//		}

	}
}

