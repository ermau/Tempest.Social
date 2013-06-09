using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tempest.Social
{
	public class GroupResultMessage
		: SocialMessage
	{
		public GroupResultMessage()
			: base (SocialMessageType.GroupResult)
		{
			
		}

		public int GroupId
		{
			get;
			set;
		}

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteInt32 (GroupId);
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			GroupId = reader.ReadInt32();
		}
	}
}
