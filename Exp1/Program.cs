using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Exp1
{
	internal class Program
	{
		static private StreamReader? reader_;

		static void Main(string[] args)
		{
			reader_ = File.OpenText(args[0]);

			Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Bind(new IPEndPoint(IPAddress.Any, 80));
			socket.Listen(100);
			socket.BeginAccept(new AsyncCallback(OnAccept), socket);
			Console.ReadKey();
		}

		static void OnAccept(IAsyncResult result)
		{
			Socket? socket = result.AsyncState as Socket;
			Socket client = socket.EndAccept(result);

			byte[] buffer = new byte[1024 * 32];
			int received = client.Receive(buffer);
			string quest = Encoding.UTF8.GetString(buffer, 0, received);

			Resolve(quest, client);

			socket.BeginAccept(new AsyncCallback(OnAccept), socket);
		}

		static void Resolve(string quest, Socket client)
		{
			Console.WriteLine(quest);
			string url = quest.Split(' ')[1];

			int paramters_begin = url.IndexOf('?');
			string path = url[1..((paramters_begin == -1)?(^0):(paramters_begin))];

			string paramters = url[((paramters_begin == -1)?(^0):(paramters_begin))..^0];

			byte[] buffer = new byte[1024 * 32];
			client.Send(Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n"));
			string? s;
			while ((s = reader_.ReadLine()) != null)
			{
				client.Send(Encoding.UTF8.GetBytes(s));
			}
			client.Close();
		}
	}
}
