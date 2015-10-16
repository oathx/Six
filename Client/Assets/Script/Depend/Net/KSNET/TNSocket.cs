//---------------------------------------------
//            Tasharen Network
// Copyright © 2012-2013 Tasharen Entertainment
//---------------------------------------------

using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;

namespace KSNET
{
	/// <summary>
	/// Common network communication-based logic: sending and receiving of data via TCP.
	/// </summary>
	
	public class TNSocket
	{
		public enum Stage
		{
			NotConnected,
			Connecting,
			Verifying,
			Connected,
		}
		
		/// <summary>
		/// Current connection stage.
		/// </summary>
		
		public Stage stage = Stage.NotConnected;
		
		/// <summary>
		/// IP end point of whomever we're connected to.
		/// </summary>
		
		public IPEndPoint tcpEndPoint;
		
		/// <summary>
		/// Timestamp of when we received the last message.
		/// </summary>
		
		public long lastReceivedTime = 0;
		public long lastSendTime = 0;
		
		/// <summary>
		/// How long to allow this player to go without packets before disconnecting them.
		/// This value is in milliseconds, so 1000 means 1 second.
		/// </summary>

		public const long TIME_OUT_TIME = 15000;
		
		// Incoming and outgoing queues
		protected Queue<TNBuffer> mIn = new Queue<TNBuffer>();
		protected Queue<TNBuffer> mOut = new Queue<TNBuffer>();
		
		// Buffer used for receiving incoming data
		protected byte[] mTemp = new byte[8192];
		public Socket mSocket;

		// Current incoming buffer
		protected TNBuffer mReceiveBuffer;
		protected int mExpected = 0;
		protected int mOffset = 0;

		protected bool mNoDelay = false;
		protected IPEndPoint mFallback;
		protected List<Socket> mConnecting = new List<Socket>();
		public int mIndex;

		//socket information
		public string serverName;
		public string ip;
		public int port;
		
		// Static as it's temporary
		static TNBuffer mBuffer;
		
		/// <summary>
		/// Whether the connection is currently active.
		/// </summary>
		
		public bool isConnected { get { return stage == Stage.Connected; } }
		
		/// <summary>
		/// Whether we are currently trying to establish a new connection.
		/// </summary>
		
		public bool isTryingToConnect { get { return mConnecting.Count != 0; } }

		/// <summary>
		/// Error Messages notification
		/// </summary>
		public delegate void OnError (string serverName, string message);
		public OnError onError;

		/// <summary>
		/// OnConnected notification..
		/// </summary>
		public delegate void OnConnected(string serverName);
		public OnConnected onConnected;

		/// <summary>
		/// OnDisConnected notification
		/// </summary>
		public delegate void OnDisConnected(string serverName, TNErrorCode code);
		public OnDisConnected onDisconnected;

		public delegate void OnDebug(string serverName, string message);
		public OnDebug onDebug;

		public delegate void OnReceivePack();
		public OnReceivePack onReceivePack;
	
		
		/// <summary>
		/// Enable or disable the Nagle's buffering algorithm (aka NO_DELAY flag).
		/// Enabling this flag will improve latency at the cost of increased bandwidth.
		/// http://en.wikipedia.org/wiki/Nagle's_algorithm
		/// </summary>
		
		public bool noDelay
		{
			get
			{
				return mNoDelay;
			}
			set
			{
				if (mNoDelay != value)
				{
					mNoDelay = value;
					mSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, mNoDelay);
				}
			}
		}
		
		/// <summary>
		/// Connected target's address.
		/// </summary>
		
		public string address { get { return (tcpEndPoint != null) ? tcpEndPoint.ToString() : "0.0.0.0:0"; } }
		
		/// <summary>
		/// Try to establish a connection with the specified address.
		/// </summary>
		
		public void Connect (IPEndPoint externalIP) { Connect(externalIP, null); }


		/// <summary>
		/// Try to establish a connection with speicified ip and port.
		/// </summary> 
		public void Connect()
		{
			this.Connect(Tools.ResolveEndPoint(ip, port));
		}
		
		/// <summary>
		/// Try to establish a connection with the specified remote destination.
		/// </summary>
		
