//
// PersonIdentityEqualityComparerTests.cs
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

using NUnit.Framework;

namespace Tempest.Social.Tests
{
	[TestFixture]
	public class PersonIdentityEqualityComparerTests
	{
		[Test]
		public void Equals()
		{
			var comparer = new PersonIdentityEqualityComparer();

			var p1 = new Person ("i1");
			var p2 = new Person ("i1");

			Assert.IsTrue (comparer.Equals (p1, p2));
			Assert.IsTrue (comparer.Equals (p2, p1));
		}

		[Test]
		public void Equals_Null()
		{
			var comparer = new PersonIdentityEqualityComparer();
			Assert.IsTrue (comparer.Equals (null, null));
		}

		[Test]
		public void Equals_False()
		{
			var comparer = new PersonIdentityEqualityComparer();

			var p1 = new Person ("i1");
			var p2 = new Person ("i2");

			Assert.IsFalse (comparer.Equals (p1, p2));
			Assert.IsFalse (comparer.Equals (p2, p1));
		}

		[Test]
		public void Equals_False_Null()
		{
			var comparer = new PersonIdentityEqualityComparer();

			var p1 = new Person ("i1");

			Assert.IsFalse (comparer.Equals (p1, null));
			Assert.IsFalse (comparer.Equals (null, p1));
		}
	}
}