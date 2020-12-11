using CefSharp;
using CefSharp.BrowserSubprocess;
using CefSharp.WinForms;
using FireSharp.Config;
using FireSharp.EventStreaming;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YouTubeSearch;

namespace SmartTV
{
	public partial class Main : Form
	{
		public string controllerControlling = "keyboard";

		bool paused = false;

		bool firstYoutubeVideo = true;

		[DllImport("user32.dll")]
		public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		[DllImport("user32.dll", EntryPoint = "SetCursorPos")]
		private static extern bool SetMousePos(int x, int y);

		private const int MOUSEEVENTF_LEFTDOWN = 0x02;
		private const int MOUSEEVENTF_LEFTUP = 0x04;

		private const int MOUSEEVENTF_RIGTHDOWN = 0x08;
		private const int MOUSEEVENTF_RIGTHUP = 0x010;

		IFirebaseConfig config = new FirebaseConfig
		{
			AuthSecret = "OTdTr8o3IhhNz6Ys5n9af4tlpBnyKTLRl1e9kYSN",
			BasePath = "https://smarttv-2967e-default-rtdb.firebaseio.com/"
		};

		IFirebaseClient client;

		ChromiumWebBrowser browser;

		int curentLine = 1;
		int curentRow = 0;
		string curentLetter = "Q";

		string respold = "";

		List<Control> line1 = new List<Control>();
		List<Control> line2 = new List<Control>();
		List<Control> line3 = new List<Control>();

		List<List<VideoVisual>> videoSearchResultsOnDisplay = new List<List<VideoVisual>>();
		int curentVideoSearchLine = 0;
		int curentVideoSearchRow = 0;

		public Main()
		{
			InitializeComponent();

			CefSettings a = new CefSettings();
			a.CefCommandLineArgs["autoplay-policy"] = "no-user-gesture-required";

			Cef.Initialize(a);
		}

		public void InitalizeDatabase()
		{
			client = new FireSharp.FirebaseClient(config);

			if (client == null)
			{
				MessageBox.Show("Shometing went wrong there!");
			}
		}

		public void InitalizeWebBrowser()
		{
			browser = new ChromiumWebBrowser("");
			browser.Dock = DockStyle.Fill;
			webBrowser.Controls.Add(browser);
		}

		private void RemoteRecive_Tick(object sender, EventArgs e)
		{
			RemoteRecive.Interval = 200;
			Recive();
			RemoteRecive.Stop();
		}

		private async void Recive()
		{

			try
			{
				FirebaseResponse response = await client.GetAsync(@"Actions/action");

				if(response.StatusCode == System.Net.HttpStatusCode.NotFound && response.StatusCode == System.Net.HttpStatusCode.BadRequest)
				{
					RemoteRecive.Interval = 1000;
				}
				else if (respold != response.Body)
				{
					respold = response.Body;

					if (response.Body.Contains("up"))
					{
						GoUp();
					}
					else if (response.Body.Contains("down"))
					{
						GoDown();
					}
					else if (response.Body.Contains("left"))
					{
						GoLeft();
					}
					else if (response.Body.Contains("right"))
					{
						GoRight();
					}
					else if (response.Body.Contains("enter"))
					{
						if (controllerControlling == "keyboard")
						{
							if (curentLetter == "OK")
							{
								Search(textBox1.Text);
								youTubeSearch.BringToFront();
								youTubeSearch.Visible = true;
								controllerControlling = "youtubeSearchSelection";
								virtualKeyboard.SendToBack();
								virtualKeyboard.Visible = false;
							}
							else
							{
								textBox1.Text += curentLetter;
							}
						}
						else if(controllerControlling == "youtubeSearchSelection")
						{
							videoSearchResultsOnDisplay[curentVideoSearchLine][curentVideoSearchRow].Play();
							controllerControlling = "playingVideo";
						}
						else if (controllerControlling == "playingVideo")
						{
							PauseAndCloseAdd();
						}
					}
					else if (response.Body.Contains("back"))
					{
						if (controllerControlling == "playingVideo")
						{
							webBrowser.SendToBack();
							webBrowser.Visible = false;
							browser.LoadHtml("");
							controllerControlling = "youtubeSearchSelection";
						}
						else if (controllerControlling == "youtubeSearchSelection")
						{
							youTubeSearch.SendToBack();
							youTubeSearch.Visible = false;
							browser.LoadHtml("");
							controllerControlling = "keyboard";
							virtualKeyboard.BringToFront();
							virtualKeyboard.Visible = true;
						}
					}
				}

				label1.Text = response.Body;
			}
			catch
			{
				RemoteRecive.Interval = 1000;
			}

			RemoteRecive.Start();
		}

