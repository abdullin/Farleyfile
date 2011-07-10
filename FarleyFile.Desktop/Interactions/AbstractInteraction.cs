using System.Text.RegularExpressions;
using FarleyFile.Interactions;

namespace FarleyFile
{
    public abstract class AbstractInteraction 
    {
        protected readonly Regex Matcher;
        public abstract string Pattern { get; }

        protected AbstractInteraction()
        {
            Matcher = new Regex(Pattern);
        }

        public bool WillProcess(string data)
        {
            var match = Matcher.Match(data);
            return match.Success;
        }

        public abstract InteractionResult Handle(InteractionContext context);
    }
}