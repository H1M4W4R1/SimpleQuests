using JetBrains.Annotations;
using Systems.SimpleQuests.Abstract;
using Systems.SimpleQuests.Data;
using UnityEngine;

namespace Systems.SimpleQuests.Examples.Scripts
{
    public sealed class ExampleQuest : Quest
    {
        [NotNull] public override QuestInstance Create(QuestInstance instance)
        {
            return instance.WithObjective(new ExampleKeyObjective(KeyCode.A))
                .WithObjective(new ExampleKeyObjective(KeyCode.B))
                .WithObjective(new ExampleKeyObjective(KeyCode.C));
        }

        protected internal override void OnQuestStarted(QuestInstance instance)
        {
            base.OnQuestStarted(instance);
            Debug.Log($"Quest {name} has been started");
        }

        protected internal override void OnQuestCompleted(QuestInstance instance)
        {
            base.OnQuestCompleted(instance);
            Debug.Log($"Quest {name} has been completed");
        }

        protected internal override void OnQuestFailed(QuestInstance instance)
        {
            base.OnQuestFailed(instance);
            Debug.Log($"Quest {name} has been failed");
        }
    }
}