
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using mathxd;

namespace PytRt {
	public enum ScreenStates {
		menu,
		game,
		submitscore
	}
	
	public class Program : Form {
		
		private Polygons World = new Polygons();
		private Polygons ViewWorld;
		private WorldClass WorldTransformation = new WorldClass();
		private PyBoardClass Board = new PyBoardClass();
		private static void Main(string[] args) {
			Application.Run(new Program());
		}
		

		public Program() : base() {								
			this.DoubleBuffered = true;
			
			Map = new PyMapClass(11);
			Player = new PyPlayerClass(Map);
			
			WindowState = FormWindowState.Maximized;
			
			Timer GameThread  = new Timer();
			GameThread.Interval = 30;
			GameThread.Tick += GameTimerHandleTick;
			GameThread.Start();
			
			Board.SubmitScore("", 0);
		}
				
		void GameTimerHandleTick(object o, EventArgs e) {
			Action();
			Invalidate();
			if (ScreenState == ScreenStates.game)
				Text = String.Format(
				                     "Score: {0}", Map.Score);
		}	
		
		private PyMapClass Map;
		private PyPlayerClass Player;
		private ScreenStates ScreenState = ScreenStates.menu;
		private int RockEffect = 0;
		private string PlayerName = "";
		protected void Action() {
			if (ScreenState == ScreenStates.game) {
				Player.Action();				
			}
			Map.Action();
			if (RockEffect > 0) RockEffect--;
			if (ScreenState == ScreenStates.game && !Map.IsHeadCellEmpty(Player.HeadX, Player.HeadY)) {				
				ScreenState = ScreenStates.submitscore;
				Map.AddEffect(new PyBeepEffectClass(Map, Player.HeadX, Player.HeadY));
				RockEffect += 20;
			}
		}
		
		protected override void OnPaintBackground (System.Windows.Forms.PaintEventArgs e) {
		}
		
		private Random rndWorldView = new Random();
		float RotateX;
		float RotateY;
		private void Draw(Graphics g) {
			World.Clear();
			Map.Render(World);
			Player.Render(World, (float)2/Map.Size);
			
			WorldTransformation.Camera = Matrix3d.getPerspective(0, 0, 2);
			float tx = 0;
			float ty = 0;
			if (ScreenState == ScreenStates.game) {
				tx = (float)2/Map.Size*(Player.HeadX - Map.Size/2)/10;
				ty = (float)2/Map.Size*(Player.HeadY - Map.Size/2)/10;
			} else {
				tx = (float)(Math.Sin(Environment.TickCount/(float)2000));
				ty = (float)(Math.Cos(Environment.TickCount/(float)2000));
			}
			RotateX -= (RotateX - tx) / 10;
			RotateY -= (RotateY - ty) / 10;
			WorldTransformation.View =
				Matrix3d.getRotateY(RotateX) * 
				Matrix3d.getRotateX(-RotateY);
				
			ViewWorld = World * WorldTransformation.Compozition;			
			ViewWorld.Sort();
			
			g.Clear(Color.Black);			
			g.SmoothingMode = SmoothingMode.AntiAlias;			
			
			float scale = (float)Math.Min(ClientSize.Width, ClientSize.Height)/2;
			
			Matrix m = g.Transform;
			g.TranslateTransform(ClientSize.Width/2, ClientSize.Height/2);
			g.ScaleTransform(scale*(float)0.9, scale*(float)0.9);
			if (RockEffect != 0) {
				float eff = (float)RockEffect/500;
				float mvx = (float)Math.Sin(Environment.TickCount/(float)30)*eff;
				float mvy = (float)Math.Cos(Environment.TickCount/(float)30)*eff;
				g.TranslateTransform(mvx, mvy);	
			}
			ViewWorld.Draw(g, 1);
			g.Transform = m;
			
			{
				Font sF = new Font(FontFamily.GenericSansSerif,
				                  scale/16, GraphicsUnit.Pixel);
				g.DrawString(Text, sF, new SolidBrush(Color.FromArgb(172, 255, 0, 0)),
				             1, 1);
			}
			
			if (ScreenState == ScreenStates.submitscore) {
				#region
				g.FillRectangle(new SolidBrush(Color.FromArgb(172, 0, 0, 0)),
				                ClientRectangle);
				Font f = new Font(FontFamily.GenericSansSerif,
				                  scale/6, GraphicsUnit.Pixel);
				//GameOver
				SizeF sz = g.MeasureString("GAME OVER!", f);				
				g.DrawString("GAME OVER!", f, Brushes.Red,
				             (ClientSize.Width - sz.Width) / 2,
				             ClientSize.Height/2-sz.Height*2);
				
				//Name
				string txt = String.Format("name: {0}", PlayerName);
				sz = g.MeasureString(txt, f);
				if ((Environment.TickCount / 600) % 2 == 0) txt += '_';
				g.DrawString(txt, f, Brushes.Red,
				             (ClientSize.Width - sz.Width) / 2,
				             ClientSize.Height/2-sz.Height/2);
				
				//Enter
				sz = g.MeasureString("[ESC] [ENTER]", f);				
				g.DrawString("[ESC] [ENTER]", f, Brushes.Red,
				             (ClientSize.Width - sz.Width) / 2,
				             ClientSize.Height/2+sz.Height);
				#endregion
			}
			
			if (ScreenState == ScreenStates.menu) {
				#region
				g.FillRectangle(new SolidBrush(Color.FromArgb(172, 0, 0, 0)),
				                ClientRectangle);
				Font f = new Font(FontFamily.GenericSansSerif,
				                  scale/6, GraphicsUnit.Pixel);
				
				//GameOver
				if (Board.IsLoading) {
					string line = "Loading";
					for (int i=0; i<(Environment.TickCount/600%6); i++)
							line += ".";
					line = String.Format("[{0}]", line);
						SizeF sz = g.MeasureString(line, f);
						g.DrawString(line, f, Brushes.Red,
						             (ClientSize.Width - sz.Width) / 2,
						             ClientSize.Height/2-sz.Height*2);
				} else {
					for (int i=0; i<3; i++) {
						string line = String.Format("{0} - {1}",
						                            Board.Items[i].Nick,
						                            Board.Items[i].Score);
						SizeF sz = g.MeasureString(line, f);
						g.DrawString(line, f, Brushes.Red,
						             (ClientSize.Width - sz.Width) / 2,
						             ClientSize.Height/2-sz.Height*3+sz.Height*i);
					}
				}
				
				string ln = "[ESC] [ENTER]";
				SizeF lsz = g.MeasureString(ln, f);
					g.DrawString(ln, f, Brushes.Red,
					             (ClientSize.Width - lsz.Width) / 2,
					             ClientSize.Height/2+lsz.Height);
				#endregion
			}
		}
		
