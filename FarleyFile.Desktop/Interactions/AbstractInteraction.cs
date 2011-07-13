using System.Collections.Generic;
using System.Text.RegularExpressions;
using FarleyFile.Interactions;

namespace FarleyFile
{
    public abstract class AbstractInteraction 
    {
        protected abstract string[] Alias { get; }

        public bool WillProcess(string data, out string alias, out string match)
        {
            alias = null;
            match = null;
            foreach (var a in Alias)
            {
                if (data == a)
                {
                    alias = a;
                    match = "";
                    return true;
                }
                if (data.StartsWith(a + ' '))
                {
                    alias = a;
                    match = data.Substring(a.Length + 1);
                    return true;
                }
            }
            return false;
        }

        public abstract InteractionResult Handle(InteractionContext context);
    }
}