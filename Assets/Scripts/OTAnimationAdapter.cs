namespace Assets.Scripts
{
    public class OTAnimationAdapter : IAnimatingSprite
    {
        private readonly OTAnimatingSprite _sprite;

        public OTAnimationAdapter(OTAnimatingSprite sprite)
        {
            _sprite = sprite;
        }

        public string CurrentAnimation { get { return _sprite.animationFrameset; } }
        public void Play(string animationName) { _sprite.Play(animationName); }
    }
}