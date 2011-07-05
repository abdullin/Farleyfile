using System.Collections.Generic;
using FarleyFile.Aggregates;

namespace FarleyFile
{
    public static class AggregateFactory
    {
        public static IEnumerable<IEvent> LoadProject(IEnumerable<IEvent> given, IEnumerable<ICommand> when)
        {
            var state = new PerspectiveAggregateState();
            foreach (var @event in given)
            {
                state.Apply(@event);
            }

            var result = new List<IEvent>();
            var ar = new PerspectiveAggregate(result.Add, state);
            foreach (var c in when)
            {
                ar.Execute(c);
            }
            return result;
        }
    }
}