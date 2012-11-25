//
// SocialClient.cs
//
// Author:
//   Eric Maupin <me@ermau.com>
//
// Copyright (c) 2012 Eric Maupin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Net;
using System.Threading.Tasks;

namespace Tempest.Social
{
	public class SocialClient
		: LocalClient
	{
		public SocialClient (IClientConnection connection)
			: base (connection, MessageTypes.Reliable)
		{
			this.watchList = new WatchList (this);
			this.RegisterMessageHandler<RequestBuddyListMessage> (OnRequestBuddyListMessage);
		}

		/// <summary>
		/// The server has requested that you send your buddy list in full.
		/// </summary>
		/// <remarks>
		/// For simple match making servers that do not have a storage mechanism,
		/// it is necessary for the client to persist its own buddy list and resend
		/// it to the server on each connection.
		/// </remarks>
		public event EventHandler BuddyListRequested;

		public WatchList WatchList
		{
			get { return this.watchList; }
		}

		public Task<EndPoint> RequestEndPointAsync (string identity)
		{
			if (identity == null)
				throw new ArgumentNullException ("identity");

			return Connection.SendFor<EndPointResultMessage> (new EndPointRequestMessage { Identity = identity })
			                 .ContinueWith (t => t.Result.EndPoint);
		}

		private readonly WatchList watchList;

		private void OnRequestBuddyListMessage (MessageEventArgs<RequestBuddyListMessage> e)
		{
			OnBuddyListRequested();
		}

		private void OnBuddyListRequested ()
		{
			var handler = BuddyListRequested;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}
	}
}