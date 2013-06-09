using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tempest.Social
{
	public class TextMessage
		: SocialMessage
	{
		public TextMessage()
			: base (SocialMessageType.Text)
		{
		}

		public string Text
		{
			get;
			set;
		}

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteString (Text);
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			Text = reader.ReadString();
		}
	}
}