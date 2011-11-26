
using System;
using System.Collections.Generic;
using System.Drawing;

namespace mathxd
{
	public class Matrix3d {
		public double[,] m;
		public Matrix3d() {
			m = new double[4,4];
			InitIdentity();
		}		
		
		public void InitIdentity() {
			for (int r=0; r<4; r++)
				for (int c=0; c<4; c++) {
					if(r==c) m[r, c] = 1; else m[r, c] = 0;
				}
		}
		
		public static Matrix3d operator* (Matrix3d m1, Matrix3d m2) {
			Matrix3d mr = new Matrix3d();
			for (int r=0; r<4; r++)
				for (int c=0; c<4; c++) {
					mr.m[r, c] =
						m1.m[r, 0] * m2.m[0, c] + 
						m1.m[r, 1] * m2.m[1, c] + 
						m1.m[r, 2] * m2.m[2, c] + 
						m1.m[r, 3] * m2.m[3, c];
				}
			return mr;
		}
		
		public static Matrix3d getRotateX(double a) {
			Matrix3d r = new Matrix3d();
			r.m[1,1] = Math.Cos(a);
			r.m[1,2] = Math.Sin(a);
			r.m[2,1] = -Math.Sin(a);
			r.m[2,2] = Math.Cos(a);
			return r;
		}
		
		public static Matrix3d getRotateY(double a) {
			Matrix3d r = new Matrix3d();
			r.m[0,0] = Math.Cos(a);
			r.m[0,2] = -Math.Sin(a);
			r.m[2,0] = Math.Sin(a);
			r.m[2,2] = Math.Cos(a);
			return r;
		}
		
		public static Matrix3d getRotateZ(double a) {
			Matrix3d r = new Matrix3d();
			r.m[0,0] = Math.Cos(a);
			r.m[0,1] = Math.Sin(a);
			r.m[1,0] = -Math.Sin(a);
			r.m[1,1] = Math.Cos(a);
			return r;
		}
		
		public static Matrix3d getScale(double sx, double sy, double sz) {
			Matrix3d r = new Matrix3d();
			r.m[0,0] = sx;
			r.m[1,1] = sy;
			r.m[2,2] = sz;
			return r;
		}
		
		public static Matrix3d getTranslate(double tx, double ty, double tz) {
			Matrix3d r = new Matrix3d();
			r.m[3,0] = tx;
			r.m[3,1] = ty;
			r.m[3,2] = tz;
			return r;
		}
		
		public static Matrix3d getOrtho() {
			Matrix3d r = new Matrix3d();			
			return r;
		}
		
		public static Matrix3d getPerspective(double dx, double dy, double dz) {
			Matrix3d r = new Matrix3d();
			if (dx != 0) r.m[0,3] = 1/dx;
			if (dy != 0) r.m[1,3] = 1/dy;
			if (dz != 0) r.m[2,3] = 1/dz;			
			return r;
		}
		
		public static Matrix3d getCabinet(double s) {
			Matrix3d r = new Matrix3d();
			//todo:
			return r;	
		}
		
		public static Matrix3d getIsomertic(double s) {
			Matrix3d r = new Matrix3d();
			//todo:
			return r;
		}
		
		public static Matrix3d getView(int w, int h) {
			Double s = Math.Min(w/2.0f, h/2.0f);
			return
				Matrix3d.getScale(s, -s, 1);				
		}
	}
	
	public class WorldClass {
		private Matrix3d FView = new Matrix3d();	
		private Matrix3d FCamera = new Matrix3d();
		private Matrix3d FProjection = new Matrix3d();
		private Matrix3d FTransformation = new Matrix3d();
		private Matrix3d FModel = new Matrix3d();
		private Boolean fIsMatrixChange = true;
		private Matrix3d FCompozition = new Matrix3d();
		
		public Matrix3d View {
			get { return FView; }
			set { fIsMatrixChange = true; FView = value; }
		}
		public Matrix3d Camera {
			get { return FCamera; }
			set { fIsMatrixChange = true; FCamera = value; }
		}
		public Matrix3d Projection {
			get { return FProjection; }
			set { fIsMatrixChange = true; FProjection = value; }
		}
		public Matrix3d Transformation {
			get { return FTransformation; }
			set { fIsMatrixChange = true; FTransformation = value; }
		}
		public Matrix3d Model {
			get { return FModel; }
			set { fIsMatrixChange = true; FModel = value; }
		}		
		public Matrix3d Compozition {
			get {
					if (fIsMatrixChange)
					{
						FCompozition =
							FTransformation *
							FView *
							FProjection *							
							//???
							FCamera *
							FModel;
						fIsMatrixChange = false;
					}
					return FCompozition;
				}
		}
	}
	
	public class Vector {
		
		private double[] v = new double[4];
		public Vector() {
			
		}
		
		public Vector(double sx, double sy, double sz, double sw) {
			X = sx;
			Y = sy;
			Z = sz;
			W = sw;
		}
				
		public double X {
			get { return v[0]; }
			set { v[0] = value; }
		}
		
		public double Y {
			get { return v[1]; }
			set { v[1] = value; }
		}
		
		public double Z {
			get { return v[2]; }
			set { v[2] = value; }
		}
		
		public double W {
			get { return v[3]; }
			set { v[3] = value; }
		}		
		
		public static Vector operator* (Vector v1, Matrix3d m1) {
			Vector vr = new Vector();
			for (int i=0; i<4; i++)
				vr.v[i] =
					v1.v[0] * m1.m[0, i] + 
					v1.v[1] * m1.m[1, i] + 
					v1.v[2] * m1.m[2, i] + 
					v1.v[3] * m1.m[3, i];
			if (vr.v[3] != 1) {
				vr.v[0] /= vr.v[3];	
				vr.v[1] /= vr.v[3];
				vr.v[2] /= vr.v[3];
				vr.v[3] = 1.0f;
			}
			return vr;
		}		
	}
}
