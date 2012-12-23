using System;
using System.Collections.Generic;

namespace Tempest.Social
{
	public class SearchResultMessage
		: SocialMessage
	{
		public SearchResultMessage()
			: base (SocialMessageType.SearchResult)
		{
		}

		public SearchResultMessage (IEnumerable<Person> results)
			: this()
		{
			if (results == null)
				throw new ArgumentNullException ("results");

			Results = results;
		}

		public IEnumerable<Person> Results
		{
			get;
			private set;
		}

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteEnumerable (context, Results);
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			Results = reader.ReadEnumerable (context, (c, r) => new Person (c, r));
		}
	}
}