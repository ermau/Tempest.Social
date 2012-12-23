namespace Tempest.Social
{
	public class SearchMessage
		: SocialMessage
	{
		public SearchMessage()
			: base (SocialMessageType.Search)
		{
		}

		public string Nickname
		{
			get;
			set;
		}

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteString (Nickname);
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			Nickname = reader.ReadString();
		}
	}
}