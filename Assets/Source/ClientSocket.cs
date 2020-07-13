
//TODO:接收数据。接收匹配数据。接收战斗数据。需不需要分包

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

public class ClientSocket
{
	private Socket socket;
	public bool isConnected = false; //是否连接到服务器
	public bool failConnected = false; //尝试连接服务器但失败

	private static string pattern = @"\{([^\{\}]*)\}";

	public ClientSocket()
	{
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	}

	/// <summary>
	/// 连接服务器
	/// </summary>
	/// <param name="ip"></param>
	/// <param name="port"></param>
	public void ConnectServer(string ip, int port)
	{
		IPAddress serverIp = IPAddress.Parse(ip);
		IPEndPoint ipEnd = new IPEndPoint(serverIp, port);

		try
		{
			socket.Connect(ipEnd);
			isConnected = true;
			failConnected = false;
			Debug.Log("连接服务器成功");
		}
		catch (Exception)
		{
			isConnected = false;
			failConnected = true;
			Debug.Log("连接服务器失败");
			return;
		}

		//开始线程持续监听服务器消息
		Thread thread = new Thread(Recive);
		//设置为后台线程
		thread.IsBackground = true;
		thread.Start(socket);
	}

	/// <summary>
	/// 使用域名连接服务器
	/// </summary>
	/// <param name="host"></param>
	/// <param name="port"></param>
	public void ConnectServerByHost(string host,int port)
	{
		IPHostEntry iPHostEntry = Dns.GetHostEntry(host);
		IPAddress serverIp = null;
		if (iPHostEntry != null && iPHostEntry.AddressList != null && iPHostEntry.AddressList.Length > 0)
		{
			serverIp = iPHostEntry.AddressList[0];
		}
		else
		{
			Debug.LogError("解析域名出错啦");
			return;
		}

		IPEndPoint ipEnd = new IPEndPoint(serverIp, port);

		try
		{
			socket.Connect(ipEnd);
			isConnected = true;
			failConnected = false;
			Debug.Log("连接服务器成功");
		}
		catch (Exception)
		{
			isConnected = false;
			failConnected = true;
			Debug.Log("连接服务器失败");
			return;
		}

		//开始线程持续监听服务器消息
		Thread thread = new Thread(Recive);
		//设置为后台线程
		thread.IsBackground = true;
		thread.Start(socket);
	}

	static void Recive(object o)
	{
		var socket = o as Socket;

		while (true)
		{
			byte[] buffer = new byte[1024];
			var len = socket.Receive(buffer);
			if (len == 0)
			{
				break;
			}
			var str = Encoding.UTF8.GetString(buffer, 0, len);
			//Debug.Log("接收到的数据：" + str);
			
			//解析数据
			if (GameData.GD_isGameReady)
			{
				List<GameCtrlMsg> gameCtrlMsgs = new List<GameCtrlMsg>();
				MatchCollection matchCollection = Regex.Matches(str, pattern);
				for (int i = 0; i < matchCollection.Count; i++)
				{
					//Debug.Log("解析数据" + i + matchCollection[i].Value);
					if (matchCollection[i].Value == "{}")
					{
						//空帧处理
						GameCtrlMsg gameCtrlMsg = new GameCtrlMsg();
						gameCtrlMsgs.Add(gameCtrlMsg);
					}
					else
					{
						gameCtrlMsgs.Add(GameData.jsonUnpacking(matchCollection[i].Value));
					}
				}

				lock (GameData.GD_gameCtrlMsgQueueLock)
				{
					//5.29 修改
					if(matchCollection.Count != 0)
					{
						GameData.GD_gameCtrlMsgQueue.Enqueue(gameCtrlMsgs);
					}
				}
				//5.29 添加
				if(matchCollection.Count == 0)
				{
					if(str == "1")
					{
						GameData.GD_playerId = 0;
						GameData.GD_isGameCanGo = true;
					}else if(str == "2")
					{
						GameData.GD_playerId = 1;
						GameData.GD_isGameCanGo = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// 发送数据
	/// </summary>
	/// <param name="message"></param>
	public void sendMessage(string message)
	{
		message = message + "\n";
		var buffer = Encoding.UTF8.GetBytes(message);
		var temp = socket.Send(buffer);
	}

	public void sendReadySign()
	{
		string str = "ready";
		var buffer = Encoding.UTF8.GetBytes(str);
		var temp = socket.Send(buffer);
	}

	public void closeSocket()
	{
		try
		{
			socket.Shutdown(SocketShutdown.Both);
			Debug.Log("shutdown success");
		}
		finally
		{
			Debug.Log("finally it done");
			socket.Close();
		}
	}
}
