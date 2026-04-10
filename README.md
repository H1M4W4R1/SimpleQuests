<div align="center">
  <h1>Simple Quests</h1>
</div>

# About

Simple Quests is a lightweight, data-driven quest system for Unity games. It provides a framework for creating quests with multiple objectives that can be combined, sequenced, or run in parallel. Quests are defined as ScriptableObjects and managed through an addressable database, making it easy to organize and load quest content without hard coding.

# Requirements

- **Unity**: 2022.3+
- **SimpleCore**: Required system module (dependency for operation results and tick system)
- **Unity Addressables**: For quest database loading
- **Unity Burst**: Included in assembly references
- **Unity Collections**: Included in assembly references
- **DOTween**: Included in assembly references (optional, for animation support)
- **.NET 4.x** or **.NET Standard 2.1** API Compatibility Level

# Usage Examples

## Creating a Custom Quest

Create a quest by extending the `Quest` base class and implementing objectives:

```csharp
using Systems.SimpleQuests.Abstract;
using Systems.SimpleQuests.Data;

public class MyQuest : Quest
{
    public override QuestInstance Create()
    {
        return base.Create()
            .WithObjective(new CollectItemObjective(5))
            .WithObjective(new DefeatEnemyObjective());
    }

    protected internal override void OnQuestStarted(QuestInstance instance)
    {
        Debug.Log($"Quest started: {name}");
    }

    protected internal override void OnQuestCompleted(QuestInstance instance)
    {
        Debug.Log($"Quest completed: {name}");
        // Reward player here
    }

    protected internal override void OnQuestFailed(QuestInstance instance)
    {
        Debug.Log($"Quest failed: {name}");
    }
}
```

## Creating Custom Objectives

Extend `QuestObjective` and implement completion/failure logic:

```csharp
using Systems.SimpleQuests.Abstract;
using Systems.SimpleQuests.Data;

public class CollectItemObjective : QuestObjective
{
    private int _itemsCollected;
    private readonly int _itemsNeeded;

    public CollectItemObjective(int itemsNeeded)
    {
        _itemsNeeded = itemsNeeded;
    }

    public override bool ShouldBeComplete() => _itemsCollected >= _itemsNeeded;

    public void OnItemCollected()
    {
        _itemsCollected++;
    }

    protected internal override void OnQuestObjectiveStarted(QuestInstance quest)
    {
        Debug.Log($"Collect {_itemsNeeded} items");
    }

    protected internal override void OnQuestObjectiveCompleted(QuestInstance quest)
    {
        Debug.Log("Collection objective completed!");
    }
}
```

## Starting and Managing Quests

Use the `QuestAPI` to start, complete, and track quests:

```csharp
using Systems.SimpleQuests.Utility;

// Start a quest
var result = QuestAPI.TryStartQuest<MyQuest>(out QuestInstance questInstance);
if (result)
{
    Debug.Log("Quest started successfully");
}

// Start a unique quest (only one instance allowed at a time)
QuestAPI.TryStartUniqueQuest<MyQuest>(out questInstance);

// Force quest completion
QuestAPI.CompleteQuest<MyQuest>();

// Force quest failure
QuestAPI.FailQuest<MyQuest>();

// Get active quest
var activeQuest = QuestAPI.GetFirstActiveQuestOfType<MyQuest>();

// Get all finished quests
var finishedQuests = QuestAPI.GetAllFinishedQuestsOfType<MyQuest>();

// Clear all quests
QuestAPI.ClearAllQuests();
```

## Combining Objectives

Use `CombinedQuestObjective` to activate multiple objectives simultaneously:

```csharp
var combined = new CombinedQuestObjective()
    .WithObjective(new KillEnemyObjective())
    .WithObjective(new CollectLootObjective());

questInstance.WithObjective(combined);
```

## Checking Quest States

Monitor quest progress using `QuestState`:

```csharp
using Systems.SimpleQuests.Data.Enums;

// Quest states: Hidden, Inactive, InProgress, Completed, Failed
if (questInstance.State == QuestState.Completed)
{
    Debug.Log("Quest is complete!");
}

// Check individual objectives
foreach (var objective in questInstance.Objectives)
{
    if (objective.State == QuestState.InProgress)
    {
        Debug.Log($"Current objective: {objective}");
    }
}
```

## Optional vs Required Objectives

Create optional objectives by overriding `IsRequired`:

```csharp
public class BonusObjective : QuestObjective
{
    public override bool IsRequired => false; // Optional objective
    
    // ... implementation
}
```

Optional objectives can fail without failing the quest, allowing for bonus tasks and side objectives.

# Architecture Overview

- **Quest**: Base class for quest definitions (ScriptableObject)
- **QuestInstance**: Runtime instance of a quest with state tracking
- **QuestObjective**: Base class for individual quest tasks
- **QuestDatabase**: Addressable database for quest management
- **QuestAPI**: Static API for quest management and querying
- **QuestState**: Enum tracking quest and objective progression
- **CombinedQuestObjective**: Composite pattern for grouped objectives

The system integrates with the SimpleCore tick system for automatic quest updates each frame.
