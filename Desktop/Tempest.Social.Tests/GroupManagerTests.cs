//
// GroupManagerTests.cs
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
using System.Linq;
using NUnit.Framework;

namespace Tempest.Social.Tests
{
	[TestFixture]
	public class GroupManagerTests
	{
		private Person person;
		private Group group;
		private GroupManager gm;

		[SetUp]
		public void Setup()
		{
			gm = new GroupManager();
			person = new Person ("Id");
			group = new Group (1);
		}

		[Test]
		public void CreateGroupInvalid()
		{
			var ane = Assert.Throws<ArgumentNullException> (() => gm.CreateGroup (null),
				"Allowed null person");
			Assert.AreEqual ("person", ane.ParamName);
		}

		[Test]
		public void CreateGroup()
		{
			var g = gm.CreateGroup (person);

			Assert.IsNotNull (g);
			CollectionAssert.Contains (g.Participants, person.Identity);
		}

		[Test]
		public void JoinGroupInvalid()
		{
			var ane = Assert.Throws<ArgumentNullException> (() => gm.JoinGroup (null, person),
				"Allowed null group");
			Assert.AreEqual ("group", ane.ParamName);

			ane = Assert.Throws<ArgumentNullException> (() => gm.JoinGroup (group, null),
				"Allowed null person");
			Assert.AreEqual ("person", ane.ParamName);
		}

		[Test]
		public void JoinGroupUnknown()
		{
			Assert.DoesNotThrow (() => gm.JoinGroup (group, person));
			CollectionAssert.IsEmpty (group.Participants);
		}

		[Test]
		public void JoinGroup()
		{
			group = gm.CreateGroup (person);
			var p2 = new Person ("2");

			bool joined = false;
			gm.GroupUpdated += g => {
				Assert.AreSame (group, g);
				joined = true;
			};

			gm.JoinGroup (group, p2);

			CollectionAssert.Contains (group.Participants, person.Identity);

			Assert.IsTrue (joined, "GroupUpdated did not fire");
		}

		[Test]
		public void LeaveGroupInvalid()
		{
			var ane = Assert.Throws<ArgumentNullException> (() => gm.LeaveGroup (new Group (1), null),
				"Allowed null person");
			Assert.AreEqual ("person", ane.ParamName);

			ane = Assert.Throws<ArgumentNullException> (() => gm.LeaveGroup (null, new Person ("id")),
				"Allowed null group");
			Assert.AreEqual ("group", ane.ParamName);
		}

		[Test]
		public void LeaveGroupUnknown()
		{
			Assert.DoesNotThrow (() => gm.LeaveGroup (group, person));
			CollectionAssert.IsEmpty (group.Participants);
		}

		[Test]
		public void LeaveGroupNotIn()
		{
			group = gm.CreateGroup (person);
			var p2 = new Person ("2");

			bool updated = false;
			gm.GroupUpdated += g => updated = true;

			gm.LeaveGroup (group, p2);

			CollectionAssert.DoesNotContain (group.Participants, p2.Identity);
			CollectionAssert.Contains (group.Participants, person.Identity);

			Assert.IsFalse (updated, "GroupUpdated was fired without any changes");
		}

		[Test]
		public void LeaveGroup()
		{
			group = gm.CreateGroup (person);
			var p2 = new Person ("2");
			gm.JoinGroup (group, p2);

			bool updated = false;
			gm.GroupUpdated += g => {
				updated = true;
				Assert.AreSame (group, g);
			};

			gm.LeaveGroup (group, p2);

			CollectionAssert.Contains (group.Participants, person.Identity);
			CollectionAssert.DoesNotContain (group.Participants, p2.Identity);
			Assert.IsTrue (updated, "GroupUpdated did not fire");
		}
	}
}