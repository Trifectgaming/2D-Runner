using System.Linq;

namespace Assets.Scripts
{
    public class TK2DAnimationAdapter : IAnimatingSprite
    {
        private readonly tk2dAnimatedSprite _sprite;

        public TK2DAnimationAdapter(tk2dAnimatedSprite sprite)
        {
            _sprite = sprite;
        }

        public string[] AvailableAnimations
        {
            get { return _sprite.Library.clips.Select(c => c.name).ToArray(); }
        }

        public string CurrentAnimation
        {
            get
            {
                return _sprite.CurrentClip != null ? _sprite.CurrentClip.name : null;
            }
        }

        public void Play(string animationName) { _sprite.Play(animationName); }
    }
}