		protected override void OnPaint (System.Windows.Forms.PaintEventArgs e) {						
			lock(this) {
				Draw(e.Graphics);	
			}
		}

		
		protected override void OnKeyDown (System.Windows.Forms.KeyEventArgs e) {
			base.OnKeyDown (e);
			switch (ScreenState) {
				case ScreenStates.game: 
					#region	 				
					switch (e.KeyCode) {
						case Keys.Up:
							Player.NextVector = PyPlayerVector.up;
							break;
						case Keys.Down:
							Player.NextVector = PyPlayerVector.down;
							break;
						case Keys.Left:
							Player.NextVector = PyPlayerVector.left;
							break;
						case Keys.Right:
							Player.NextVector = PyPlayerVector.right;
							break;
						case Keys.Escape:
							ScreenState = ScreenStates.submitscore;
							break;
					}
					#endregion
					break;
				case ScreenStates.submitscore:
					#region
					switch (e.KeyCode) {
						case Keys.Back :
							if (PlayerName.Length > 0) {
								PlayerName = PlayerName.Substring(0, PlayerName.Length-1);
							} else {
								RockEffect = 10;	
							}
							break;
						case Keys.Return:
							if (PlayerName.Length > 0) {
								ScreenState = ScreenStates.menu;
								Board.SubmitScore(PlayerName, Map.Score);
								Map = new PyMapClass(Map.Size);
							Player = new PyPlayerClass(Map);
							} else
								RockEffect = 10;
							break;
						case Keys.Escape:
							ScreenState = ScreenStates.menu;
							Map = new PyMapClass(Map.Size);
							Player = new PyPlayerClass(Map);
							break;
						default:
							if (PlayerName.Length < 8 &&
							    e.KeyData >= Keys.A &&
							    e.KeyData <= Keys.Z) {
								PlayerName += e.KeyData;
							} else {
								RockEffect = 10;	
							}						
							break;
					}
					#endregion								
				break;
				case ScreenStates.menu:
					#region
					switch (e.KeyCode) {
						case Keys.Escape:
							this.Close();
							break;
						case Keys.Return:
							ScreenState = ScreenStates.game;							
							break;
					}
					#endregion	
					break;				
			}
		}

	}
}
