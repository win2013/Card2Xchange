using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;


class bcard
{
    public string path;
	public bcard(string n)
	{
        path=n;
	}
}

class user
{
    public string name;
	public string passwd;
	public SortedList businessCards;
    public bcard myCard;

	public user(string n, string nCard, string passwd)
	{
		 myCard= new bcard(nCard);
		 this.passwd=passwd;
		 this.name=n; 
         businessCards=new SortedList();
		 businessCards.Add(name, myCard);
	}

	public void addBCard(string name, bcard card)
	{
		if (!(businessCards.ContainsKey(name))){
			businessCards.Add(name, card);
		}
	}
}

namespace ExchangeCards
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class cardXchange : System.Windows.Forms.Form
	{

		private SortedList users;
		private SortedList usersActive;

		private System.Windows.Forms.Label status;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public Thread sampleTcpThread;
		public TcpListener tcpListener;
		public const int TCPPORT      =5101;
		public const int TCPPORTSERVER=5100;
		public string WebPath = "c:\\inetPub\\wwwroot\\phoneomena";
       
		public void StartThreadServer()
		{
			try
			{
				//Starting the TCP Listener thread.
				tcpListener = new TcpListener(TCPPORTSERVER);
				tcpListener.Start();
				this.textBox1.Text=this.textBox1.Text+" Starting server ";
				sampleTcpThread = new Thread(new ThreadStart(Listen));
				sampleTcpThread.Start();
								
			}
			catch (Exception e)
			{
				this.textBox1.Text=this.textBox1.Text+"An TCP Exception has occurred!" + e.ToString();
				sampleTcpThread.Abort();
			}
		}


		public void InitializeUsers()
		{
			// users.Add("bill  ", new user("Bill Gates", "BillGates.png","123"));
			// users.Add("sumi  ", new user("Sumi Helal", "SumiPhoneomena.png","123"));
			// users.Add("edwin ", new user("Edwin Hernandez", "edwinPhoneomena.png","123"));
			// users.Add("harold", new user("Harold", "HaroldNextel.png","123"));
			users=new SortedList();
			usersActive=new SortedList();
			users.Add("bill",   "BillGates.png");
			users.Add("sumi",   "SumiPhoneomena.png");
			users.Add("edwin",  "edwinPhoneomena.png");
			users.Add("harol", "HaroldNextel.png");
		}

		public cardXchange()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.StartThreadServer();
			InitializeUsers();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.status = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// status
			// 
			this.status.Location = new System.Drawing.Point(24, 232);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(240, 16);
			this.status.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(24, 40);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(216, 168);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Log:";
			// 
			// cardXchange
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label1,
																		  this.textBox1,
																		  this.status});
			this.Name = "cardXchange";
			this.Text = "CardXchange Server";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new cardXchange());
		}

 
		public void Listen()
		{
			while (true)
			{
				//Create an instance of TcpListener to listen for TCP connection.
				Socket socket = this.tcpListener.AcceptSocket();					
				if (socket.Connected)
				{				
					//Program blocks on Accept() until a client connects.
					this.textBox1.Text=this.textBox1.Text+" SampleClient is connected through TCP \n";
					try
					{
						NetworkStream networkStream = new NetworkStream(socket);
						StreamWriter streamwriter = new StreamWriter(networkStream);
						StreamReader streamReader = new StreamReader(networkStream);
						string line  = streamReader.ReadLine();
						//streamwriter.Write("OK");
						PacketProcess(line, socket.RemoteEndPoint.ToString(), socket);						
						streamwriter.Flush();
						socket.Close();
					}
					catch (Exception e ) 
					{
						this.textBox1.Text=this.textBox1.Text+" "+e.ToString();
					}
				}
			}
		}
		
		public void PacketProcess(string input, string remoteIP, Socket socket)
		{
			this.textBox1.Text=this.textBox1.Text+" Processing :-> "+ input ;
			this.textBox1.Text=this.textBox1.Text+Environment.NewLine+" from : "+remoteIP+ Environment.NewLine;
			NetworkStream networkStream = new NetworkStream(socket);						
			StreamWriter streamwriter = new StreamWriter(networkStream);
			StreamReader streamReader = new StreamReader(networkStream);
						
			if (input.StartsWith("Init"))
			{
				this.textBox1.Text=this.textBox1.Text+input.Substring(5,input.Length-5);
				this.usersActive.Add(input.Substring(5, input.Length-5), remoteIP);
				this.textBox1.Text=this.textBox1.Text+" Active users : "+Environment.NewLine;
				for (int j=1; j<=usersActive.Count; j++)
				{
					this.textBox1.Text=this.textBox1.Text+ (string) usersActive.GetByIndex(j-1)+"  "+usersActive.GetKey(j-1)+Environment.NewLine;
				}
				streamwriter.Write("OK");
				streamwriter.Flush();
			}

			if (input.StartsWith("Close"))
			{
				this.usersActive.Remove(input.Substring(5,input.Length-5));
				this.textBox1.Text=this.textBox1.Text+" Active users : "+Environment.NewLine;
				for (int j=1; j<=usersActive.Count; j++)
				{
					this.textBox1.Text=this.textBox1.Text+ (string) usersActive.GetByIndex(j-1)+"  "+usersActive.GetKey(j-1)+Environment.NewLine;
				}		
 				streamwriter.Write("OK");
                streamwriter.Flush();
			}

			if (input.StartsWith("GetCard"))
			{
				string paramStr=input.Substring(7, input.Length-7);
				string resultStr=(string) users.GetByIndex(users.IndexOfKey(paramStr));
				streamwriter.Write(paramStr);				
				streamwriter.Flush();
			}

			if (input.StartsWith("Exchange"))
			{
				streamwriter.WriteLine("OK");
				streamwriter.Flush();
				string line  = streamReader.ReadLine();
				if (line.ToUpper()!="NEXT")
					textBox1.Text=textBox1.Text+"Error receiving start";

				for (int i=0; i<=this.usersActive.Count-1; i++)
				{  
					textBox1.Text=textBox1.Text+"Exchange for user:"+input.Substring(8, input.Length-8)+"  "+Environment.NewLine;
					if (!usersActive.GetKey(i).ToString().Equals(input.Substring(8,input.Length-8)))
					{
						// Copies the file to the destination in the directory of the user.
						FileInfo fi = new FileInfo(this.WebPath+"\\"+(string) users.GetByIndex(users.IndexOfKey(usersActive.GetKey(i))));
						try 
						{
							fi.CopyTo(this.WebPath+"\\"+usersActive.GetKey(i).ToString()+"\\"+fi.Name, true);					
							textBox1.Text=textBox1.Text+" ->"+fi.Name+Environment.NewLine;
						}
						catch (Exception e)
						{
							this.textBox1.Text=this.textBox1.Text+e.ToString()+Environment.NewLine; 
						}
						streamwriter.WriteLine(usersActive.GetKey(i).ToString()+":"+fi.Name);
						streamwriter.Flush();			
						line  = streamReader.ReadLine();
			            if (line.ToUpper()!="NEXT")
							i=usersActive.Count+1;
					}
				}	
				streamwriter.WriteLine("OK");
				streamwriter.Flush();
			}
 
		}
	
	}
}
