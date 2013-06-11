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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tempest.Social.Tests
{
	[TestFixture]
	public class GroupManagerTests
	{
		[Test]
		public void CreateGroupInvalid()
		{
			var gm = new GroupManager();
			var ane = Assert.Throws<ArgumentNullException> (() => gm.CreateGroup (null),
				"Allowed null person");
			Assert.AreEqual ("person", ane.ParamName);
		}

		[Test]
		public void CreateGroup()
		{
			var person = new Person ("id");

			var vm = new GroupManager();
			var group = vm.CreateGroup (person);

			Assert.IsNotNull (group);
			CollectionAssert.Contains (group.Participants, person.Identity);
		}

		[Test]
		public void LeaveGroupInvalid()
		{
			var gm = new GroupManager();
			var ane = Assert.Throws<ArgumentNullException> (() => gm.LeaveGroup (null, new Group (1)),
				"Allowed null person");
			Assert.AreEqual ("person", ane.ParamName);

			ane = Assert.Throws<ArgumentNullException> (() => gm.LeaveGroup (new Person ("id"), null),
				"Allowed null group");
			Assert.AreEqual ("group", ane.ParamName);
		}
	}
}