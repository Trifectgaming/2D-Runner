using System;

namespace Assets.Scripts
{
    [Serializable]
    public class StateTransition
    {
        protected bool Equals(StateTransition other)
        {
            return 
                currentState == other.currentState && 
                command == other.command && 
                below.Equals(other.below) && 
                above.Equals(other.above) && 
                front.Equals(other.front) && 
                behind.Equals(other.behind);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int) currentState;
                hashCode = (hashCode*397) ^ (int) command;
                hashCode = (hashCode*397) ^ below.GetHashCode();
                hashCode = (hashCode*397) ^ above.GetHashCode();
                hashCode = (hashCode*397) ^ front.GetHashCode();
                hashCode = (hashCode*397) ^ behind.GetHashCode();
                return hashCode;
            }
        }

        public RunnerState currentState;
        public InputState command;
        public bool below;
        public bool above;
        public bool front;
        public bool behind;
        public float minSpeedX = -1;
        public float minSpeedY = -1;
        public float maxSpeedX = -1;
        public float maxSpeedY = -1;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StateTransition) obj);
        }
    }
}