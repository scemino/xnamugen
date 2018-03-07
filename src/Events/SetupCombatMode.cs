namespace xnaMugen.Events
{
    internal class SetupCombatMode : Base
    {
        public SetupCombatMode(CombatMode mode)
        {
            Mode = mode;
        }

        public CombatMode Mode { get; }
    }
}