		private void PauseAndCloseAdd()
		{
			SetMousePos(1085, 735);
			mouse_event(MOUSEEVENTF_LEFTDOWN, 1085, 735, 0, 0);
			mouse_event(MOUSEEVENTF_LEFTUP, 1085, 735, 0, 0);

			SendKeys.Send("{TAB}");
			SendKeys.Send("{TAB}");
			SendKeys.Send("{TAB}");
			SendKeys.Send(" ");
		}

		private void SkipAdd()
		{
			SetMousePos(1311, 668);
			mouse_event(MOUSEEVENTF_LEFTDOWN, 1311, 668, 0, 0);
			mouse_event(MOUSEEVENTF_LEFTUP, 1311, 668, 0, 0);
		}

		private void Main_Load(object sender, EventArgs e)
		{
			//Cursor.Hide();
			InitalizeDatabase();
			InitalizeWebBrowser();

			line1.Add(button1);
			line1.Add(button2);
			line1.Add(button3);
			line1.Add(button4);
			line1.Add(button5);
			line1.Add(button6);
			line1.Add(button7);
			line1.Add(button9);
			line1.Add(button11);
			line1.Add(button8);
			line1.Add(button10);
			line1.Add(button12);

			line2.Add(button18);
			line2.Add(button23);

			line2.Add(button29);
			line2.Add(button20);
			line2.Add(button25);
			line2.Add(button31);
			line2.Add(button19);
			line2.Add(button24);
			line2.Add(button30);
			line2.Add(button21);
			line2.Add(button26);
			line2.Add(button32);

			line3.Add(button13);
			line3.Add(button14);
			line3.Add(button15);
			line3.Add(button16);
			line3.Add(button17);
			line3.Add(button22);
			line3.Add(button27);
			line3.Add(button33);
			line3.Add(button28);
			line3.Add(button34);
			line3.Add(button35);
			line3.Add(button36);

			RemoteRecive.Start();
		}

