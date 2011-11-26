using System;
using mathxd;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PytRt {
	public class CustomMapObject {
		
		protected PyMapClass Map;
		protected int CX;
		protected int CY;
		public CustomMapObject(PyMapClass map, int cx, int cy) {
			Map = map;
			CX = cx;
			CY = cy;
		}
		
		public virtual bool Action() {
			return true;
		}
		
		public virtual void Execute() {
			
		}
		
		public virtual void Draw(Polygons w, float ds) {
		
		}
	}
	
	public class PyObjectGreen : CustomMapObject {
		
		private int HashCode;
		public PyObjectGreen(PyMapClass map, int cx, int cy) : base(map, cx, cy) {
			HashCode = GetHashCode();
		}
		
		public override bool Action () {
			if (FShowEffect < 1) FShowEffect+= (float)0.05;
			if (FDieEffect) FShowEffect+= (float)0.1;
			return FShowEffect < 4;
		}
		
		private Random rnd = new Random();
		public override void Execute () {
			Map.Score += 3;
			Map.Player.DoGrow();
			Map.AddEffect(this);			
			Map.SetCell(CX, CY, null);			
			FDieEffect = true;
			int nx = 0;
			int ny = 0;
			
			do {
				nx = rnd.Next(Map.Size);
				ny = rnd.Next(Map.Size);
			} while(!Map.IsCellEmpty(nx, ny));
			Map.SetCell(nx, ny, new PyObjectGreen(Map, nx, ny));
			Map.ObjsCount++;
			if (Map.ObjsCount % 5 == 0) {				
				do {
				nx = rnd.Next(Map.Size);
				ny = rnd.Next(Map.Size);
				} while(!Map.IsCellEmpty(nx, ny));
				Map.SetCell(nx, ny, new PyObjectRed(Map, nx, ny));	
			}
		}

		
		private float FShowEffect;
		private bool FDieEffect = false;
		public override void Draw (mathxd.Polygons w, float ds) {
			float dx = -1+ds*CX + ds/2;
			float dy = -1+ds*CY + ds/2;
			
			Random col = new Random(HashCode);
			for (int i=0; i<5; i++)
				for (int j=0; j<3; j++) {
				float PiD6 = (float)Math.PI/3;
				float a = (float)(PiD6*i + Math.PI/10*j)+
					(float)(Math.Sin(Environment.TickCount/(float)(1000+(i*j+1)*300))*0.3);
				float rx = (float)Math.Sin(a)*ds/4*FShowEffect;
				float ry = (float)Math.Cos(a)*ds/4*FShowEffect;
				float rx1 = (float)Math.Sin(a+Math.PI/2)*ds/2*FShowEffect;
				float ry1 = (float)Math.Cos(a+Math.PI/2)*ds/2*FShowEffect;
				float up = !FDieEffect ?
					0 : -ds * (FShowEffect-1);
				byte alpha = 
					!FDieEffect ?
						(byte)255 :
						(byte)(128 - (float)128/3*(FShowEffect-1));
				w.Add(new Polygon(
				                  dx-rx, dy-ry, ds/2+up,
				                  dx+rx, dy+ry, ds/2+up,
				                  dx-rx1, dy-ry1, -ds/2+ds/5*j+up,
				                  new SolidBrush(Color.FromArgb(alpha, 0, 172-j*50+col.Next(50), 0)))
				      );
			}
		}
	}
	
	public class PyObjectRed : CustomMapObject {
		
		private int HashCode;
		public PyObjectRed(PyMapClass map, int cx, int cy) : base(map, cx, cy) {
			HashCode = GetHashCode();
		}
		
		public override bool Action () {
			if (FShowEffect < 1) FShowEffect+= (float)0.05;
			FTimer-=(float)(0.02 + (float)Map.Score/30000);
			if (FDieEffect) {
				FShowEffect+= (float)0.1;			
				FTimer = 1;
			}
			if (FTimer <= 0) Map.SetCell(CX, CY, null);
			return FShowEffect < 4;
		}
		
		private Random rnd = new Random();
		private float FTimer = 2;
		public override void Execute () {
			Map.Score += 3 + (int)(15 * FTimer);
			Map.Player.DoGrow();
			Map.AddEffect(this);			
			Map.SetCell(CX, CY, null);			
			FDieEffect = true;
		}

		
		private float FShowEffect;
		private bool FDieEffect = false;
		public override void Draw (mathxd.Polygons w, float ds) {
			float dx = -1+ds*CX + ds/2;
			float dy = -1+ds*CY + ds/2;
			
			Random col = new Random(HashCode);
			for (int i=0; i<6; i++)
				for (int j=0; j<3; j++) {
				float PiD6 = (float)Math.PI/3;
				float a = (float)(PiD6*i + Math.PI/12*j)+
					(float)(Math.Sin(Environment.TickCount/(float)(1000+(i*j+1)*300))*0.3);
				float rx = (float)Math.Sin(a)*ds/4*FShowEffect*FTimer;
				float ry = (float)Math.Cos(a)*ds/4*FShowEffect*FTimer;
				float rx1 = (float)Math.Sin(a+Math.PI/2)*ds/2*FShowEffect*FTimer;
				float ry1 = (float)Math.Cos(a+Math.PI/2)*ds/2*FShowEffect*FTimer;
				float up = !FDieEffect ?
					0 : -ds * (FShowEffect-1);
				byte alpha = 
					!FDieEffect ?
						(byte)255 :
						(byte)(128 - (float)128/3*(FShowEffect-1));
				w.Add(new Polygon(
				                  dx-rx, dy-ry, ds/2+up,
				                  dx+rx, dy+ry, ds/2+up,
				                  dx-rx1, dy-ry1, -ds/2+ds/4*j+up,
				                  new SolidBrush(Color.FromArgb(alpha, 172-j*50+col.Next(50), 0 , 0)))
				      );
			}
		}

	}
	
	public class PyBeepEffectClass : CustomMapObject {
		public PyBeepEffectClass(PyMapClass map, int x, int y) : base(map, x, y) {
		}
		
		private int FTimer = 20;
		public override bool Action () {
			FTimer--;
			return FTimer > 0;
		}

		
		public override void Draw (mathxd.Polygons w, float ds) {
			float dx = -1+ds*CX + ds/2;
			float dy = -1+ds*CY + ds/2;
			w.Add(new PyBeepPolygone(dx, dy, ds/2, ds, (float)(1.0-(float)FTimer/20)));
		}
	}
	
	public class PyPlayerSectorView : CustomPolygon {
		private Vector p1;
		private Vector p2;
		private float Angle;
		private bool Head;
		public PyPlayerSectorView(float dx,
		                          float dy,
		                          float dz,
		                          float ds,
		                          float angle,
		                          bool head)  {
			p1 = new Vector(dx, dy, dz, 1);
			p2 = new Vector(dx+ds, dy, dz, 1);
			Angle = angle;
			Head = head;
		}
		
		public override void Draw (System.Drawing.Graphics g, double scale) {
			float l =
				(float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.Z - p2.Z, 2))/2;
			Color c = !Head ?
				Color.FromArgb(127, 127, 127) : Color.FromArgb(80, 80, 80);
			
			Brush b = new LinearGradientBrush(
			                                  new PointF((float)p1.X-l, (float)p1.Y-l),
			                                  new PointF((float)p1.X, (float)p1.Y+l*2),
			                                  Color.LightGray,
			                                  c);
			                                  
			//Brush b = new SolidBrush(c);
			g.FillEllipse(new SolidBrush(Color.FromArgb(127, 127, 127)),
			(float)(p1.X-l*1.08), (float)(p1.Y-l*1.08), (float)(l*1.08*2), (float)(l*1.08*2));
			
			g.FillEllipse(b,
			              (float)(p1.X-l*1.08),
			              (float)(p1.Y-l*1.08),
			              l*2, l*2);
			
			if (Head) {
				for (int i=0; i<2; i++) {
					float iSz = (float)1.5*l*(i+1);
					float ix = (float)(Math.Sin(Angle-0.4+0.8*i) * l*0.75);
					float iy = (float)(Math.Cos(Angle-0.4+0.8*i) * l*0.75);
					g.FillEllipse(Brushes.Black,
					              (float)(p1.X - ix - iSz/10), 
					              (float)(p1.Y - iy - iSz/10),
					              iSz/5, iSz/5);
				}
			}
			
			g.FillEllipse(new SolidBrush(Color.FromArgb(64, 255, 255, 255)),
			              (float)(p1.X-l*0.65),
			              (float)(p1.Y-l*0.65),
			              (float)(l*0.5),
			              (float)(l*0.5));			
			
		}
		
		public override bool ChechXY (double x, double y) {
			return false;
		}
		
		public override double Z {
			get {
				return p1.Z;
			}
		}

		public override CustomPolygon GetMult (mathxd.Matrix3d m1) {
			PyPlayerSectorView p = new PyPlayerSectorView(0, 0, 0, 0, Angle, Head);
			p.p1 = p1 * m1;
			p.p2 = p2 * m1;                                              
			return p;
		}
	}
	
	public class PyBeepPolygone : CustomPolygon {
		
		private Vector p1;
		private Vector p2;
		private float Size;
		public PyBeepPolygone(float dx,
		                          float dy,
		                          float dz,
		                          float ds,
		                          float size)  {
			p1 = new Vector(dx, dy, dz, 1);
			p2 = new Vector(dx+ds, dy, dz, 1);
			Size = size;
		}
		
		public override void Draw (System.Drawing.Graphics g, double scale) {
			float l =
				(float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.Z - p2.Z, 2));
			Pen p = new Pen(Color.White, l/32);
			float sec = (float)(Math.PI*2/12);
			for (int i=0; i<12; i++) {
				float dx = (float)(Math.Cos(sec*i)*(Size));
				float dy = (float)(Math.Sin(sec*i)*(Size));
				g.DrawLine(p,
				           (float)(p1.X + dx*l),
				           (float)(p1.Y + dy*l),
				           (float)(p1.X + dx*l*(Size)),
				           (float)(p1.Y + dy*l*(Size)));
			}
		}
		
		public override bool ChechXY (double x, double y) {
			return false;
		}
		
		public override double Z {
			get {
				return p1.Z;
			}
		}

		public override CustomPolygon GetMult (mathxd.Matrix3d m1) {
			PyBeepPolygone p = new PyBeepPolygone(0, 0, 0, 0, Size);
			p.p1 = p1 * m1;
			p.p2 = p2 * m1;                                              
			return p;
		}
	}
}