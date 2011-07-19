using System;
using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class NoteViewHandler : 
        IConsume<NoteAdded>,
        IConsume<NoteEdited>,
        IConsume<NoteArchived>,
        IConsume<NoteRenamed>
    {
        readonly IAtomicEntityWriter<Identity, NoteView> _writer;
        public NoteViewHandler(IAtomicEntityWriter<Identity, NoteView> writer)
        {
            _writer = writer;
        }

        public void Consume(NoteAdded e)
        {
            _writer.Add(e.NoteId, new NoteView()
                {
                    Text = e.Text,
                    Title = e.Title
                });
        }

        public void Consume(NoteEdited e)
        {
            _writer.UpdateOrThrow(e.NoteId, nv => nv.Text = e.NewText);
        }

        public void Consume(NoteArchived e)
        {
            //_writer.TryDelete(e.NoteId);
        }

        public void Consume(NoteRenamed e)
        {
            _writer.UpdateOrThrow(e.NoteId, nv => nv.Title = e.NewName);
        }
    }
}