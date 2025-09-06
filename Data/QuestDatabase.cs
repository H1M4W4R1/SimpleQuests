using JetBrains.Annotations;
using Systems.SimpleCore.Storage;
using Systems.SimpleCore.Storage.Databases;
using Systems.SimpleQuests.Abstract;

namespace Systems.SimpleQuests.Data
{
    /// <summary>
    ///     Database containing all in-game quests and tasks.
    /// </summary>
    public sealed class QuestDatabase : AddressableDatabase<QuestDatabase, Quest>
    {
        public const string LABEL = "SimpleQuests.Quests";
        
        [NotNull] protected override string AddressableLabel => LABEL;
    }
}