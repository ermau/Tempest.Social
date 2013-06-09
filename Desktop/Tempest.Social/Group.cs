using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tempest.Social
{
	public class Group
	{
		public int Id
		{
			get;
			internal set;
		}

		public IEnumerable<string> Participants
		{
			get { return this.participants; }
		}

		internal readonly ObservableCollection<string> participants = new ObservableCollection<string>();
	}

	public class GroupSerializer
		: ISerializer<Group>
	{
		public static readonly GroupSerializer Instance = new GroupSerializer();

		public void Serialize (ISerializationContext context, IValueWriter writer, Group element)
		{
			writer.WriteInt32 (element.Id);
			writer.WriteEnumerable (context, Serializer<string>.Default, element.Participants);
		}

		public Group Deserialize (ISerializationContext context, IValueReader reader)
		{
			Group group = new Group();
			group.Id = reader.ReadInt32();
			foreach (string p in reader.ReadEnumerable (context, Serializer<string>.Default))
				group.participants.Add (p);

			return group;
		}
	}
}