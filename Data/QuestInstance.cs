using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleQuests.Abstract;
using Systems.SimpleQuests.Abstract.Markers;
using Systems.SimpleQuests.Data.Enums;
using UnityEngine;

namespace Systems.SimpleQuests.Data
{
    /// <summary>
    ///     Instance of quest created from quest object
    /// </summary>
    public sealed class QuestInstance : IWithObjectives<QuestInstance>
    {
        /// <summary>
        ///     Reference to the quest object this quest is based upon
        /// </summary>
        [field: SerializeReference] [NotNull] private readonly Quest _quest;

        /// <summary>
        ///     Quest object this quest is based upon
        /// </summary>
        [NotNull] public Quest Quest => _quest;

        /// <summary>
        ///     List of all objectives
        /// </summary>
        private readonly List<QuestObjective> _objectives = new();

        /// <summary>
        ///     List of all instance objectives
        /// </summary>
        public IReadOnlyList<QuestObjective> Objectives => _objectives;

        /// <summary>
        ///     State of the quest
        /// </summary>
        public QuestState State { get; private set; } = QuestState.Inactive;

        /// <summary>
        ///     Adds an objective to the list
        /// </summary>
        [NotNull] public QuestInstance WithObjective(QuestObjective objective)
        {
            _objectives.Add(objective);
            return this;
        }

        /// <summary>
        ///     Starts the quest
        /// </summary>
        public void Start()
        {
            State = QuestState.InProgress;
            _quest.OnQuestStarted(this);

            ActivateFirstInactiveObjectiveIfNoneAreInProgress();
        }

        /// <summary>
        ///     Handle quest failure or completion states
        /// </summary>
        void IWithObjectives.AfterQuestIterationComplete()
        {
            if (AreRequiredObjectivesComplete())
            {
                State = QuestState.Completed;
                _quest.OnQuestCompleted(this);
            }

            if (IsAnyRequiredObjectiveFailed())
            {
                State = QuestState.Failed;
                _quest.OnQuestFailed(this);
            }

            ActivateFirstInactiveObjectiveIfNoneAreInProgress();
        }

        /// <summary>
        ///     Checks if all required objectives are completed
        /// </summary>
        private bool AreRequiredObjectivesComplete()
        {
            for (int i = 0; i < Objectives.Count; i++)
            {
                QuestObjective objective = Objectives[i];
                if (!objective.IsRequired) continue;
                if (objective.State != QuestState.Completed) return false;
            }

            return true;
        }

        /// <summary>
        ///     Checks if any required objective is failed
        /// </summary>
        private bool IsAnyRequiredObjectiveFailed()
        {
            for (int i = 0; i < Objectives.Count; i++)
            {
                QuestObjective objective = Objectives[i];
                if (!objective.IsRequired) continue;
                if (objective.State == QuestState.Failed) return true;
            }

            return false;
        }

        /// <summary>
        ///     Creates a new quest instance from a quest object
        /// </summary>
        [NotNull] public static QuestInstance FromQuest([NotNull] Quest fromQuest)
        {
            QuestInstance createdInstance = new(fromQuest);
            QuestInstance instance = fromQuest.Create(createdInstance);
            return instance;
        }

        private QuestInstance([NotNull] Quest fromQuest)
        {
            _quest = fromQuest;
        }

        internal void Tick(float deltaTime)
        {
            IWithObjectives self = this;
            self.TickCompletionStatusCheck(this, deltaTime);
        }

        /// <summary>
        ///     Activates the first inactive objective
        /// </summary>
        private void ActivateFirstInactiveObjectiveIfNoneAreInProgress()
        {
            for (int i = 0; i < Objectives.Count; i++)
                if(Objectives[i].State == QuestState.InProgress) return;
            
            for (int i = 0; i < Objectives.Count; i++)
            {
                QuestObjective objective = Objectives[i];
                if (objective.State != QuestState.Inactive) continue;
                objective.State = QuestState.InProgress;
                objective.OnQuestObjectiveStarted(this);
                return;
            }
        }
    }
}