using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tempest.Social
{
	public class Group
	{
		public Group (int id)
		{
			Id = id;
		}

		public Group (int id, IEnumerable<string> participants)
			: this (id)
		{
			if (participants == null)
				throw new ArgumentNullException ("participants");

			foreach (string participant in participants)
				this.participants.Add (participant);
		}

		public int Id
		{
			get;
			private set;
		}

		public ICollection<string> Participants
		{
			get { return this.participants; }
		}

		private readonly ObservableCollection<string> participants = new ObservableCollection<string>();
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
			return new Group (reader.ReadInt32(), reader.ReadEnumerable (context, Serializer<string>.Default));
		}
	}
}