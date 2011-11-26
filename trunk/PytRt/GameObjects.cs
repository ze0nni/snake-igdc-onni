
using System;
using System.Collections.Generic;
using System.Drawing;

namespace mathxd
{
	public abstract class CustomPolygon {		
		public abstract double Z {
			get;
		}							
		public abstract void Draw(Graphics g, double scale);
		public abstract CustomPolygon GetMult(Matrix3d m1);
		public abstract Boolean ChechXY(double x, double y);
	}
	
	public class Polygon : CustomPolygon {
		private Vector[] p = new Vector[3];
		private Brush FillBrush;
		
		public Polygon(double x1,double y1, double z1,
		               double x2, double y2, double z2,
		               double x3, double y3, double z3,
		               Brush b) {
			p[0] = new Vector(x1, y1, z1, 1);
			p[1] = new Vector(x2, y2, z2, 1);
			p[2] = new Vector(x3, y3, z3, 1);
			FillBrush = b;
		}
		
		public Polygon(Vector v1,
		               Vector v2,
		               Vector v3,
		               Brush b) {
			p[0] = v1;
			p[1] = v2;
			p[2] = v3;
			FillBrush = b;
		}
		
		public override double Z {
			get { return (p[0].Z + p[1].Z + p[2].Z)/3.0f; }
		}					
		
		public override void Draw(Graphics g, double scale) {
			PointF[] P = new PointF[3];
			P[0] = new PointF((float)(p[0].X * scale), (float)(p[0].Y * scale));
			P[1] = new PointF((float)(p[1].X * scale), (float)(p[1].Y * scale));
			P[2] = new PointF((float)(p[2].X * scale), (float)(p[2].Y * scale));			
			g.FillPolygon(FillBrush, P);	
			/*
			for (int i=0; i<3; i++)
			 g.DrawString(String.Format("{0}", i),
			             new Font(FontFamily.GenericMonospace, 6, FontStyle.Regular),
			             Brushes.Black,
			             (int)P[i].X, (int)P[i].Y);
			*/
		}
		
		public override CustomPolygon GetMult(Matrix3d m1) {
			return new Polygon(
			                   p[0] * m1,
			                   p[1] * m1,
			                   p[2] * m1,
			                   FillBrush);
		}
		
		public override Boolean ChechXY(double x, double y) {
			double d12 = (p[0].X - x) * (p[1].Y - y) - (p[1].X - x) * (p[0].Y - y);
			double d23 = (p[1].X - x) * (p[2].Y - y) - (p[2].X - x) * (p[1].Y - y);
			double d31 = (p[2].X - x) * (p[0].Y - y) - (p[0].X - x) * (p[2].Y - y);
			return (d12 * d23 > 0) & (d23 * d31 > 0);
		}
		
		public double GetZByXY(double x, double y) {
			//todo:
			return 0.0f;	
		}
		
		private Vector GetVectorToSlide(double x1, double y1, double x2, double y2, int vector) {			
			double a = Math.Atan2(y2 - y1, x2 - x1) + Math.PI/2.0f *vector;
			return new Vector(Math.Cos(a), Math.Sin(a), 0, 1);
		}
		
		public Vector[] GetSlidesVectors() {			
			Vector[] V = new Vector[3];
			int vect = 1;
						
			double a = Math.Atan2(p[1].Y - p[0].Y, p[1].X - p[0].X) - Math.Atan2(p[2].Y - p[0].Y, p[2].X - p[0].X);
			if (a<0) a += Math.PI*2;
			if (a>Math.PI) vect = -1;
			
			V[0] = GetVectorToSlide(p[0].X, p[0].Y, p[1].X, p[1].Y, vect);
			V[1] = GetVectorToSlide(p[1].X, p[1].Y, p[2].X, p[2].Y, vect);
			V[2] = GetVectorToSlide(p[2].X, p[2].Y, p[0].X, p[0].Y, vect);
			return V;
		}
		
		private double GetLenToSlide(double x, double y, double x1, double y1, double x2, double y2) {
			double res = 1.0f;
			
			x -= x1;
			y -= y1;
			x2 -= x1;
			y2 -= y1;
			double lineLen = Math.Sqrt(x*x + y*y);
			double ptAngle = Math.Atan2(y, x);
			double lnAngle = Math.Atan2(y2, x2);
			res = -Math.Sin(ptAngle-lnAngle) * lineLen;
			return Math.Abs(res);
		}
		
		public double[] GetLenToSlides(double x, double y) {
			double[] res = new double[3];
		 	res[0] = GetLenToSlide(x, -y, p[0].X, p[0].Y, p[1].X, p[1].Y);
			res[1] = GetLenToSlide(x, -y, p[1].X, p[1].Y, p[2].X, p[2].Y);
			res[2] = GetLenToSlide(x, -y, p[2].X, p[2].Y, p[0].X, p[0].Y);
			return res;
		}
		
