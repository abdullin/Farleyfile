using System;
using System.Runtime.Serialization;

namespace FarleyFile
{
    public interface IEvent : IBaseMessage
    {

    }

    [DataContract]
    public class Identity
    {
        [DataMember(Order = 1)]
        public Guid Id { get; protected set; }
        [DataMember(Order = 2)]
        public int Tag { get; protected set; }

        public Identity(Guid id, int tag)
        {
            Id = id;
            Tag = tag;
        }

        public bool IsEmpty { get { return Id == Guid.Empty; } }

        public static readonly Identity Empty = new Identity(Guid.Empty, 0);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            var identity = obj as Identity;

            if (identity == null)
            {
                return false;
            }

            return identity.Id.Equals(Id) && identity.Tag == Tag;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode()*397) ^ Tag;
            }
        }

        public static implicit operator Guid(Identity id)
        {
            return id.Id;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})-{2}", GetType().Name.Replace("Id", ""), Tag, Id);
        }
    }


    [DataContract]
    public sealed class NoteId : Identity
    {
        public const int Tag = 1;
        public NoteId(Guid id) : base(id, Tag)
        {
            
        }
    }
    [DataContract]
    public sealed class StoryId : Identity
    {
        public const int Tag = 2;
        public StoryId(Guid id) : base(id, Tag) {}
        
    }
    [DataContract]
    public sealed class ActivityId : Identity
    {
        public const int Tag = 3;
        public ActivityId(Guid id) : base(id, Tag) {}
    }

    [DataContract]
    public sealed class TaskId : Identity
    {
        public const int Tag = 4;
        public TaskId(Guid id) : base(id, Tag) {}
    }

}