using Systems.SimpleCore.Operations;

namespace Systems.SimpleQuests.Operations
{
    public static class QuestOperations
    {
        public const ushort SYSTEM_QUESTS = 0x0009;

        public const ushort SUCCESS_STARTED = 0x0001;
        public static OperationResult Permitted() => 
            OperationResult.Success(SYSTEM_QUESTS, OperationResult.SUCCESS_PERMITTED);
        
        public static OperationResult Started() => 
            OperationResult.Success(SYSTEM_QUESTS, SUCCESS_STARTED);
    }
}