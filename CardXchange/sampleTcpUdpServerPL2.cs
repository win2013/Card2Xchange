/* Project  : Simple Multi-threaded TCP/UDP Server v2
 * Author   : Patrick Lam
 * Date     : 09/19/2001
 * Brief    : The simple multi-threaded TCP/UDP Server v2 does the same thing as v1.  What
 *            it intends to demonstrate is the amount of code you can save by using TcpListener
 *            instead of the traditional raw socket implementation (The UDP part is still
 *            the same.  When you compare the following code with v1, you will see the 
 *            difference.
 * Usage    : sampleTcpUdpServer2
 */
namespace sampleTcpUdpServer2
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading;
	
	public class SampleTcpUdpServer2
	{
		private const int sampleTcpPort = 4567;
		private const int sampleUdpPort = 4568;
		public Thread sampleTcpThread, sampleUdpThread;
		
		public SampleTcpUdpServer2()
		{
			try
			{
				//Starting the TCP Listener thread.
				sampleTcpThread = new Thread(new ThreadStart(StartListen2));
				sampleTcpThread.Start();
				Console.WriteLine("Started SampleTcpUdpServer's TCP Listener Thread!\n");
			}
			catch (Exception e)
			{
				Console.WriteLine("An TCP Exception has occurred!" + e.ToString());
				sampleTcpThread.Abort();
			}
			
			try
			{		
				//Starting the UDP Server thread.
				sampleUdpThread = new Thread(new ThreadStart(StartReceiveFrom2));
				sampleUdpThread.Start();
				Console.WriteLine("Started SampleTcpUdpServer's UDP Receiver Thread!\n");
			}
			catch (Exception e)
			{
				Console.WriteLine("An UDP Exception has occurred!" + e.ToString());
				sampleUdpThread.Abort();
			}
		}
		
		public static void Main(String[] argv)
		{
			SampleTcpUdpServer2 sts = new SampleTcpUdpServer2();
		}
		
		public void StartListen2()
		{
			//Create an instance of TcpListener to listen for TCP connection.
			TcpListener tcpListener = new TcpListener(sampleTcpPort);

			try 
			{
				while (true)
				{
					tcpListener.Start();
	 
					//Program blocks on Accept() until a client connects.
					Socket soTcp = tcpListener.AcceptSocket();

					Console.WriteLine("SampleClient is connected through TCP.");
					Byte[] received = new Byte[512];
					int bytesReceived = soTcp.Receive(received, received.Length, 0);
					
					String dataReceived = System.Text.Encoding.ASCII.GetString(received);
					Console.WriteLine(dataReceived);

					String returningString = "The Server got your message through TCP: " +
						dataReceived;
									
					Byte[] returningByte = System.Text.Encoding.ASCII.GetBytes(returningString.ToCharArray());
						
					//Returning a confirmation string back to the client.				
					soTcp.Send(returningByte, returningByte.Length, 0);

					tcpListener.Stop();				
				}
			}
			catch (SocketException se)
			{
				Console.WriteLine("A Socket Exception has occurred!" + se.ToString());
			}
		}
		
		public void StartReceiveFrom2()
		{
			
			IPHostEntry localHostEntry;
		
			try
			{
				//Create a UDP socket.
				Socket soUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				
				try
				{
					localHostEntry = Dns.GetHostByName(Dns.GetHostName());
				}
				catch(Exception)
				{
					Console.WriteLine("Local Host not found"); // fail
					return ;
				}
				
			
				IPEndPoint localIpEndPoint = new IPEndPoint(localHostEntry.AddressList[0], sampleUdpPort);
				
				soUdp.Bind(localIpEndPoint);
			
				while (true)
				{
					Byte[] received = new Byte[256];
					IPEndPoint tmpIpEndPoint = new IPEndPoint(localHostEntry.AddressList[0], sampleUdpPort);

					EndPoint remoteEP = (tmpIpEndPoint);

					int bytesReceived = soUdp.ReceiveFrom(received, ref remoteEP);

					String dataReceived = System.Text.Encoding.ASCII.GetString(received);

					Console.WriteLine("SampleClient is connected through UDP.");

					Console.WriteLine(dataReceived);
				
					String returningString = "The Server got your message through UDP:" + dataReceived;
						
					Byte[] returningByte = System.Text.Encoding.ASCII.GetBytes(returningString.ToCharArray());
						
					soUdp.SendTo(returningByte, remoteEP);
				}
					
			}
			catch (SocketException se)
			{
				Console.WriteLine("A Socket Exception has occurred!" + se.ToString());
			}
				
		}
	}
}
