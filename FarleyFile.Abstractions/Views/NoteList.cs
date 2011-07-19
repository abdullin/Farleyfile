using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Views
{
    public sealed class NoteList : IEntityBase
    {
        public IList<Item> Notes { get; set; }

        public NoteList()
        {
            Notes = new List<Item>();
        }

        public void ArchiveNote(NoteId noteId)
        {
            var notes = Notes.Where(n => n.NoteId == noteId).ToList();
            foreach (var note in notes)
            {
                Notes.Remove(note);
            }
        }

        public void AddNote(NoteId noteId, string title, string text)
        {
            Notes.Add(new Item()
                {
                    NoteId = noteId,
                    Title = title
                });
        }

        
        public void UpdateNote(NoteId noteId, Action<Item> update)
        {
            foreach (var note in Notes.Where(t => t.NoteId == noteId))
            {
                update(note);
            }
        }

        public sealed class Item
        {
            public NoteId NoteId { get; set; }
            public string Title { get; set; }
        }

    }
}