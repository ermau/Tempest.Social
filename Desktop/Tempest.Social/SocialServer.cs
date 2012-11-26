//
// SocialServer.cs
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
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Cadenza.Collections;

namespace Tempest.Social
{
	public class SocialServer
		: Server
	{
		public SocialServer (IWatchListProvider provider, IIdentityProvider identityProvider)
			: base (MessageTypes.Reliable)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");
			if (identityProvider == null)
				throw new ArgumentNullException ("identityProvider");

			this.provider = provider;
			this.identityProvider = identityProvider;

			this.RegisterMessageHandler<PersonMessage> (OnPersonMessage);
			this.RegisterMessageHandler<BuddyListMessage> (OnBuddyListMessage );
		}

		private readonly IWatchListProvider provider;
		private readonly IIdentityProvider identityProvider;
		
		private readonly object sync = new object();
		private readonly Dictionary<string, Person> people = new Dictionary<string, Person>();
		private readonly BidirectionalDictionary<string, IConnection> connections = new BidirectionalDictionary<string, IConnection>();
		
		private async Task<Person> GetPerson (IConnection connection)
		{
			string identity;
			bool found = false;
			lock (this.sync)
				found = this.connections.TryGetKey (connection, out identity);

			if (!found)
				identity = await this.identityProvider.GetIdentityAsync (connection);

			Person person;
			lock (this.sync)
			{
				if (!this.people.TryGetValue (identity, out person))
					this.people[identity] = person = new Person (identity);
			}

			if (person.Identity != identity)
				return null;

			return person;
		}

		private async void OnBuddyListMessage (MessageEventArgs<BuddyListMessage> e)
		{
			Person owner = await GetPerson (e.Connection);
			if (owner == null)
			{
				e.Connection.Disconnect (ConnectionResult.Custom, "Identity not found or verified");
				return;
			}

			switch (e.Message.ChangeAction)
			{
				case NotifyCollectionChangedAction.Add:
					this.provider.AddRangeAsync (owner, e.Message.People);
					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (Person person in e.Message.People)
						await this.provider.RemoveAsync (owner.Identity, person.Identity);

					break;

				case NotifyCollectionChangedAction.Reset:
					await this.provider.ClearAsync (owner.Identity);
					this.provider.AddRangeAsync (owner, e.Message.People);
					break;
			}
		}

		private async void OnPersonMessage (MessageEventArgs<PersonMessage> e)
		{
			string identity = await this.identityProvider.GetIdentityAsync (e.Connection);
			if (identity == null || identity != e.Message.Person.Identity)
			{
				e.Connection.Disconnect (ConnectionResult.Custom, "Identity not found or verified");
				return;
			}

			bool justJoined = false;
			lock (this.sync)
			{
				Person person;
				if (this.people.TryGetValue (identity, out person))
				{
					person.Nickname = e.Message.Person.Nickname;
					person.Status = e.Message.Person.Status;
				}
				else
				{
					this.connections[identity] = e.Connection;
					this.people[identity] = e.Message.Person;
					justJoined = true;
				}
			}

			if (justJoined)
			{
				IEnumerable<Person> watched = await this.provider.GetWatchedAsync (identity);

				if (watched == null || !watched.Any())
					e.Connection.Send (new RequestBuddyListMessage());
				else
				{
					e.Connection.Send (new BuddyListMessage
					{
						ChangeAction = NotifyCollectionChangedAction.Reset,
						People = watched
					});
				}
			}

			foreach (Person watcher in await this.provider.GetWatchersAsync (identity))
			{
				IConnection connection;
				lock (this.sync)
				{
					if (!this.connections.TryGetValue (watcher.Identity, out connection))
						continue;
				}

				connection.Send (new PersonMessage { Person = e.Message.Person });
			}
		}
	}
}