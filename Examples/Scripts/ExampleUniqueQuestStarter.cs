using Systems.SimpleQuests.Utility;
using UnityEngine;

namespace Systems.SimpleQuests.Examples.Scripts
{
    /// <summary>
    ///     Demonstrates <see cref="Systems.SimpleQuests.Abstract.Markers.IUniqueQuest"/> behaviour.
    ///     Use the context-menu actions to start the quest twice and observe that the second
    ///     attempt is rejected while the first instance is still active.
    /// </summary>
    public sealed class ExampleUniqueQuestStarter : MonoBehaviour
    {
        /// <summary>
        ///     Starts the unique quest. The first call succeeds; subsequent calls while the
        ///     quest is active log a rejection message via <see cref="ExampleUniqueQuest.OnQuestStartFailed"/>.
        /// </summary>
        [ContextMenu("Start Unique Quest")]
        private void StartUniqueQuest()
        {
            var result = QuestAPI.TryStartQuest<ExampleUniqueQuest>(out _);
            if (result)
                Debug.Log("Unique quest started successfully.");
            else
                Debug.Log($"Unique quest start rejected: {result}");
        }

        /// <summary>
        ///     Checks whether any completed or failed instance of the quest exists
        ///     and logs the result — demonstrates <c>IsQuestCompleted</c> / <c>IsQuestFailed</c>.
        /// </summary>
        [ContextMenu("Check Unique Quest State")]
        private void CheckState()
        {
            bool completed = QuestAPI.IsQuestCompleted<ExampleUniqueQuest>();
            bool failed    = QuestAPI.IsQuestFailed<ExampleUniqueQuest>();
            Debug.Log($"ExampleUniqueQuest — completed: {completed}, failed: {failed}");
        }

        private void OnDestroy()
        {
            QuestAPI.ClearAllQuests();
        }
    }
}
