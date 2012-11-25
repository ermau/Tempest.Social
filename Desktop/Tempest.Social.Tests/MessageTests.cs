//
// MessageTests.cs
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

using System.Net;
using NUnit.Framework;
using Tempest.Tests;

namespace Tempest.Social.Tests
{
	[TestFixture]
	public class MessageTests
	{
		[Test]
		public void EndPointRequestMessage()
		{
			const string identity = "identity";
			var msg = new EndPointRequestMessage { Identity = identity };

			msg = msg.AssertLengthMatches();
			Assert.AreEqual (identity, msg.Identity);
		}

		[Test]
		public void EndPointResultMessage()
		{
			IPEndPoint endPoint = new IPEndPoint (IPAddress.Loopback, 42);
			var msg = new EndPointResultMessage { EndPoint = endPoint };

			msg = msg.AssertLengthMatches();
			Assert.AreEqual (endPoint, msg.EndPoint);
		}

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
	}
}