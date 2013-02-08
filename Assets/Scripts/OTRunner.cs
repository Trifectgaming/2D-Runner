namespace Assets.Scripts
{
    public class OTRunner : Runner
    {
        protected override IAnimatingSprite GetSprite()
        {
            return new OTAnimationAdapter(GetComponentInChildren<OTAnimatingSprite>());
        }
    }
}