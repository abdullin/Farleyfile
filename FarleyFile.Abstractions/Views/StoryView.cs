using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Views
{
    public sealed class StoryView : IEntityBase
    {
        public string Name { get; set; }
        public StoryId StoryId { get; set; }

        public IList<StoryViewNote> Notes { get; set; }
        

        public StoryView()
        {
            Notes = new List<StoryViewNote>();
        }

        public void RemoveNote(NoteId noteId)
        {
            var notes = Notes.Where(n => n.NoteId == noteId).ToList();
            foreach (var note in notes)
            {
                Notes.Remove(note);
            }
        }

        public void AddNote(NoteId noteId, string title, string text)
        {
            Notes.Add(new StoryViewNote()
                {
                    NoteId = noteId,
                    Title = title
                });
        }

        
        public void UpdateNote(NoteId noteId, Action<StoryViewNote> update)
        {
            foreach (var note in Notes.Where(t => t.NoteId == noteId))
            {
                update(note);
            }
        }
    }
}