		public void Connect (IPEndPoint externalIP, IPEndPoint internalIP)
		{
			Disconnect();
			
			// Some routers, like Asus RT-N66U don't support NAT Loopback, and connecting to an external IP
			// will connect to the router instead. So if it's a local IP, connect to it first.
			if (internalIP != null && Tools.GetSubnet(Tools.localAddress) == Tools.GetSubnet(internalIP.Address))
			{
				tcpEndPoint = internalIP;
				mFallback = externalIP;
			}
			else
			{
				tcpEndPoint = externalIP;
				mFallback = internalIP;
			}
			ConnectToTcpEndPoint();
		}
		
		/// <summary>
		/// Try to establish a connection with the current tcpEndPoint.
		/// </summary>
		
		bool ConnectToTcpEndPoint ()
		{
			if (tcpEndPoint != null)
			{
				stage = Stage.Connecting;
				
				try
				{
					lock (mConnecting)
					{
						mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						mConnecting.Add(mSocket);
					}
					
					IAsyncResult result = mSocket.BeginConnect(tcpEndPoint, OnConnectResult, mSocket);
					Thread th = new Thread(CancelConnect);
					th.Start(result);
					return true;
				}
				catch (System.Exception ex)
				{
					Error(ex.Message);
				}
			}
			else Error("Unable to resolve the specified address");
			return false;
		}
		
		/// <summary>
		/// Try to establish a connection with the fallback end point.
		/// </summary>
		
		bool ConnectToFallback ()
		{
			tcpEndPoint = mFallback;
			mFallback = null;
			return (tcpEndPoint != null) && ConnectToTcpEndPoint();
		}
		
		/// <summary>
		/// Default timeout on a connection attempt it something around 15 seconds, which is ridiculously long.
		/// </summary>
		
		void CancelConnect (object obj)
		{
			IAsyncResult result = (IAsyncResult)obj;
			
			if (result != null && !result.AsyncWaitHandle.WaitOne(5000, true))
			{
				try
				{
					Socket sock = (Socket)result.AsyncState;
					
					if (sock != null)
					{
						sock.Close();
						
						lock (mConnecting)
						{
							// Last active connection attempt
							if (mConnecting.Count > 0 && mConnecting[mConnecting.Count -1] == sock)
							{
								mSocket = null;
								
								if (!ConnectToFallback())
								{
									Error("Unable to connect");
									Close(false);
								}
							}
							mConnecting.Remove(sock);

							if (mSocket == null)
							{
								this.onDisconnected(this.serverName, TNErrorCode.TIMEOUT);
							}
						}
					}
				}
				catch (System.Exception) { }
			}
		}
		
		/// <summary>
		/// Connection attempt result.
		/// </summary>
		
		void OnConnectResult (IAsyncResult result)
		{
			Socket sock = (Socket)result.AsyncState;
			
			// Windows handles async sockets differently than other platforms, it seems.
			// If a socket is closed, OnConnectResult() is never called on Windows.
			// On the mac it does get called, however, and if the socket is used here
			// then a null exception gets thrown because the socket is not usable by this point.
			if (sock == null) return;
			
			if (mSocket != null && sock == mSocket)
			{
				bool success = true;
				string errMsg = "Failed to connect";
				
				try
				{
					sock.EndConnect(result);
				}
				catch (System.Exception ex)
				{
					if (sock == mSocket) mSocket = null;
					sock.Close();
					errMsg = ex.Message;
					success = false;
				}
				
				if (success)
				{
					stage = Stage.Connected;
					if (this.onConnected != null)
					{
						this.onConnected(this.serverName);
					}
					StartReceiving();
				}
				else if (!ConnectToFallback())
				{
					Error(errMsg);
					Close(false);
				}
			}
			
			// We are no longer trying to connect via this socket
			lock (mConnecting) mConnecting.Remove(sock);
		}
		
		/// <summary>
		/// Disconnect the player, freeing all resources.
		/// </summary>
		
		public void Disconnect ()
		{
			try
			{
				lock (mConnecting)
				{
					for (int i = mConnecting.Count; i > 0; )
					{
						Socket sock = mConnecting[--i];
						mConnecting.RemoveAt(i);
						if (sock != null) sock.Close();
					}
				}
				if (mSocket != null) Close(mSocket.Connected);
			}
			catch (System.Exception)
			{
				lock (mConnecting) mConnecting.Clear();
				mSocket = null;
			}
		}
		
		/// <summary>
		/// Close the connection.
		/// </summary>
		
