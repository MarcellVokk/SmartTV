using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib;

namespace SmartTV
{
	public partial class VideoVisual : UserControl
	{
		[Category("Events")]
		public event EventHandler play = null;

		public VideoVisual()
		{
			InitializeComponent();
		}

		public string link = "";

		public string title
		{
			get { return textBox1.Text; }
			set { textBox1.Text = value; }
		}

		public string author
		{
			get { return textBox2.Text; }
			set { textBox2.Text = value; }
		}

		public Image thumbnail
		{
			get { return pictureBox1.Image; }
			set { pictureBox1.Image = value; }
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}

		public void Play()
		{
			play?.Invoke(this, new EventArgs());
		}

		private void VideoVisual_BackColorChanged(object sender, EventArgs e)
		{
			textBox1.BackColor = BackColor;
			textBox2.BackColor = BackColor;
			pictureBox1.BackColor = BackColor;
			panel1.BackColor = BackColor;
			panel1.Refresh();
			pictureBox1.Refresh();
			textBox1.Refresh();
			textBox2.Refresh();
		}
	}
}
