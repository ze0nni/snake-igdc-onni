
using System;
using mathxd;
using System.Drawing;
using System.Collections.Generic;

namespace PytRt {
	public enum PyPlayerVector {
		up, down, left, right	
	}
	
	public class PyPlayerSector {
		public PyPlayerSector(int x, int y, bool head) {
			X = x;
			Y = y;
			Head = head;
		}
		public int X;
		public int Y;
		public bool Head;
		public bool Fat;
		public PyPlayerVector Vector = PyPlayerVector.right;
		public float Shift;
		public bool Grow;
		public bool Wait;
	}
	
	public class PyPlayerClass {
				
		public PyPlayerClass(PyMapClass map) {
			FMap = map;	
			FMap.Player = this;
			Py.Add(new PyPlayerSector(map.Size/2+1, map.Size/2, true));
			Py.Add(new PyPlayerSector(map.Size/2, map.Size/2, false));
			Py.Add(new PyPlayerSector(map.Size/2-1, map.Size/2, false));			

		}
		
		private List<PyPlayerSector> Py = new List<PyPlayerSector>();
		
		private PyPlayerVector FNextVector = PyPlayerVector.right;
		public PyPlayerVector NextVector {
			get { return FNextVector; }
			set { 
				switch (Py[0].Vector) {
					case PyPlayerVector.up:
						if (value == PyPlayerVector.down) return;
						break;
					case PyPlayerVector.down:
						if (value == PyPlayerVector.up) return;
						break;
					case PyPlayerVector.left:
						if (value == PyPlayerVector.right) return;
						break;
					case PyPlayerVector.right:
						if (value == PyPlayerVector.left) return;
						break;					
				}
				FNextVector = value;
			}
		}
		
		
		private PyMapClass FMap;
		public PyMapClass Map {
			get { return FMap; }
		}
		
		public void Action() {
			bool Isoffset=false;
			int LX = 0;
			int LY = 0;
		 	PyPlayerVector LVector = PyPlayerVector.right;
			foreach (PyPlayerSector sec in Py) {
				sec.Shift += (float)0.15+(float)Map.Score/3000;
				if (sec.Shift >= 1) {
					if (sec.Wait) break;
					Isoffset = true;
					sec.Shift-=1;
					LX = sec.X;
					LY = sec.Y;
					LVector = sec.Vector;
					switch (sec.Vector) {
						case PyPlayerVector.up:
							sec.Y--;
							break;
						case PyPlayerVector.down:
							sec.Y++;
							break;
						case PyPlayerVector.left:
							sec.X--;
							break;
						case PyPlayerVector.right:
							sec.X++;
							break;
					}
					if (sec.X >= Map.Size) sec.X = 0;
					if (sec.Y >= Map.Size) sec.Y = 0;
					if (sec.X < 0) sec.X = Map.Size-1;
					if (sec.Y < 0) sec.Y = Map.Size-1;
				}
			}
			if (Isoffset) {								
				for (int i=Py.Count-1; i>=0; i--) {										
					PyPlayerSector sec = Py[i];
					if (i==0) { 
						sec.Vector = NextVector;
						Map.ExecuteCell(sec.X, sec.Y);
					} else {
						if (Py[i-1].Grow) {
							sec.Grow = true;
							Py[i-1].Grow = false;
						}
						sec.Vector = Py[i-1].Vector;
					}					
				}
				PyPlayerSector lsec = Py[Py.Count-1];
				if (lsec.Grow) {
					lsec.Grow = false;
					PyPlayerSector nsec = new PyPlayerSector(LX, LY, false);
					nsec.Vector = LVector;
					Py.Add(nsec);
					nsec.Shift = lsec.Shift;
					Map.AddEffect(new PyBeepEffectClass(Map, nsec.X, nsec.Y));
				}			
			}
		}
		
