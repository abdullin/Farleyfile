using System.Collections.Generic;

namespace FarleyFile
{
    public sealed class DayText : IEntityBase
    {
        public IList<string> Notes { get; set; }

        public DayText()
        {
            Notes = new List<string>();
        }
    }
}