
using System;
using System.Drawing;
using mathxd;
using System.Collections.Generic;

namespace PytRt
{
	
	
	public class PyMapClass {
		
		public int Score = 0;
		public PyPlayerClass Player;
		public int ObjsCount = 0;
		
		public PyMapClass(int size) {
			FSize = size;
			FMap = new CustomMapObject[Size, Size];
			Random rndMap = new Random();
			for (int i=0; i<1; i++) {
				int cx = rndMap.Next(Size);
				int cy = rndMap.Next(Size);
				FMap[cx, cy] = new PyObjectGreen(this, cx, cy);
			}
		}
				
		private int FSize;
		public int Size {
			get { return FSize; }
		}
		
		private CustomMapObject[,] FMap;
		public CustomMapObject[,] Map {
			get { return FMap; }	
		}
		
		public void SetCell(int x, int y, CustomMapObject o) {
			FMap[x, y] = o;	
		}
		
		public CustomMapObject GetCell(int x, int y) {
			return FMap[x, y];
		}
		
		private List<CustomMapObject> Effects = new List<CustomMapObject>();
		public void AddEffect(CustomMapObject e) {
			Effects.Add(e);	
		}
		
		public void Render(Polygons w) {
			#region Box
			for (int i=0;i<2;i++) {
				int l = -1 + i*2;
				w.Add(new Polygon(
				                  l, -1, 0,
				                  l, 1, 0,
				                  l, -1, 0.01,
				      Brushes.Gray));
				w.Add(new Polygon(
				                  l, -1, 0.01,
				                  l, 1, 0,
				                  l, 1, 0.01,
				      Brushes.Gray));
			}
			for (int i=0;i<2;i++) {
				int t = -1 + i*2;
				w.Add(new Polygon(
				                  -1, t, 0,
				                  1, t, 0,
				                  -1, t, 0.01,
				      Brushes.Gray));
				w.Add(new Polygon(
				                  -1, t, 0.01,
				                  1, t, 0,
				                  1, t, 0.01,
				      Brushes.Gray));
			}
			#endregion Box
			float cSize = (float)2/Size;
			
			for (int i=0; i<Size; i++)
				for (int j=0; j<Size; j++) {
				if (FMap[i,j] != null) 
					FMap[i,j].Draw(w,  cSize);
					               
			}
			foreach (CustomMapObject e in Effects.ToArray()) {
				e.Draw(w, cSize);	
			}
		}
		
		public void Action() {
			for (int i=0; i<Size; i++)
				for (int j=0; j<Size; j++) {
				if (FMap[i,j] != null) {
					if (!FMap[i,j].Action())
						FMap[i,j] = null;
				}
			}
			//effects
			if (Effects.Count!=0)
				for (int i=Effects.Count-1; i>=0; i--) {
					if (!Effects[i].Action()) Effects.RemoveAt(i);	
			}
		}
		
		public bool IsCellEmpty(int x, int y) {
			return 
				(FMap[x,y] == null) &&
					Player.IsCellEmpty(x,y);
		}
		
		public bool IsHeadCellEmpty(int x, int y) {
			return 
				(FMap[x,y] == null) &&
					Player.IsHeadCellEmpty(x,y);
		}
		
		public void ExecuteCell(int x, int y) {
			if (FMap[x,y] != null) {
				FMap[x,y].Execute();
			}
		}
	}
}