		#region GoDirection
		private void GoRight()
		{
			try
			{
				if (controllerControlling == "keyboard")
				{
					if (curentLine == 1)
					{
						line1[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}
					else if (curentLine == 2)
					{
						line2[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}
					else if (curentLine == 3)
					{
						line3[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}

					if (curentRow + 1 > 11)
					{
						curentRow = 0;
					}
					else
					{
						curentRow++;
					}

					if (curentLine == 1)
					{
						line1[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line1[curentRow].Text;
					}
					else if (curentLine == 2)
					{
						line2[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line2[curentRow].Text;
					}
					else if (curentLine == 3)
					{
						line3[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line3[curentRow].Text;
					}
				}
				else if(controllerControlling == "youtubeSearchSelection")
				{
					if(curentVideoSearchRow + 1 < videoSearchResultsOnDisplay[curentVideoSearchLine].Count)
					{
						videoSearchResultsOnDisplay[curentVideoSearchLine][curentVideoSearchRow].BackColor = Color.FromArgb(50, 50, 50);

						VideoVisual curent = videoSearchResultsOnDisplay[curentVideoSearchLine][curentVideoSearchRow + 1];
						curent.BackColor = Color.Blue;
						curentVideoSearchRow++;
					}
				}
				else if (controllerControlling == "playingVideo")
				{
					SkipAdd();
				}
			}
			catch
			{

			}
		}

		private void GoLeft()
		{
			if (controllerControlling == "keyboard")
			{
				try
				{
					if (curentLine == 1)
					{
						line1[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}
					else if (curentLine == 2)
					{
						line2[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}
					else if (curentLine == 3)
					{
						line3[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}

					if (curentRow - 1 < 0)
					{
						curentRow = 11;
					}
					else
					{
						curentRow--;
					}

					if (curentLine == 1)
					{
						line1[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line1[curentRow].Text;
					}
					else if (curentLine == 2)
					{
						line2[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line2[curentRow].Text;
					}
					else if (curentLine == 3)
					{
						line3[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line3[curentRow].Text;
					}
				}
				catch
				{

				}
			}
			else if (controllerControlling == "youtubeSearchSelection")
			{
				if (curentVideoSearchRow - 1 >= 0)
				{
					videoSearchResultsOnDisplay[curentVideoSearchLine][curentVideoSearchRow].BackColor = Color.FromArgb(50, 50, 50);

					VideoVisual curent = videoSearchResultsOnDisplay[curentVideoSearchLine][curentVideoSearchRow - 1];
					curent.BackColor = Color.Blue;
					curentVideoSearchRow--;
				}
			}
		}

		private void GoDown()
		{
			if (controllerControlling == "keyboard")
			{
				try
				{
					if (curentLine == 1)
					{
						line1[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}
					else if (curentLine == 2)
					{
						line2[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}
					else if (curentLine == 3)
					{
						line3[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}

					if (curentLine + 1 > 3)
					{
						curentLine = 1;
					}
					else
					{
						curentLine++;
					}

					if (curentLine == 1)
					{
						line1[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line1[curentRow].Text;
					}
					else if (curentLine == 2)
					{
						line2[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line2[curentRow].Text;
					}
					else if (curentLine == 3)
					{
						line3[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line3[curentRow].Text;
					}
				}
				catch
				{

				}
			}
			else if (controllerControlling == "youtubeSearchSelection")
			{
				if (curentVideoSearchLine + 1 < videoSearchResultsOnDisplay.Count)
				{
					videoSearchResultsOnDisplay[curentVideoSearchLine][curentVideoSearchRow].BackColor = Color.FromArgb(50, 50, 50);

					VideoVisual curent = videoSearchResultsOnDisplay[curentVideoSearchLine + 1][curentVideoSearchRow];
					curent.BackColor = Color.Blue;
					curentVideoSearchLine++;
				}
			}
		}

		private void GoUp()
		{
			if (controllerControlling == "keyboard")
			{
				try
				{
					if (curentLine == 1)
					{
						line1[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}
					else if (curentLine == 2)
					{
						line2[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}
					else if (curentLine == 3)
					{
						line3[curentRow].BackColor = Color.FromArgb(30, 30, 30);
					}

					if (curentLine - 1 < 1)
					{
						curentLine = 3;
					}
					else
					{
						curentLine--;
					}


					if (curentLine == 1)
					{
						line1[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line1[curentRow].Text;
					}
					else if (curentLine == 2)
					{
						line2[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line2[curentRow].Text;
					}
					else if (curentLine == 3)
					{
						line3[curentRow].BackColor = Color.FromArgb(10, 10, 10);
						curentLetter = line3[curentRow].Text;
					}
				}
				catch
				{

				}
			}
			else if (controllerControlling == "youtubeSearchSelection")
			{
				if (curentVideoSearchLine - 1 >= 0)
				{
					videoSearchResultsOnDisplay[curentVideoSearchLine][curentVideoSearchRow].BackColor = Color.FromArgb(50, 50, 50);

					VideoVisual curent = videoSearchResultsOnDisplay[curentVideoSearchLine - 1][curentVideoSearchRow];
					curent.BackColor = Color.Blue;
					curentVideoSearchLine--;
				}
			}
		}
		#endregion

		private void label1_TextChanged(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void button31_Click(object sender, EventArgs e)
		{

		}

		private void button34_Click(object sender, EventArgs e)
		{

		}

		private async void Search(string keyword)
		{
			videoSearchResultsOnDisplay.Clear();
			flowLayoutPanel2.Controls.Clear();

			curentVideoSearchLine = 0;
			curentVideoSearchRow = 0;

			bool first = true;

			int maxRows = 6;
			List<VideoVisual> curentLine = new List<VideoVisual>();

			foreach (Video v in await new Search().Run(keyword))
			{
				VideoVisual vv = new VideoVisual();
				vv.title = v.Title;
				vv.author = v.Chanel;
				vv.thumbnail = v.Thumbnail;
				vv.link = v.Link;
				vv.play += Vv_play;

				if (first)
				{
					vv.BackColor = Color.Blue;
					first = false;
				}

				flowLayoutPanel2.Controls.Add(vv);
				curentLine.Add(vv);
				if(curentLine.Count >= maxRows)
				{
					videoSearchResultsOnDisplay.Add(new List<VideoVisual>(curentLine));
					curentLine.Clear();
				}
			}
		}

		private void Vv_play(object sender, EventArgs e)
		{
			VideoVisual self = sender as VideoVisual;

			browser.Load(self.link + "?autoplay=1");

			First.Start();

			webBrowser.BringToFront();
			webBrowser.Visible = true;
		}

		private void button38_Click(object sender, EventArgs e)
		{
			Search(textBox1.Text);
		}

		private void button36_Click(object sender, EventArgs e)
		{

		}

		private void First_Tick(object sender, EventArgs e)
		{
			First.Stop();
			First.Interval = 4000;
			browser.Focus();
			SendKeys.Send("f");
			if (firstYoutubeVideo)
			{
				firstYoutubeVideo = false;
				Second.Start();
			}
		}

		private void Second_Tick(object sender, EventArgs e)
		{
			Second.Stop();
			SetMousePos(779, 538);
			mouse_event(MOUSEEVENTF_LEFTDOWN, 779, 538, 0, 0);
			mouse_event(MOUSEEVENTF_LEFTUP, 779, 538, 0, 0);
			Third.Start();
		}

		private void Third_Tick(object sender, EventArgs e)
		{
			Third.Stop();
			SetMousePos(947, 630);
			mouse_event(MOUSEEVENTF_LEFTDOWN, 947, 630, 0, 0);
			mouse_event(MOUSEEVENTF_LEFTUP, 947, 630, 0, 0);

			SetMousePos(0, 320);
			mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 320, 0, 0);
			mouse_event(MOUSEEVENTF_LEFTUP, 0, 320, 0, 0);

			First.Interval = 2000;
			First.Start();
		}
	}
}
