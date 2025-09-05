using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleQuests.Abstract;
using Systems.SimpleQuests.Abstract.Markers;

namespace Systems.SimpleQuests.Objectives
{
    /// <summary>
    ///     Objective consisting of multiple objectives to complete that
    ///     are activated at same time
    /// </summary>
    public sealed class CombinedQuestObjective : QuestObjective, IWithObjectives<CombinedQuestObjective>
    {
        /// <summary>
        ///     List of objectives that need to be completed
        /// </summary>
        private readonly List<QuestObjective> _objectives = new();
        
        /// <summary>
        ///     Access to list of objectives that need to be completed
        /// </summary>
        public IReadOnlyList<QuestObjective> Objectives => _objectives;

        /// <summary>
        ///     Adds an objective to the list
        /// </summary>
        [NotNull] public CombinedQuestObjective WithObjective([NotNull] QuestObjective objective)
        {
            _objectives.Add(objective);
            return this;
        }

        /// <summary>
        ///     Checks if all required objectives are completed
        /// </summary>
        public override bool ShouldBeComplete()
        {
            for (int i = 0; i < _objectives.Count; i++)
            {
                // Skip non-required objectives
                QuestObjective objective = _objectives[i];
                if (!objective.IsRequired) continue;
                
                if (!objective.ShouldBeComplete()) return false;
            }
            
            // If all required objectives are completed, return true
            return true;
        }

        /// <summary>
        ///     Checks if any required objective is failed
        /// </summary>
        public override bool ShouldBeFailed()
        {
            for (int i = 0; i < _objectives.Count; i++)
            {
                // Skip non-required objectives
                QuestObjective objective = _objectives[i];
                if (!objective.IsRequired) continue;
            
                // If any required objective is failed, return true
                if (objective.ShouldBeFailed()) return true;
            }
            
            return false;
        }

        void IWithObjectives.AfterQuestIterationComplete()
        {
            // This should be handled by quest instance
        }
    }
}