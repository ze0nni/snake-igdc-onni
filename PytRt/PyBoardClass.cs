
using System;
using System.Collections.Generic;
using System.Net;

namespace PytRt {
		
	public class PyBoardItem {
		public PyBoardItem(string nick, int score, bool isYou) {
			Nick = nick;
			Score = score;
			IsYou = isYou;
		}
		public string Nick;
		public int Score;
		public bool IsYou;
	}
	
	public class PyBoardClass {
		
		public PyBoardClass() {
			FItems = new PyBoardItem[3];
			FItems[0] = new PyBoardItem("NO SERVER", 0, false);
			FItems[1] = new PyBoardItem("NO SERVER", 0, false);
			FItems[2] = new PyBoardItem("NO SERVER", 0, false);			
		}	
		
		private class SubmitScoreThread {
			public WebClient Wc;
			public Uri url;
			private void ThreadProc() {
				Wc.DownloadDataAsync(url);
			}
			
			public void Start() {
				System.Threading.Thread t= new System.Threading.Thread(ThreadProc);
				t.Start();
			}
		}
		
		public void SubmitScore(string nick, int score) {			
			FIsLoading = true;
			SubmitScoreThread c = new SubmitScoreThread();			
			c.Wc = new WebClient();
			c.Wc.DownloadDataCompleted += HandleDownloadDataCompleted;
			string url = String.Format("{0}?nick={1}&score={2}&hash={3}",
			                           "http://jrudelphi.org/cgi-bin/score.pl",
			                           nick,
			                           score,
			                           0);
			c.url = new Uri(url);
			c.Start();
		}

		void HandleDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
			FIsLoading = false;
			try {
				Console.WriteLine(e.Error);
				if (e.Result != null) {
					string[] lns = System.Text.Encoding.ASCII.GetString(e.Result).Split(';');
					List<PyBoardItem> l = new List<PyBoardItem>();
					foreach (string ln in lns) {
						string[] v = ln.Split('=');
						if (v.Length==2)
							l.Add(new PyBoardItem(v[0], int.Parse(v[1]), false));
					}
					if (l.Count>=3)
						Items = l.ToArray();
				} else {
					FItems = new PyBoardItem[3];
					FItems[0] = new PyBoardItem("NO SERVER", 0, false);
					FItems[1] = new PyBoardItem("NO SERVER", 0, false);
					FItems[2] = new PyBoardItem("NO SERVER", 0, false);				
				}
			} catch( Exception exc) {
				FItems = new PyBoardItem[3];
				FItems[0] = new PyBoardItem("NO SERVER", 0, false);
				FItems[1] = new PyBoardItem("NO SERVER", 0, false);
				FItems[2] = new PyBoardItem("NO SERVER", 0, false);
			} finally {

			}
		}
		
		private PyBoardItem[] FItems;
		
		public PyBoardItem[] Items {
			get { 
				lock (this) {
					return FItems;	
				}
			}
			set {
				lock (this) {
					FItems = value;
				}
			}
		}
		
		private bool FIsLoading;
		public bool IsLoading {
			get { return FIsLoading; }	
		}
	}
}
