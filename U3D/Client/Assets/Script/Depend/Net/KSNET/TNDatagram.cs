//---------------------------------------------
//            Tasharen Network
// Copyright © 2012-2013 Tasharen Entertainment
//---------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace KSNET
{
/// <summary>
/// Simple datagram container -- contains a data buffer and the address of where it came from (or where it's going).
/// </summary>

public struct Datagram
{
	public TNBuffer buffer;
	public IPEndPoint ip;
}
}