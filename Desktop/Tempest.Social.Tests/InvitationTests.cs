//
// Invitation.cs
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
	public class InvitationTests
	{
		[Test]
		public void CtorInvalid()
		{
			var ane = Assert.Throws<ArgumentNullException> (() =>
				new Invitation (null, new Person ("person"), InvitationResponse.Accepted),
				"Allowed a null group");
			Assert.AreEqual ("group", ane.ParamName);

			ane = Assert.Throws<ArgumentNullException> (() =>
				new Invitation (new Group (1), null, InvitationResponse.Accepted),
				"Allowed a null person");
			Assert.AreEqual ("person", ane.ParamName);

			var an = Assert.Throws<ArgumentException> (() =>
				new Invitation (new Group (1), new Person ("person"), (InvitationResponse)Byte.MaxValue),
				"Allowed an invalid value for response");
			Assert.AreEqual ("response", an.ParamName);
		}

		[Test]
		public void Ctor()
		{
			const string id = "identity";
			var group = new Group (1);
			var person = new Person ("persno");
			var invitation = new Invitation (group, person, InvitationResponse.Accepted);

			Assert.AreEqual (InvitationResponse.Accepted, invitation.Response);
			Assert.AreSame (group, invitation.Group);
			Assert.AreSame (person, invitation.Person);
		}
	}
}
