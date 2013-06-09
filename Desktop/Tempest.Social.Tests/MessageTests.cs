//
// MessageTests.cs
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
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Tempest.Tests;

namespace Tempest.Social.Tests
{
	[TestFixture]
	public class MessageTests
	{
		[Test]
		public void PersonMessage()
		{
			Person person = new Person ("identity");
			person.Nickname = "nickname";
			person.Status = Status.Away;

			var msg = new PersonMessage { Person = person };

			msg = msg.AssertLengthMatches();
			Assert.AreEqual (person.Identity, msg.Person.Identity);
			Assert.AreEqual (person.Nickname, msg.Person.Nickname);
			Assert.AreEqual (person.Status, msg.Person.Status);
		}

		[Test]
		public void RequestBuddyListMessage()
		{
			var msg = new RequestBuddyListMessage();

			msg = msg.AssertLengthMatches();
		}

		[Test]
		public void BuddyListMessage()
		{
			Person p1 = new Person ("i1") { Nickname = "p1", Status = Status.Online };
			Person p2 = new Person ("i2") { Nickname = "p2", Status = Status.Away };

			var msg = new BuddyListMessage();
			msg.People = new[] { p1, p2 };
			msg.ChangeAction = NotifyCollectionChangedAction.Add;

			msg = msg.AssertLengthMatches();
			Person[] people = msg.People.ToArray();
			Assert.AreEqual (2, people.Length);
			Assert.IsNotNull (people[0]);
			Assert.IsNotNull (people[1]);

			Assert.AreEqual (p1.Identity, people[0].Identity);
			Assert.AreEqual (p1.Nickname, people[0].Nickname);
			Assert.AreEqual (p1.Status, people[0].Status);

			Assert.AreEqual (p2.Identity, people[1].Identity);
			Assert.AreEqual (p2.Nickname, people[1].Nickname);
			Assert.AreEqual (p2.Status, people[1].Status);
		}

		[Test]
		public void SearchMessage()
		{
			const string nick = "ermau";
			var msg = new SearchMessage { Nickname = nick };

			msg = msg.AssertLengthMatches();
			Assert.AreEqual (nick, msg.Nickname);
		}
	}
}