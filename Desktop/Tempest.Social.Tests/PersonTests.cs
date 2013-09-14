//
// PersonTests.cs
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
using NUnit.Framework;

namespace Tempest.Social.Tests
{
	[TestFixture]
	public class PersonTests
	{
		[Test]
		public void CtorInvalid()
		{
			Assert.Throws<ArgumentNullException> (() => new Person (null));
			Assert.Throws<ArgumentException> (() => new Person (String.Empty));
			Assert.Throws<ArgumentException> (() => new Person ("  "));
		}

		[Test]
		public void Ctor()
		{
			const string id = "fooBar";
			var person = new Person (id);

			Assert.AreEqual (id, person.Identity);
			Assert.AreEqual (Status.Offline, person.Status);
		}

		[Test]
		public void Serialize()
		{
			var person = new Person ("foobar") {
				Nickname = "MyNickname",
				Avatar = "MyAvatar",
				Status = Status.Away
			};

			byte[] buffer = new byte[2048];
			var bufferWriter = new BufferValueWriter (buffer);

			person.Serialize (null, bufferWriter);

			var bufferReader = new BufferValueReader (buffer);
			var person2 = new Person (null, bufferReader);

			Assert.AreEqual (person.Identity, person2.Identity);
			Assert.AreEqual (person.Nickname, person2.Nickname);
			Assert.AreEqual (person.Avatar, person2.Avatar);
			Assert.AreEqual (person.Status, person2.Status);
		}
	}
}