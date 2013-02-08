namespace Assets.Scripts
{
    public class TK2DAnimationAdapter : IAnimatingSprite
    {
        private readonly tk2dAnimatedSprite _sprite;

        public TK2DAnimationAdapter(tk2dAnimatedSprite sprite)
        {
            _sprite = sprite;
        }

        public string CurrentAnimation { get { return _sprite.CurrentClip.name; } }
        public void Play(string animationName) { _sprite.Play(animationName); }
    }
}