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
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Tempest.Social
{
	public class ConnectRequestEventArgs
		: EventArgs
	{
		public ConnectRequestEventArgs (Person person, EndPoint endPoint, bool youreHosting)
		{
			if (person == null)
				throw new ArgumentNullException ("person");
			if (endPoint == null)
				throw new ArgumentNullException ("endPoint");

			Person = person;
			EndPoint = endPoint;
			YoureHosting = youreHosting;
		}

		/// <summary>
		/// Gets the person you're to connect to.
		/// </summary>
		public Person Person
		{
			get;
			private set;
		}

		/// <summary>
		/// The endpoint to connect to or receive a connection from.
		/// </summary>
		public EndPoint EndPoint
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets whether or not the server picked you as the host.
		/// </summary>
		public bool YoureHosting
		{
			get;
			private set;
		}
	}

	public class SocialClient
		: LocalClient
	{
		public SocialClient (IClientConnection connection, Person persona)
			: base (connection, MessageTypes.Reliable)
		{
			if (persona == null)
				throw new ArgumentNullException ("persona");

			this.watchList = new WatchList (this);
			this.persona = persona;

			this.RegisterMessageHandler<RequestBuddyListMessage> (OnRequestBuddyListMessage);
			this.RegisterMessageHandler<ConnectToMessage> (OnConnectToMessage);
		}

		/// <summary>
		/// The server has requested that you send your buddy list in full.
		/// </summary>
		/// <remarks>
		/// <para>For simple match making servers that do not have a storage mechanism,
		/// it is necessary for the client to persist its own buddy list and resend
		/// it to the server on each connection.</para>
		/// <para>This event is also an indication that you should record your buddy list
		/// locally.</para>
		/// </remarks>
		public event EventHandler WatchListRequested;

		/// <summary>
		/// A connect request was received.
		/// </summary>
		public event EventHandler<ConnectRequestEventArgs> ConnectionRequest;

		public Person Persona
		{
			get { return this.persona; }
		}

		public WatchList WatchList
		{
			get { return this.watchList; }
		}

		public async Task<ConnectResult> RequestEndPointAsync (string identity)
		{
			if (identity == null)
				throw new ArgumentNullException ("identity");

			var response = await Connection.SendFor<ConnectResultMessage> (new ConnectRequestMessage { Identity = identity }).ConfigureAwait (false);
			return response.Result;
		}

		/// <summary>
		/// Searches for users with a similar <paramref name="nickname"/>.
		/// </summary>
		/// <param name="nickname">The nickname to search for.</param>
		/// <returns>Any users that at least partially match the nickname.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="nickname"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException"><paramref name="nickname"/> is empty.</exception>
		public async Task<IEnumerable<Person>> SearchAsync (string nickname)
		{
			if (nickname == null)
				throw new ArgumentNullException ("nickname");
			if (nickname.Trim() == String.Empty)
				throw new ArgumentException ("nickname can not be empty", "nickname");

			var response = await Connection.SendFor<SearchResultMessage> (new SearchMessage { Nickname = nickname }).ConfigureAwait (false);
			return response.Results;
		}

		private readonly WatchList watchList;
		private readonly Person persona;

		protected override void OnConnected (ClientConnectionEventArgs e)
		{
			Connection.Send (new PersonMessage { Person = Persona });

			base.OnConnected (e);
		}

		private void OnRequestBuddyListMessage (MessageEventArgs<RequestBuddyListMessage> e)
		{
			OnBuddyListRequested();
		}

		private void OnConnectToMessage (MessageEventArgs<ConnectToMessage> e)
		{
			Person person;
			if (!WatchList.TryGetPerson (e.Message.Id, out person))
				return;

			OnConnectionRequest (new ConnectRequestEventArgs (person, e.Message.EndPoint, e.Message.YoureHosting));
		}

		private void OnBuddyListRequested ()
		{
			var handler = this.WatchListRequested;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		private void OnConnectionRequest (ConnectRequestEventArgs e)
		{
			var handler = ConnectionRequest;
			if (handler != null)
				handler (this, e);
		}
	}
}