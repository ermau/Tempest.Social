using System;
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
			Assert.Throws<ArgumentNullException> (() => new Group (1, null));
		}

		[Test]
		public void Ctor()
		{
			string[] ids = new[] { "foo", "bar" };
			var g = new Group (1, ids);
			Assert.AreEqual (1, g.Id);
			CollectionAssert.AreEqual (ids, g.Participants);
		}
	}
}