		public double GetZValue(){
			return (p[0].Z + p[1].Z	+ p[1].Z)/3.0f;
		}
	}

	
	public class Polygons {
		private List<CustomPolygon> P = new List<CustomPolygon>();
		
		public static Polygons operator* (Polygons p1, Matrix3d m1) {
		 	//todo:
			Polygons pr = new Polygons();
			for (int i=0; i<p1.P.Count; i++) {
				pr.Add(p1.P[i].GetMult(m1));
			}
			return pr;
		}
		
		public void Clear() {
			P.Clear();	
		}
		
		public void Add(CustomPolygon newP) {
			P.Add(newP);	
		}
		
		public void AddBox(Vector v1, Vector v2, Brush b) {
			/**/
			P.Add(new Polygon(v1.X, v1.Y, v1.Z,
			                  v2.X, v1.Y, v1.Z,
			                  v1.X, v2.Y, v1.Z,
			                  b));
			P.Add(new Polygon(v1.X, v1.Y, v1.Z,
			                  v2.X, v1.Y, v1.Z,
			                  v1.X, v1.Y, v2.Z,
			                  b));
			P.Add(new Polygon(v1.X, v1.Y, v1.Z,
			                  v1.X, v2.Y, v1.Z,
			                  v1.X, v1.Y, v2.Z,
			                  b));
			P.Add(new Polygon(v2.X, v1.Y, v1.Z,
			                  v1.X, v2.Y, v1.Z,
			                  v2.X, v2.Y, v1.Z,
			                  b));
			P.Add(new Polygon(v2.X, v1.Y, v1.Z,
			                  v1.X, v1.Y, v2.Z,
			                  v2.X, v1.Y, v2.Z,
			                  b));
			P.Add(new Polygon(v1.X, v2.Y, v1.Z,
			                  v1.X, v1.Y, v2.Z,
			                  v1.X, v2.Y, v2.Z,
			                  b));
			/**/
			//P.Add(new Polygon(v2.X, v2.Y, v2.Z,
			//                 v1.X, v2.Y, v2.Z,
			//                  v2.X, v1.Y, v2.Z,
			//                  b));
			                  
			/* */
			P.Add(new Polygon(v2.X, v2.Y, v2.Z,
			                  v1.X, v2.Y, v2.Z,
			                  v2.X, v2.Y, v1.Z,
			                  b));
			P.Add(new Polygon(v2.X, v2.Y, v2.Z,
			                  v2.X, v1.Y, v2.Z,
			                  v2.X, v2.Y, v1.Z,
			                  b));
			/* */
			//P.Add(new Polygon(v1.X, v2.Y, v2.Z,
			//                  v2.X, v1.Y, v2.Z,
			//                  v1.X, v1.Y, v2.Z,
			//                  b));
			
			/* */ 
			P.Add(new Polygon(v1.X, v2.Y, v2.Z,
			                  v2.X, v2.Y, v1.Z,
			                  v1.X, v2.Y, v1.Z,
			                  b));
			P.Add(new Polygon(v2.X, v1.Y, v2.Z,
			                  v2.X, v2.Y, v1.Z,
			                  v2.X, v1.Y, v1.Z,
			                  b));
			/* */
		}
		
		public void Draw(Graphics g, double scale) {
			for (int i=0; i<P.Count; i++) {
				P[i].Draw(g, scale);	
			}
		}
		
		public void Sort() {
			int c=P.Count;
			while (c>0)
			{
				for (int i=0; i<c-1; i++) {
					if (P[i].Z < P[i+1].Z) {
						CustomPolygon tp = P[i];
						P[i] = P[i+1];
						P[i+1] = tp;
					}					
				}
				c--;
			}
		}
		
		public CustomPolygon getPoygonXY(double x, double y) {
			for (int i=P.Count-1; i>=0; i--){
				if (P[i].ChechXY(x, -y)) return P[i];
			}
			return null;	
		}
	}
	
	public class MapObject: CustomPolygon {
		protected Vector P = new Vector();
		protected Vector Ps = new Vector();
		
		public MapObject() {
		}
		
		public MapObject(double x, double y, double z) {
			P = new Vector(x, y, z, 1);
			Ps = new Vector(x+0.05f, y, z, 1);
		}
		
		public MapObject(Vector v, Vector v1) {
			P = v;
			Ps = v1;
		}
		
		public override double Z {
			get { return P.Z; }
		}
		
		public override Boolean ChechXY(double x, double y) {			
			return false;
		}
		
		public override CustomPolygon GetMult(Matrix3d m1) {
			return new MapObject(P * m1, Ps * m1);
		}
		
		public override void Draw(Graphics g, double scale) {
		}
		
		public virtual void Action() {
			
		}
	}
		
	public class MapHpObject: MapObject {
		public MapHpObject(double x, double y, double z) : base(x, y, z) {
		}
		
		public MapHpObject(Vector v, Vector v1) : base(v, v1) {
		}
		
