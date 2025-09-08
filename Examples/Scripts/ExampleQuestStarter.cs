using Systems.SimpleQuests.Utility;
using UnityEngine;

namespace Systems.SimpleQuests.Examples.Scripts
{
    public sealed class ExampleQuestStarter : MonoBehaviour
    {
        [ContextMenu("Start Quest")]
        private void StartQuest()
        {
            QuestAPI.TryStartQuest<ExampleQuest>(out _);
        }

        private void OnDestroy()
        {
            QuestAPI.ClearAllQuests();
        }
    }
}