//
// WatchListTests.cs
//
// Author:
//   Eric Maupin <me@ermau.com>
//
// Copyright (c) 2013 Eric Maupin
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Tempest.Tests;

namespace Tempest.Social.Tests
{
	[TestFixture]
	public class WatchListTests
	{
		[Test]
		public void CtorInvalid()
		{
			Assert.Throws<ArgumentNullException> (() => new WatchList (null));
		}

		private WatchList list;
		private ConnectionBuffer client;
		private ConnectionBuffer server;
		private SerializationContext clientContext;

		[SetUp]
		public void Setup()
		{
			var provider = new MockConnectionProvider (SocialProtocol.Instance);
			provider.ConnectionMade += (sender, args) => server = new ConnectionBuffer (args.Connection);
			provider.Start (MessageTypes.Reliable);

			var c = new MockClientConnection (provider);
			client = new ConnectionBuffer (c);

			clientContext = new SerializationContext (c, new Dictionary<byte, Protocol> { { 2, SocialProtocol.Instance } });

			var context = new TempestClient (c, MessageTypes.Reliable);
			context.ConnectAsync (new Target (Target.LoopbackIP, 1)).Wait();

			list = new WatchList (context);
		}

		[Test]
		public void Ctor()
		{
			Assert.AreEqual (0, list.Count);
			CollectionAssert.IsEmpty (list);
		}

		[Test]
		public void AddInvalid()
		{
			Assert.Throws<ArgumentNullException> (() => list.Add (null));
		}

		[Test]
		public void Add()
		{
			var person = new Person ("identity");
			list.Add (person);

			CollectionAssert.Contains (list, person);

			var msg = server.DequeueAndAssertMessage<BuddyListMessage>();
			Assert.AreEqual (NotifyCollectionChangedAction.Add, msg.ChangeAction);
			Assert.AreEqual (1, msg.People.Count());
			Assert.AreEqual (person, msg.People.Single());
		}

		[Test]
		public void ReceiveAdd()
		{
			var test = new AsyncTest();

			var p = new Person ("id");

			list.CollectionChanged += (sender, args) => {
				Assert.AreEqual (NotifyCollectionChangedAction.Add, args.Action);
				Assert.IsNotNull (args.NewItems);
				Assert.AreEqual (p, args.NewItems[0]);
				CollectionAssert.Contains (list, p);
				test.PassHandler (null, EventArgs.Empty);
			};

			this.server.SendAsync (new BuddyListMessage
			{
				ChangeAction = NotifyCollectionChangedAction.Add,
				People = new[] { p }
			});

			test.Assert (10000);

			Assert.AreEqual (p, list.FirstOrDefault());
		}

		[Test]
		public void ContainsInvalid()
		{
			Assert.Throws<ArgumentNullException> (() => list.Contains (null));
		}

		[Test]
		public void Contains()
		{
			var person = new Person ("contains");
			list.Add (person);

			Assert.IsTrue (list.Contains (person));
			Assert.IsFalse (list.Contains (new Person ("id2")));
		}

		[Test]
		public void Clear()
		{
			Add();

			list.Clear();
			CollectionAssert.IsEmpty (list);

			var msg = server.DequeueAndAssertMessage<BuddyListMessage>();
			Assert.AreEqual (NotifyCollectionChangedAction.Reset, msg.ChangeAction);
		}

		[Test]
		public void ReceiveReset()
		{
			Add();

			var test = new AsyncTest();

			list.CollectionChanged += (sender, args) => {
				Assert.AreEqual (NotifyCollectionChangedAction.Reset, args.Action);
				Assert.IsNull (args.NewItems);
				Assert.IsNull (args.OldItems);
				test.PassHandler (null, EventArgs.Empty);
			};

			this.server.SendAsync (new BuddyListMessage {
				ChangeAction = NotifyCollectionChangedAction.Reset
			});

			test.Assert (10000);
		}

		[Test]
		public void RemoveInvalid()
		{
			Assert.Throws<ArgumentNullException> (() => list.Remove (null));
		}

		[Test]
		public void Remove()
		{
			Add();
			var person = list.First();

			Assert.IsTrue (list.Remove (person));
			CollectionAssert.DoesNotContain (list, person);

			var msg = server.DequeueAndAssertMessage<BuddyListMessage>();
			Assert.AreEqual (NotifyCollectionChangedAction.Remove, msg.ChangeAction);
			CollectionAssert.Contains (msg.People, person);
			Assert.AreEqual (1, msg.People.Count());

			Assert.IsFalse (list.Remove (person));
			server.AssertNoMessage();
		}

		[Test]
		public void ReceiveRemove()
		{
			var person = new Person ("remove");
			list.Add (person);
			
			var test = new AsyncTest();

			list.CollectionChanged += (sender, args) => {
				Assert.AreEqual (NotifyCollectionChangedAction.Remove, args.Action);
				Assert.IsNull (args.NewItems);
				Assert.IsNotNull (args.OldItems);
				CollectionAssert.Contains (args.OldItems, person);
				CollectionAssert.DoesNotContain (list, person);
				test.PassHandler (null, EventArgs.Empty);
			};

			this.server.SendAsync(new BuddyListMessage
			{
				ChangeAction = NotifyCollectionChangedAction.Remove,
				People = new[] {  person }
			});

			test.Assert (10000);
		}

		[Test]
		public void CopyToInvalid()
		{
			Assert.Throws<ArgumentNullException> (() => list.CopyTo (null, 0));
			Assert.Throws<ArgumentOutOfRangeException> (() => list.CopyTo (new Person[1], 1));
		}

		[Test]
		public void TryGetPersonInvalid()
		{
			Person person;
			Assert.Throws<ArgumentNullException> (() => list.TryGetPerson (null, out person));
		}

		[Test]
		public void TryGetPerson()
		{
			var person = new Person ("try");
			list.Add (person);

			Person getPerson;
			Assert.IsTrue (list.TryGetPerson (person.Identity, out getPerson));
			Assert.IsNotNull (getPerson);
			Assert.AreEqual (person, getPerson);
		}

		[Test]
		public void Serialize()
		{
			var person = new Person ("first");
			var person2 = new Person ("second");
			list.Add (person);
			server.DequeueAndAssertMessage<BuddyListMessage>();
			list.Add (person2);
			server.DequeueAndAssertMessage<BuddyListMessage>();

			var writer = new BufferValueWriter (new byte[1024]);
			list.Serialize (clientContext, writer);

			server.AssertNoMessage();

			var reader = new BufferValueReader (writer.Buffer);
			list.Deserialize (clientContext, reader);

			Assert.AreEqual (writer.Length, reader.Position);

			server.AssertNoMessage();

			CollectionAssert.Contains (list, person);
			CollectionAssert.Contains (list, person2);
		}
	}
}