		public override void Draw(Graphics g, double scale) {
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			int eSize = (int)(Math.Sqrt(
			                      Math.Pow(P.X - Ps.X, 2) + 
			                      Math.Pow(P.Y - Ps.Y, 2) +
			                      Math.Pow(P.Z - Ps.Z, 2)
			                     )*scale);	
			eSize = (int)(eSize * (Math.Sin(Environment.TickCount/100.0f)/10+0.9));
			
			g.FillEllipse(Brushes.LightCoral,
			              (int)((P.X*scale - eSize)),
			              (int)((P.Y*scale - eSize)),
			              eSize * 2, eSize * 2);	
			g.DrawEllipse(Pens.Red,
			              (int)((P.X*scale - eSize)),
			              (int)((P.Y*scale - eSize)),
			              eSize * 2, eSize * 2);	
			
			
			int Px = (int)(P.X*scale);
			int Py = (int)(P.Y*scale);
			PointF[] pts = new PointF[6];
			pts[0] = new PointF(Px, Py + eSize*0.8f);
			pts[1] = new PointF(Px-eSize*0.8f, Py);
			pts[2] = new PointF(Px-eSize* 0.6f, Py-eSize* 0.4f);
			pts[3] = new PointF(Px, Py);
			pts[4] = new PointF(Px+eSize* 0.6f, Py-eSize* 0.4f);
			pts[5] = new PointF(Px+eSize*0.8f, Py);
			g.FillPolygon(Brushes.White, pts);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
		}
		
		public override CustomPolygon GetMult(Matrix3d m1) {
			return new MapHpObject(P * m1, Ps * m1);
		}
	}
	
	public class MapObjectStar: MapObject {
		public MapObjectStar(double x, double y, double z) : base(x, y, z) {
			
		}
		
		private MapObjectStar(Vector v, Vector v1) : base(v, v1) {
				
		}
								
		public override void Draw(Graphics g, double scale) {
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			int eSize = (int)(Math.Sqrt(
			                      Math.Pow(P.X - Ps.X, 2) + 
			                      Math.Pow(P.Y - Ps.Y, 2) +
			                      Math.Pow(P.Z - Ps.Z, 2)
			                     )*scale);	
			eSize = (int)(eSize * (Math.Sin(Environment.TickCount/100.0f)/10+0.9));
			
			g.FillEllipse(Brushes.LightGreen,
			              (int)((P.X*scale - eSize)),
			              (int)((P.Y*scale - eSize)),
			              eSize * 2, eSize * 2);
			g.DrawEllipse(Pens.Green,
			              (int)((P.X*scale - eSize)),
			              (int)((P.Y*scale - eSize)),
			              eSize * 2, eSize * 2);
			
			int Px = (int)(P.X*scale);
			int Py = (int)(P.Y*scale);
			PointF[] pts = new PointF[12];
			double aStep = Math.PI*2.0f/12.0f;
			for (int i=0; i<12; i++) {
				double s = eSize * 0.9f;
				if (i % 2 == 0) s *= 0.3f;
				pts[i] = new PointF(
				                    (float)(Px + Math.Cos(i * aStep)*s),
				                    (float)(Py + Math.Sin(i * aStep)*s)
				                   );
			}
			g.FillPolygon(Brushes.White, pts);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
		}
		
		public override CustomPolygon GetMult(Matrix3d m1) {
			return new MapObjectStar(P * m1, Ps * m1);
		}
	}
	
	public class MapObjectPackMan: MapObject {
		public MapObjectPackMan(double x, double y, double z) : base(x, y, z) {
			
		}
		
		private MapObjectPackMan(Vector v, Vector v1) : base(v, v1) {
				
		}
								
		public override void Draw(Graphics g, double scale) {	
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			int eSize = (int)(Math.Sqrt(
			                      Math.Pow(P.X - Ps.X, 2) + 
			                      Math.Pow(P.Y - Ps.Y, 2) +
			                      Math.Pow(P.Z - Ps.Z, 2)
			                     )*scale);
			eSize = (int)(eSize * (Math.Sin(Environment.TickCount/100.0f)/10+0.9));
			int mSize = (int)(Math.Abs(Math.Sin(Environment.TickCount/100.0f))*30);
			
			g.FillPie(Brushes.Yellow,
			              (int)((P.X*scale - eSize)),
			              (int)((P.Y*scale - eSize)),
			              eSize * 2, eSize * 2,
			             0 + mSize, 360 - mSize * 2);
			g.DrawPie(Pens.LightCoral,
			              (int)((P.X*scale - eSize)),
			              (int)((P.Y*scale - eSize)),
			              eSize * 2, eSize * 2,
			             0 + mSize, 360 - mSize * 2);
			g.FillEllipse(Brushes.LightCoral,
			              (int)((P.X*scale - eSize*0.4)),
			              (int)((P.Y*scale - eSize*0.8)),
			              (int)(eSize*0.6), (int)(eSize*0.6));
			
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
		}
		
		public override CustomPolygon GetMult(Matrix3d m1) {
			return new MapObjectPackMan(P * m1, Ps * m1);
		}
	}
}
