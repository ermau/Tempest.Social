//
// SocialClient.cs
//
// Author:
//   Eric Maupin <me@ermau.com>
//
// Copyright (c) 2012-2013 Eric Maupin
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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Tempest.Social
{
	public class SocialClient
		: TempestClient
	{
		public SocialClient (IClientConnection connection, Person persona)
			: base (connection, MessageTypes.Reliable)
		{
			if (persona == null)
				throw new ArgumentNullException ("persona");

			this.watchList = new WatchList (this);
			this.persona = persona;
			this.persona.PropertyChanged += OnPersonaPropertyChanged;

			this.RegisterMessageHandler<RequestBuddyListMessage> (OnRequestBuddyListMessage);
			this.RegisterMessageHandler<GroupInviteMessage> (OnGroupInviteMessage);
			this.RegisterMessageHandler<GroupUpdateMessage> (OnGroupUpdatedMessage);
			this.RegisterMessageHandler<TextMessage> (OnTextMessage);
			this.RegisterMessageHandler<ForwardMessage> (OnForwardMessage);
			this.RegisterMessageHandler<ConnectionInfoMessage> (OnConnectionInfoMessage);
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
		/// You have been invited to join a group.
		/// </summary>
		public event EventHandler<GroupInviteEventArgs> InvitedToGroup;

		/// <summary>
		/// You have received a text message in a group.
		/// </summary>
		public event EventHandler<TextMessageEventArgs> ReceivedTextMessage;

		/// <summary>
		/// Received a message that was forwarded from another client.
		/// </summary>
		public event EventHandler<ForwardedMessageEventArgs> ReceivedForwardedMessage;

		/// <summary>
		/// Gets your public target that the server sees.
		/// </summary>
		/// <remarks>This is useful for being able to forward messages with your IP to other clients.</remarks>
		public Target ServerTarget
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets your <see cref="Person"/>.
		/// </summary>
		public Person Persona
		{
			get { return this.persona; }
		}

		/// <summary>
		/// Gets your <see cref="WatchList"/>.
		/// </summary>
		public WatchList WatchList
		{
			get { return this.watchList; }
		}

		/// <summary>
		/// Gets the groups you're currently a part of.
		/// </summary>
		public IEnumerable<Group> Groups
		{
			get { return this.groups.Values; }
		}

		/// <summary>
		/// Asynchronously creates a group.
		/// </summary>
		/// <returns>The created <see cref="Group"/> or <c>null</c> if disconnected.</returns>
		public async Task<Group> CreateGroupAsync()
		{
			var response = await Connection.SendFor<GroupUpdateMessage> (new CreateGroupMessage()).ConfigureAwait (false);
			if (response == null)
				return null;

			lock (this.groups)
				this.groups.Add (response.Group.Id, response.Group);

			return response.Group;
		}

		/// <summary>
		/// Asynchronously leaves a group.
		/// </summary>
		/// <param name="group">The group to leave.</param>
		/// <exception cref="ArgumentNullException"><paramref name="group"/> is <c>null</c>.</exception>
		public async Task LeaveGroupAsync (Group group)
		{
			if (group == null)
				throw new ArgumentNullException ("group");

			await Connection.SendAsync (new LeaveGroupMessage { GroupId = group.Id }).ConfigureAwait (false);
			lock (this.groups)
				this.groups.Remove (group.Id);
		}

		/// <summary>
		/// Invites <paramref name="person"/> to the <paramref name="group"/>.
		/// </summary>
		/// <param name="group">The group to invite to.</param>
		/// <param name="person">The person to invite.</param>
		/// <returns>An <see cref="Invitation" /> containing the response or <c>null</c> if disconnected.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="group"/> or <paramref name="person"/> is <c>null</c>.</exception>
		public async Task<Invitation> InviteToGroupAsync (Group group, Person person)
		{
			if (group == null)
				throw new ArgumentNullException ("group");
			if (person == null)
				throw new ArgumentNullException ("person");

			var response = await Connection.SendFor<GroupInviteResponse> (
				new InviteToGroupMessage {
					Invitee = person.Identity,
					GroupId = group.Id
				}).ConfigureAwait (false);

			if (response == null)
				return null;

			return new Invitation (group, person, response.Response);
		}

		/// <summary>
		/// Searches for users with a similar <paramref name="nickname"/>.
		/// </summary>
		/// <param name="nickname">The nickname to search for.</param>
		/// <returns>Any users that at least partially match the nickname or <c>null</c> if disconnected.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="nickname"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException"><paramref name="nickname"/> is empty.</exception>
		public async Task<IEnumerable<Person>> SearchAsync (string nickname)
		{
			if (nickname == null)
				throw new ArgumentNullException ("nickname");
			if (nickname.Trim() == String.Empty)
				throw new ArgumentException ("nickname can not be empty", "nickname");

			var response = await Connection.SendFor<SearchResultMessage> (new SearchMessage { Nickname = nickname }).ConfigureAwait (false);
			if (response == null)
				return null;

			return response.Results;
		}

		/// <summary>
		/// Asynchronously sends a text message to a <paramref name="group"/>.
		/// </summary>
		/// <param name="group">The group to send the message to.</param>
		/// <param name="text">The message to send.</param>
		/// <exception cref="ArgumentNullException"><paramref name="group"/> or <paramref name="text"/> is <c>null</c>.</exception>
		public Task SendTextAsync (Group group, string text)
		{
			if (group == null)
				throw new ArgumentNullException ("group");
			if (text == null)
				throw new ArgumentNullException ("text");

			return Connection.SendAsync (new TextMessage {
				GroupId = group.Id,
				Text = text
			});
		}

		/// <summary>
		/// Asynchronously forwards a message payload to a another person.
		/// </summary>
		/// <param name="forwardTo">The person to forward the payload to.</param>
		/// <param name="message">The message information to forward.</param>
		/// <param name="payload">The actual message payload to forward.</param>
		/// <remarks>
		/// <para><paramref name="message"/> is passed along to obtain protocol and message type information. You must serialize
		/// the message's payload yourself into <paramref name="payload"/> and deserialize it the same way on the other end.</para>
		/// <para>This is done because only you really have access to the proper information to construct a <see cref="ISerializationContext"/>
		/// to use for writing the payload.</para>
		/// </remarks>
		public Task ForwardAsync (Person forwardTo, Message message, byte[] payload)
		{
			if (forwardTo == null)
				throw new ArgumentNullException ("forwardTo");
			if (message == null)
				throw new ArgumentNullException ("message");
			if (payload == null)
				throw new ArgumentNullException ("payload");

			var forward = new ForwardMessage {
				ForwardedProtocol = message.Protocol,
				MessageType = message.MessageType,
				Identity = forwardTo.Identity,
				Payload = payload
			};

			return Connection.SendAsync (forward);
		}

		private readonly WatchList watchList;
		private readonly Person persona;
		private readonly ObservableDictionary<int, Group> groups = new ObservableDictionary<int, Group>();

		private void OnPersonaPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			Connection.SendAsync (new PersonMessage (this.persona));
		}

		protected override void OnConnected (ClientConnectionEventArgs e)
		{
			Connection.SendAsync (new PersonMessage { Person = Persona });

			base.OnConnected (e);
		}

		private void OnConnectionInfoMessage (MessageEventArgs<ConnectionInfoMessage> e)
		{
			ServerTarget = e.Message.ConnectingFrom;
		}

		private void OnForwardMessage (MessageEventArgs<ForwardMessage> e)
		{
			Person sender;
			if (!this.watchList.TryGetPerson (e.Message.Identity, out sender))
				return;

			OnReceivedForwardedMessage (new ForwardedMessageEventArgs (
				sender, e.Message.ForwardedProtocol, e.Message.MessageType, e.Message.Payload));
		}

		private void OnTextMessage (MessageEventArgs<TextMessage> e)
		{
			Group group;
			lock (this.groups) {
				if (!this.groups.TryGetValue (e.Message.GroupId, out group))
					return;
			}

			if (e.Message.SenderId == null) {
				Trace.TraceWarning ("TextMessage received with no sender");
				return;
			}

			Person sender;
			if (!this.watchList.TryGetPerson (e.Message.SenderId, out sender))
				return;

			OnTextMessageReceived (new TextMessageEventArgs (group, sender, e.Message.Text));
		}

		private void OnGroupUpdatedMessage (MessageEventArgs<GroupUpdateMessage> e)
		{
			lock (this.groups) {
				Group group;
				if (!this.groups.TryGetValue (e.Message.Group.Id, out group))
					return;

				HashSet<string> newSet = new HashSet<string> (e.Message.Group.Participants);
				newSet.ExceptWith (group.Participants);

				HashSet<string> removedSet = new HashSet<string> (group.Participants);
				removedSet.ExceptWith (e.Message.Group.Participants);

				foreach (string id in removedSet) {
					group.Participants.Remove (id);
				}

				foreach (string id in newSet) {
					group.Participants.Add (id);
				}

				group.OwnerId = e.Message.Group.OwnerId;
			}
		}

		private void OnGroupInviteMessage (MessageEventArgs<GroupInviteMessage> e)
		{
			Task.Factory.StartNew (s => {
				var msg = (GroupInviteMessage)s;
				
				var args = new GroupInviteEventArgs (msg.Group);
				OnGroupInvite (args);

				Connection.SendResponseAsync (msg, new GroupInviteResponse {
					GroupId = msg.Group.Id,
					Response = (args.AcceptInvite) ? InvitationResponse.Accepted : InvitationResponse.Rejected
				});
			}, e.Message);
		}

		private void OnRequestBuddyListMessage (MessageEventArgs<RequestBuddyListMessage> e)
		{
			Task.Factory.StartNew (OnBuddyListRequested);
		}

		private void OnReceivedForwardedMessage (ForwardedMessageEventArgs args)
		{
			var handler = ReceivedForwardedMessage;
			if (handler != null)
				handler (this, args);
		}

		private void OnTextMessageReceived (TextMessageEventArgs args)
		{
			var handler = ReceivedTextMessage;
			if (handler != null)
				handler (this, args);
		}

		private void OnGroupInvite (GroupInviteEventArgs args)
		{
			var handler = InvitedToGroup;
			if (handler != null)
				handler (this, args);
		}

		private void OnBuddyListRequested ()
		{
			var handler = this.WatchListRequested;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}
	}
}