// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
// 12/23/15 3:55 PM    
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2013-2015 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.transport {
	
	using ILOG.J2CsMapping.NIO;
	using ILOG.J2CsMapping.NIO.Channels;
	using ILOG.J2CsMapping.Util;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.encoding;
	
	public class TcpTransport : Transport {
		public TcpTransport() {
			this.inputBuffer_ = ILOG.J2CsMapping.NIO.ByteBuffer.allocate(net.named_data.jndn.util.Common.MAX_NDN_PACKET_SIZE);
		}
		/// <summary>
		/// A TcpTransport.ConnectionInfo extends Transport.ConnectionInfo to hold
		/// the host and port info for the TCP connection.
		/// </summary>
		///
		public class ConnectionInfo : Transport.ConnectionInfo {
			/// <summary>
			/// Create a ConnectionInfo with the given host and port.
			/// </summary>
			///
			/// <param name="host">The host for the connection.</param>
			/// <param name="port">The port number for the connection.</param>
			public ConnectionInfo(String host, int port) {
				host_ = host;
				port_ = port;
			}
	
			/// <summary>
			/// Create a ConnectionInfo with the given host and default port 6363.
			/// </summary>
			///
			/// <param name="host">The host for the connection.</param>
			public ConnectionInfo(String host) {
				host_ = host;
				port_ = 6363;
			}
	
			/// <summary>
			/// Get the host given to the constructor.
			/// </summary>
			///
			/// <returns>The host.</returns>
			public String getHost() {
				return host_;
			}
	
			/// <summary>
			/// Get the port given to the constructor.
			/// </summary>
			///
			/// <returns>The port number.</returns>
			public int getPort() {
				return port_;
			}
	
			private readonly String host_;
			private readonly int port_;
		}
	
		/// <summary>
		/// Determine whether this transport connecting according to connectionInfo is
		/// to a node on the current machine; results are cached. According to
		/// http://redmine.named-data.net/projects/nfd/wiki/ScopeControl#local-face,
		/// TCP transports with a loopback address are local. If connectionInfo
		/// contains a host name, InetAddress will do a blocking DNS lookup; otherwise
		/// it will parse the IP address and examine the first octet to determine if
		/// it is a loopback address (e.g. first octet == 127).
		/// </summary>
		///
		/// <param name="connectionInfo">A TcpTransport.ConnectionInfo with the host to check.</param>
		/// <returns>True if the host is local, false if not.</returns>
		/// <exception cref="System.IO.IOException"></exception>
		public override bool isLocal(Transport.ConnectionInfo connectionInfo) {
			if (connectionInfo_ == null
					|| !((TcpTransport.ConnectionInfo ) connectionInfo).getHost().equals(
							connectionInfo_.getHost())) {
				isLocal_ = getIsLocal(((TcpTransport.ConnectionInfo ) connectionInfo).getHost());
				connectionInfo_ = (TcpTransport.ConnectionInfo ) connectionInfo;
			}
			return isLocal_;
		}
	
		/// <summary>
		/// Override to return false since connect does not need to use the onConnected
		/// callback.
		/// </summary>
		///
		/// <returns>False.</returns>
		public override bool isAsync() {
			return false;
		}
	
		/// <summary>
		/// Connect according to the info in ConnectionInfo, and use elementListener.
		/// </summary>
		///
		/// <param name="connectionInfo">A TcpTransport.ConnectionInfo.</param>
		/// <param name="elementListener"></param>
		/// <param name="onConnected"></param>
		/// <exception cref="IOException">For I/O error.</exception>
		public override void connect(Transport.ConnectionInfo connectionInfo,
				ElementListener elementListener, IRunnable onConnected) {
			close();
	
			channel_ = ILOG.J2CsMapping.NIO.Channels.SocketChannel.open(new InetSocketAddress(
					((TcpTransport.ConnectionInfo ) connectionInfo).getHost(),
					((TcpTransport.ConnectionInfo ) connectionInfo).getPort()));
			channel_.configureBlocking(false);
	
			elementReader_ = new ElementReader(elementListener);
	
			if (onConnected != null)
				onConnected.run();
		}
	
		/// <summary>
		/// Set data to the host
		/// </summary>
		///
		/// <param name="data"></param>
		/// <exception cref="IOException">For I/O error.</exception>
		public override void send(ByteBuffer data) {
			if (channel_ == null)
				throw new IOException(
						"Cannot send because the socket is not open.  Use connect.");
	
			// Save and restore the position.
			int savePosition = data.position();
			try {
				while (data.hasRemaining())
					channel_.write(data);
			} finally {
				data.position(savePosition);
			}
		}
	
		/// <summary>
		/// Process any data to receive.  For each element received, call
		/// elementListener.onReceivedElement.
		/// This is non-blocking and will return immediately if there is no data to
		/// receive. You should normally not call this directly since it is called by
		/// Face.processEvents.
		/// If you call this from an main event loop, you may want to catch and
		/// log/disregard all exceptions.
		/// </summary>
		///
		/// <exception cref="IOException">For I/O error.</exception>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public override void processEvents() {
			if (!getIsConnected())
				return;
	
			while (true) {
				inputBuffer_.limit(inputBuffer_.capacity());
				inputBuffer_.position(0);
				int bytesRead = channel_.read(inputBuffer_);
				if (bytesRead <= 0)
					return;
	
				inputBuffer_.flip();
				elementReader_.onReceivedData(inputBuffer_);
			}
		}
	
		/// <summary>
		/// Check if the transport is connected.
		/// </summary>
		///
		/// <returns>True if connected.</returns>
		public override bool getIsConnected() {
			if (channel_ == null)
				return false;
	
			return channel_.isConnected();
		}
	
		/// <summary>
		/// Close the connection.  If not connected, this does nothing.
		/// </summary>
		///
		/// <exception cref="IOException">For I/O error.</exception>
		public override void close() {
			if (channel_ != null) {
				if (channel_.isConnected())
					channel_.close();
				channel_ = null;
			}
		}
	
		/// <summary>
		/// A static method to determine whether the host is on the current machine.
		/// Results are not cached. According to
		/// http://redmine.named-data.net/projects/nfd/wiki/ScopeControl#local-face,
		/// TCP transports with a loopback address are local. If connectionInfo
		/// contains a host name, InetAddress will do a blocking DNS lookup; otherwise
		/// it will parse the IP address and examine the first octet to determine if
		/// it is a loopback address (e.g. first octet == 127).
		/// </summary>
		///
		/// <param name="host">The host to check.</param>
		/// <returns>True if the host is local, False if not.</returns>
		/// <exception cref="IOException"></exception>
		public static bool getIsLocal(String host) {
			InetAddress address = ILOG.J2CsMapping.Util.InetAddress.getByName(host);
			return address.isLoopbackAddress();
		}
	
		internal SocketChannel channel_;
		internal ByteBuffer inputBuffer_;
		// TODO: This belongs in the socket listener.
		private ElementReader elementReader_;
		private TcpTransport.ConnectionInfo  connectionInfo_;
		private bool isLocal_;
	}
}
