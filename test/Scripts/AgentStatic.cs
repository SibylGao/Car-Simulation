namespace Simulate
{
    /// <summary>
    /// agent的所有状态，根据需要自己填充
    /// </summary>
    public struct AgentStates
    {
        private int state;
        public static readonly AgentStates Hide = new AgentStates(1);
        public static readonly AgentStates Show = new AgentStates(2);
        public static readonly AgentStates Enter = new AgentStates(3);

        public AgentStates(int s)
        {
            state = s;
        }

        //public override bool Equals(object other);

        public static bool operator ==(AgentStates a, AgentStates b)
        {
            return a.state == b.state;
        }
        public static bool operator !=(AgentStates a, AgentStates b)
        {
            return a.state != b.state;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AgentStates))
            {
                return false;
            }

            var states = (AgentStates)obj;
            return state == states.state;
        }

        public override int GetHashCode()
        {
            var hashCode = 2016490230;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + state.GetHashCode();
            return hashCode;
        }
    }
}
