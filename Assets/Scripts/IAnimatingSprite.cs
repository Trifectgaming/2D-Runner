namespace Assets.Scripts
{
    public interface IAnimatingSprite
    {
        string CurrentAnimation { get; }
        void Play(string animationName);
    }
}