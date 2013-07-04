//
// GroupTests.cs
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
using System.Linq;
using NUnit.Framework;

namespace Tempest.Social.Tests
{
	[TestFixture]
	public class GroupTests
	{
		[Test]
		public void CtorInvalid()
		{
			var ane = Assert.Throws<ArgumentNullException> (() => new Group (1, (string)null),
				"ownerId was allowed to be null");
			Assert.AreEqual ("ownerId", ane.ParamName);
		}

		[Test]
		public void Ctor()
		{
			const string owner = "foo";
			var g = new Group (1, owner);
			Assert.AreEqual (1, g.Id);
			Assert.AreEqual (owner, g.OwnerId);
			CollectionAssert.Contains (g.Participants, owner);
		}
	}
}