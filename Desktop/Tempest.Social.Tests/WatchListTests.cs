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
		private MockClientConnection clientConn;
		private MockServerConnection serverConn;

		[SetUp]
		public void Setup()
		{
			var provider = new MockConnectionProvider (SocialProtocol.Instance);
			provider.Start (MessageTypes.Reliable);

			var cs = provider.GetConnections (SocialProtocol.Instance);
			clientConn = cs.Item1;
			serverConn = cs.Item2;

			var context = new TempestClient (clientConn, MessageTypes.Reliable);
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
		public void ReceiveAdd()
		{
			var test = new AsyncTest();

			var p = new Person ("id");

			list.CollectionChanged += (sender, args) => {
				Assert.AreEqual (NotifyCollectionChangedAction.Add, args.Action);
				Assert.IsNotNull (args.NewItems);
				Assert.AreEqual (p, args.NewItems[0]);
				test.PassHandler (null, EventArgs.Empty);
			};

			serverConn.SendAsync (new PersonMessage (p));

			test.Assert (10000);

			Assert.AreEqual (p, list.FirstOrDefault());
		}
	}
}