		public void Close (bool notify)
		{
			stage = Stage.NotConnected;
			
			if (mReceiveBuffer != null)
			{
				mReceiveBuffer.Recycle();
				mReceiveBuffer = null;
			}
			
			if (mSocket != null)
			{
				try
				{
					if (mSocket.Connected) mSocket.Shutdown(SocketShutdown.Both);
						mSocket.Close();
				}
				catch (System.Exception) {}
				
				mSocket = null;
				
				if (notify)
				{
					if (this.onDisconnected != null)
					{
						this.onDisconnected(this.serverName, TNErrorCode.DISCONNECT);
					}
				}
			}
		}
		
		/// <summary>
		/// Release the buffers.
		/// </summary>
		
		public void Release ()
		{
			stage = Stage.NotConnected;
			
			if (mSocket != null)
			{
				try
				{
					if (mSocket.Connected) mSocket.Shutdown(SocketShutdown.Both);
					mSocket.Close();
				}
				catch (System.Exception) {}
				mSocket = null;
			}
			
			TNBuffer.Recycle(mIn);
			TNBuffer.Recycle(mOut);
		}


		/// <summary>
		/// Begin sending a new packet to the server.
		/// </summary>
		
		public BinaryWriter BeginSend (byte packetID)
		{
			mBuffer = TNBuffer.Create(false);
			return mBuffer.BeginPacket(packetID);
		}

		/// <summary>
		/// Begin sending a new packet to the server.
		/// </summary>

		public BinaryWriter BeginSend (int packetID)
		{
			mBuffer = TNBuffer.Create(false);
			return mBuffer.BeginPacket(packetID);
		} 
		
		/// <summary>
		/// Send the outgoing buffer.
		/// </summary>
		
		public void EndSend ()
		{
			mBuffer.EndPacket();
			SendTcpPacket(mBuffer);
			mBuffer = null;
		}
		
		/// <summary>
		/// Send the specified packet. Marks the buffer as used.
		/// </summary>
		
		public void SendTcpPacket (TNBuffer buffer)
		{
			buffer.MarkAsUsed();

			if (mSocket != null && mSocket.Connected)
			{
				buffer.BeginReading();
				
				lock (mOut)
				{
					mOut.Enqueue(buffer);
					
					if (mOut.Count == 1)
					{
						try
						{
							if (onDebug != null)
							{
								onDebug(this.serverName, "SendTcpPacket, size: " + buffer.size);
							}

							// If it's the first packet, let's begin the send process
							lastSendTime = DateTime.Now.Ticks / 10000;
							mSocket.BeginSend(buffer.buffer, buffer.position, buffer.size, SocketFlags.None, OnSend, buffer);
						}
						catch (System.Exception ex)
						{
							Error(ex.Message);
							Close(false);
							Release();
						}
					}
				}
			}
			else buffer.Recycle();
		}

	
		/// <summary>
		/// Sends the tcp package.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="msgs">Msgs.</param>
		public void SendTcpPacket(int packId, params object[] msgs)
		{
			TNPackWriter encoder = new TNPackWriter();
			TNBuffer buffer = TNBuffer.Create(false);
			buffer.BeginPacket(packId);
			encoder.encode(buffer, msgs);
			buffer.EndPacket();
			this.SendTcpPacket(buffer);
		}
	
		/// <summary>
		/// Send completion callback. Recycles the buffer.
		/// </summary>
		
		void OnSend (IAsyncResult result)
		{
			if (stage == Stage.NotConnected) return;

			lastSendTime = 0;
			int bytes;
			
			try
			{
				bytes = mSocket.EndSend(result);
			}
			catch (System.Exception ex)
			{
				bytes = 0;
				Close(true);
				Error(ex.Message);
				return;
			}
			
			lock (mOut)
			{
				// The buffer has been sent and can now be safely recycled
				mOut.Dequeue().Recycle();
				
				if (bytes > 0 && mSocket != null && mSocket.Connected)
				{
					// If there is another packet to send out, let's send it
					TNBuffer next = (mOut.Count == 0) ? null : mOut.Peek();
					if (next != null)
					{
						lastSendTime = DateTime.Now.Ticks / 10000;
						mSocket.BeginSend(next.buffer, next.position, next.size,   SocketFlags.None, OnSend, next);
					}
				}
				else Close(true);
			}
		}
		
		/// <summary>
		/// Start receiving incoming messages on the current socket.
		/// </summary>
		
		public void StartReceiving () { StartReceiving(null); }
		
		/// <summary>
		/// Start receiving incoming messages on the specified socket (for example socket accepted via Listen).
		/// </summary>
		