		public void Render(Polygons w, float ds) {
			for (int i=0; i<Py.Count; i++) {
				PyPlayerSector sec = Py[i];
				float dx = -1 + sec.X*ds + ds/2;
				float dy = -1 + sec.Y*ds + ds/2;
				Brush b = sec.Head ? Brushes.Red : Brushes.LightCoral;				
				float offset = ds * sec.Shift;
				if (sec.Vector == PyPlayerVector.up || sec.Vector == PyPlayerVector.left) {
					offset = -offset;	
				}
				float angle = 0;
				//AnimationTick++;
				//float aa = (float)((AnimationTick*Math.PI/100 + i*Math.PI/2));
				//dx += (float)(Math.Sin(aa)*ds/30);
				//dy += (float)(Math.Cos(aa)*ds/30);
				switch (sec.Vector) {
					case PyPlayerVector.up:
						angle = 0;
						break;
					case PyPlayerVector.down:
						angle = (float)Math.PI;
						break;
					case PyPlayerVector.left:
						angle = (float)(Math.PI/2);
						break;
					case PyPlayerVector.right:
						angle = (float)(Math.PI*1.5);
						break;
				}
					
				switch (sec.Vector) {
					case PyPlayerVector.up:
					case PyPlayerVector.down:
					w.Add(new PyPlayerSectorView(dx, dy+offset, ds/2, ds*(float)1.1, angle, i==0));
					break;
					case PyPlayerVector.left:
					case PyPlayerVector.right:
					w.Add(new PyPlayerSectorView(dx+offset, dy, ds/2, ds*(float)1.1, angle, i==0));
					break;
				}
				/* rect body
				if (i==0) {
				#region head 
					switch (sec.Vector) {
						case PyPlayerVector.up:
						case PyPlayerVector.down:
							w.AddBox(
							         new Vector(dx, dy+offset, 0, 1),
							         new Vector(dx+ds,dy+ds+offset, ds, 1),
							         b);
							break;
						case PyPlayerVector.left:
						case PyPlayerVector.right:
							w.AddBox(
							         new Vector(dx+offset, dy, 0, 1),
							         new Vector(dx+ds+offset,dy+ds, ds, 1),
							         b);
							break;
					}
				#endregion
				} else if (i==1) {
				#region	head2
				switch (sec.Vector) {
						case PyPlayerVector.up:
							w.AddBox(
							         new Vector(dx, dy+offset, 0, 1),
							         new Vector(dx+ds,dy+ds, ds, 1),
							         b);
							break;
						case PyPlayerVector.down:
							w.AddBox(
							         new Vector(dx, dy, 0, 1),
							         new Vector(dx+ds,dy+ds+offset, ds, 1),
							         b);
							break;
						case PyPlayerVector.left:
							w.AddBox(
							         new Vector(dx+offset, dy, 0, 1),
							         new Vector(dx+ds,dy+ds, ds, 1),
							         b);
							break;
						case PyPlayerVector.right:
							w.AddBox(
							         new Vector(dx, dy, 0, 1),
							         new Vector(dx+ds+offset,dy+ds, ds, 1),
							         b);
							break;
					}	
				#endregion
				} else if (i==Py.Count-1) {
				#region	tail	
					if (sec.Grow) offset = 0;
					switch (sec.Vector) {
						case PyPlayerVector.up:
							w.AddBox(
							         new Vector(dx, dy, 0, 1),
							         new Vector(dx+ds,dy+ds+offset, ds, 1),
							         b);
							break;
						case PyPlayerVector.down:
							w.AddBox(
							         new Vector(dx, dy+offset, 0, 1),
							         new Vector(dx+ds,dy+ds, ds, 1),
							         b);
							break;
						case PyPlayerVector.left:
							w.AddBox(
							         new Vector(dx, dy, 0, 1),
							         new Vector(dx+ds+offset,dy+ds, ds, 1),
							         b);
							break;
						case PyPlayerVector.right:
							w.AddBox(
							         new Vector(dx+offset, dy, 0, 1),
							         new Vector(dx+ds,dy+ds, ds, 1),
							         b);
							break;
					}
				#endregion
				} else {
				#region	body
					switch (sec.Vector) {
						case PyPlayerVector.up:
							w.AddBox(
							         new Vector(dx, dy, 0, 1),
							         new Vector(dx+ds,dy+ds, ds, 1),
							         b);
							break;
						case PyPlayerVector.down:
							w.AddBox(
							         new Vector(dx, dy, 0, 1),
							         new Vector(dx+ds,dy+ds, ds, 1),
							         b);
							break;
						case PyPlayerVector.left:
							w.AddBox(
							         new Vector(dx, dy, 0, 1),
							         new Vector(dx+ds,dy+ds, ds, 1),
							         b);
							break;
						case PyPlayerVector.right:
							w.AddBox(
							         new Vector(dx, dy, 0, 1),
							         new Vector(dx+ds,dy+ds, ds, 1),
							         b);
							break;
					}	
				#endregion	
				}
				rect body */
				if (sec.Grow && i != 0) {
					/* rect body 
					w.AddBox(
					         new Vector(dx-ds*0.2, dy-ds*0.2, 0, 1),
					         new Vector(dx+ds*1.2,dy+ds*1.2, ds, 1),
					         b);
					*/
					w.Add(new PyPlayerSectorView(dx, dy, ds/(float)2.1, ds*(float)1.3, angle, false));
				}
			}
		}
		
		public void DoGrow() {
			Py[0].Grow = true;
		}
		
		public int HeadX {
			get { return Py[0].X; }
		}
		
		public int HeadY {
			get { return Py[0].Y; }
		}
		
		public bool IsCellEmpty(int x, int y) {
			foreach (PyPlayerSector s in Py)
				if (s.X == x && s.Y == y) return false;				
			return true;
		}
		
		public bool IsHeadCellEmpty(int x, int y) {
			foreach (PyPlayerSector s in Py)
				if (s != Py[0]  && s.X == x && s.Y == y) return false;				
			return true;
		}
	}
}
