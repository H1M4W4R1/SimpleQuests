using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleCore.Operations;
using Systems.SimpleCore.Storage;
using Systems.SimpleCore.Timing;
using Systems.SimpleQuests.Abstract;
using Systems.SimpleQuests.Data;
using Systems.SimpleQuests.Operations;
using UnityEngine.Assertions;

namespace Systems.SimpleQuests.Utility
{
    public static class QuestAPI
    {
        /// <summary>
        ///     Variable to check if the tick system is already hooked
        /// </summary>
        private static bool _isTickSystemHooked;

        /// <summary>
        ///     List of current quests
        /// </summary>
        private static readonly List<QuestInstance> _currentQuests = new();

        /// <summary>
        ///     Removes all quests from the list
        /// </summary>
        public static void ClearAllQuests() => _currentQuests.Clear();

        /// <summary>
        ///     Forces a quest to finish
        /// </summary>
        /// <typeparam name="TQuest">The quest to finish</typeparam>
        /// <returns>True if the quest was found and finished, false otherwise</returns>
        public static bool CompleteQuest<TQuest>()
            where TQuest : Quest, new()
        {
            // Find instance and finish it
            for (int i = 0; i < _currentQuests.Count; i++)
            {
                if (_currentQuests[i].Quest is not TQuest) continue;
                _currentQuests[i].ForceFinish();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Forces a quest to finish
        /// </summary>
        /// <param name="quest">Quest to finish</param>
        /// <returns>True if the quest was found and finished, false otherwise</returns>
        public static bool CompleteQuest([NotNull] Quest quest)
        {
            // Find instance and finish it
            for (int i = 0; i < _currentQuests.Count; i++)
            {
                if (!ReferenceEquals(_currentQuests[i].Quest, quest)) continue;
                _currentQuests[i].ForceFinish();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Forces a quest to finish
        /// </summary>
        /// <typeparam name="TQuest">The quest to finish</typeparam>
        /// <returns>True if the quest was found and finished, false otherwise</returns>
        public static bool FailQuest<TQuest>()
            where TQuest : Quest, new()
        {
            // Find instance and finish it
            for (int i = 0; i < _currentQuests.Count; i++)
            {
                if (_currentQuests[i].Quest is not TQuest) continue;
                _currentQuests[i].ForceFail();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Forces a quest to finish
        /// </summary>
        /// <param name="quest">Quest to finish</param>
        /// <returns>True if the quest was found and finished, false otherwise</returns>
        public static bool FailQuest([NotNull] Quest quest)
        {
            // Find instance and finish it
            for (int i = 0; i < _currentQuests.Count; i++)
            {
                if (!ReferenceEquals(_currentQuests[i].Quest, quest)) continue;
                _currentQuests[i].ForceFail();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Tries to start a quest
        /// </summary>
        public static OperationResult<QuestInstance> TryStartQuest<TQuest>()
            where TQuest : Quest, new()
        {
            // Ensure tick system exists
            TickSystem.EnsureExists();

            // and is hooked properly
            if (!_isTickSystemHooked)
            {
                TickSystem.OnTick += OnTick;
                _isTickSystemHooked = true;
            }

            // Create new instance
            TQuest quest = QuestDatabase.GetExact<TQuest>();
            Assert.IsNotNull(quest, "Quest was not found in database");

            // Check if quest can be started
            OperationResult result = quest.CanBeStarted();
            if (!result)
            {
                quest.OnQuestStartFailed(result);
                return result.WithData<QuestInstance>(null);
            }

            // Create instance, start it and add to list
            QuestInstance instance = QuestInstance.FromQuest(quest);
            instance.Start();
            _currentQuests.Add(instance);
            return QuestOperations.Started().WithData(instance);
        }

        /// <summary>
        ///     Gets all quest instances of a quest
        /// </summary>
        public static ROListAccess<TQuest> GetAllInstancesOf<TQuest>()
            where TQuest : Quest
        {
            RWListAccess<TQuest> list = RWListAccess<TQuest>.Create();
            List<TQuest> refList = list.List;

            for (int i = 0; i < _currentQuests.Count; i++)
            {
                if (_currentQuests[i].Quest is TQuest requestedQuest) refList.Add(requestedQuest);
            }
            
            return list.ToReadOnly();
        }

        /// <summary>
        ///     Gets all quest instances of a quest
        /// </summary>
        public static ROListAccess<QuestInstance> GetAllInstancesOf([NotNull] Quest quest)
        {
            RWListAccess<QuestInstance> list = RWListAccess<QuestInstance>.Create();
            List<QuestInstance> refList = list.List;

            for (int i = 0; i < _currentQuests.Count; i++)
            {
                if (ReferenceEquals(_currentQuests[i].Quest, quest)) refList.Add(_currentQuests[i]);
            }

            return list.ToReadOnly();
        }

        /// <summary> 
        ///     Gets the first quest instance of a quest
        /// </summary>
        [CanBeNull] public static TQuest GetFirstQuestInstanceOf<TQuest>()
            where TQuest : Quest
        {
            for (int i = 0; i < _currentQuests.Count; i++)
            {
                if (_currentQuests[i].Quest is TQuest requestedQuest) return requestedQuest;
            }

            return null;
        }

        /// <summary>
        ///     Gets the first quest instance of a quest
        /// </summary>
        [CanBeNull] public static QuestInstance GetFirstQuestInstanceOf([NotNull] Quest quest)
        {
            for (int i = 0; i < _currentQuests.Count; i++)
            {
                if (ReferenceEquals(_currentQuests[i].Quest, quest)) return _currentQuests[i];
            }

            return null;
        }

        /// <summary>
        ///     Should be called every frame or every turn to update objectives and check if quest is completed
        ///     or failed
        /// </summary>
        private static void OnTick(float deltaTime)
        {
            for (int i = 0; i < _currentQuests.Count; i++) _currentQuests[i].Tick(deltaTime);
        }
    }
}