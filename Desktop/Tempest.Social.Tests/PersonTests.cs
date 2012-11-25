using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	}
}