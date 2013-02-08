namespace Assets.Scripts
{
    public class TK2DRunner : Runner
    {
        protected override IAnimatingSprite GetSprite()
        {
            return new TK2DAnimationAdapter(GetComponentInChildren<tk2dAnimatedSprite>());
        }
    }
}