namespace FarleyFile.Interactions
{
    public sealed class ClearScreen : AbstractInteraction
    {
        public override string Pattern
        {
            get { return "^clr$"; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            
            context.Viewport.Clear();
            context.Viewport.Log("Yahoo!");
            return InteractionResult.Handled;
        }
    }
}