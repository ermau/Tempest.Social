//
// WatchList.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Tempest.Social
{
	public sealed class WatchList
		: ISerializable, ICollection<Person>, INotifyCollectionChanged
	{
		public WatchList (IClientContext clientContext)
		{
			if (clientContext == null)
				throw new ArgumentNullException ("clientContext");
			
			this.clientContext = clientContext;
			this.clientContext.RegisterMessageHandler<PersonMessage> (OnPersonMessage);
			this.clientContext.RegisterMessageHandler<BuddyListMessage> (OnBuddyListMessage);
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add { this.people.CollectionChanged += value; }
			remove { this.people.CollectionChanged -= value; }
		}

		public int Count
		{
			get { return this.people.Count; }
		}

		bool ICollection<Person>.IsReadOnly
		{
			get { return false; }
		}

		public void Add (Person item)
		{
			lock (this.people)
				this.people.Add (item);

			Send (NotifyCollectionChangedAction.Add, item);
		}

		public void Clear()
		{
			lock (this.people)
				this.people.Clear();

			Send (NotifyCollectionChangedAction.Reset);
		}

		public bool Contains (Person item)
		{
			lock (this.people)
				return this.people.Contains (item);
		}

		public void CopyTo (Person[] array, int arrayIndex)
		{
			lock (this.people)
				this.people.CopyTo (array, arrayIndex);
		}

		public bool Remove (Person item)
		{
			bool removed;
			lock (this.people)
				removed = this.people.Remove (item);

			Send (NotifyCollectionChangedAction.Remove, item);

			return removed;
		}

		public bool TryGetPerson (string identity, out Person person)
		{
			lock (this.people)
			{
				person = this.people.FirstOrDefault (p => p.Identity == identity);
				return (person != null);
			}
		}

		public void Serialize (ISerializationContext context, IValueWriter writer)
		{
			lock (this.people)
				writer.WriteEnumerable (context, this.people);
		}

		public void Deserialize (ISerializationContext context, IValueReader reader)
		{
			lock (this.people)
			{
				this.people.Clear();

				foreach (Person person in reader.ReadEnumerable (context, (c, r) => new Person (c, r)))
					this.people.Add (person);
			}
		}

		public IEnumerator<Person> GetEnumerator()
		{
			return this.people.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private readonly ObservableCollection<Person> people = new ObservableCollection<Person>();
		private readonly IClientContext clientContext;

		private void OnPersonMessage (MessageEventArgs<PersonMessage> e)
		{
			lock (this.people)
			{
				Person person;
				if (TryGetPerson (e.Message.Person.Identity, out person)) {
					person.Nickname = e.Message.Person.Nickname;
					person.Status = e.Message.Person.Status;
				} else
					this.people.Add (e.Message.Person);
			}
		}

		private void Send (NotifyCollectionChangedAction action)
		{
			if (action != NotifyCollectionChangedAction.Reset)
				throw new ArgumentException();

			Send (action, (IEnumerable<Person>)null);
		}

		private void Send (NotifyCollectionChangedAction action, Person person)
		{
			Send (action, new[] { person });
		}

		private void Send (NotifyCollectionChangedAction action, IEnumerable<Person> people)
		{
			var msg = new BuddyListMessage();
			msg.People = people;
			msg.ChangeAction = action;
			this.clientContext.Connection.SendAsync (msg);
		}

		private void OnBuddyListMessage (MessageEventArgs<BuddyListMessage> e)
		{
			switch (e.Message.ChangeAction)
			{
				case NotifyCollectionChangedAction.Reset:
					lock (this.people)
					{
						this.people.Clear();
						foreach (Person person in e.Message.People)
							this.people.Add (person);
					}
					break;

				case NotifyCollectionChangedAction.Add:
					lock (this.people)
					{
						foreach (Person person in e.Message.People)
							this.people.Add (person);
					}

					break;

				case NotifyCollectionChangedAction.Remove:
					lock (this.people)
					{
						foreach (Person person in e.Message.People)
							this.people.Remove (person);
					}

					break;
			}
		}
	}
}