		public void StartReceiving (Socket socket)
		{
			if (socket != null)
			{
				Close(false);
				mSocket = socket;
			}
			
			if (mSocket != null && mSocket.Connected)
			{
				// We are not verifying the connection
				//stage = Stage.Verifying;
				
				// Save the timestamp
				lastReceivedTime = DateTime.Now.Ticks / 10000;
				
				// Save the address
				tcpEndPoint = (IPEndPoint)mSocket.RemoteEndPoint;

				// Queue up the read operation
				try
				{
					mSocket.BeginReceive(mTemp, 0, mTemp.Length, SocketFlags.None, OnReceive, null);
				}
				catch (System.Exception ex)
				{
					Error(ex.Message);
					Disconnect();
				}
			}
			else
			{
				Error("mSocket " + mSocket);
			}
		}
		
		/// <summary>
		/// Extract the first incoming packet.
		/// </summary>
		
		public bool ReceivePacket (out TNBuffer buffer)
		{
			if (mIn.Count != 0)
			{
				lock (mIn)
				{
					buffer = mIn.Dequeue();
					return true;
				}
			}
			buffer = null;
			return false;
		}


		/// <summary>
		/// Receive incoming data.
		/// </summary>
		
		void OnReceive (IAsyncResult result)
		{
			if (stage == Stage.NotConnected) return;
			int bytes = 0;
			
			try
			{
				bytes = mSocket.EndReceive(result);
			}
			catch (System.Exception ex)
			{
				Error(ex.Message);
				Disconnect();
				return;
			}
			lastReceivedTime = DateTime.Now.Ticks / 10000;
			
			if (bytes == 0)
			{
				Close(true);
			}
			else if (ProcessBuffer(bytes))
			{
				if (stage == Stage.NotConnected) return;
				
				try
				{
					// Queue up the next read operation
					mSocket.BeginReceive(mTemp, 0, mTemp.Length, SocketFlags.None, OnReceive, null);
				}
				catch (System.Exception ex)
				{
					Error(ex.Message);
					Close(false);
				}
			}
			else Close(true);
		}
		
		/// <summary>
		/// See if the received packet can be processed and split it up into different ones.
		/// </summary>
		
		bool ProcessBuffer (int bytes)
		{
			if (mReceiveBuffer == null)
			{
				// Create a new packet buffer
				mReceiveBuffer = TNBuffer.Create();
				mReceiveBuffer.BeginWriting(false).Write(mTemp, 0, bytes);
			}
			else
			{
				// Append this data to the end of the last used buffer
				mReceiveBuffer.BeginWriting(true).Write(mTemp, 0, bytes);
			}
			
			for (int available = mReceiveBuffer.size - mOffset; available >= 4; )
			{
				// Figure out the expected size of the packet
				if (mExpected == 0)
				{
					mExpected = mReceiveBuffer.PeekInt(mOffset);
					if (mExpected == -1) break;
					
					if (mExpected == 0)
					{
						Close(true);
						return false;
					}
				}
				
				// The first 4 bytes of any packet always contain the number of bytes in that packet
				available -= 4;
				
				// If the entire packet is present
				if (available == mExpected)
				{

					if (onDebug != null)
					{
						onDebug(this.serverName, "OnReceive, bytes: " + mExpected);
					}

					// Reset the position to the beginning of the packet
					mReceiveBuffer.BeginReading(mOffset + 4);
					
					// This packet is now ready to be processed
					lock (mIn) mIn.Enqueue(mReceiveBuffer);
					mReceiveBuffer = null;
					mExpected = 0;
					mOffset = 0;

					if (null != this.onReceivePack)
					{
						this.onReceivePack();
					}
					break;
				}
				else if (available > mExpected)
				{
					// There is more than one packet. Extract this packet fully.
					int realSize = mExpected + 4;
					TNBuffer temp = TNBuffer.Create();
					
					// Extract the packet and move past its size component
					temp.BeginWriting(false).Write(mReceiveBuffer.buffer, mOffset, realSize);
					temp.BeginReading(4);
					
					// This packet is now ready to be processed
					lock (mIn) mIn.Enqueue(temp);
					
					// Skip this packet
					available -= mExpected;
					mOffset += realSize;
					mExpected = 0;

					if (null != this.onReceivePack)
					{
						this.onReceivePack();
					}
				}
				else break;
			}
			return true;
		}
		
		/// <summary>
		/// Add an error packet to the incoming queue.
		/// </summary>
		
		public void Error (string error)
		{ 
			if (this.onError != null)
			{
				this.onError(this.serverName, error);
			}
		}
	}
}
