<div align="center">
  <h1>Simple Quests</h1>
</div>

# About

Simple Quests is a package of SimpleKit intended for quick and easy implementation of quest systems with objectives that can be completed, failed, or tracked over time.

*For requirements check .asmdef*

# Creating a Quest

To create a quest you simply need to extend the `Quest` class. Afterward, you can implement the `Create` method to define your objectives and override event methods for quest lifecycle management.

```csharp
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
```

# Creating Quest Objectives

Quest objectives are the individual tasks that make up a quest. To create an objective, extend the `QuestObjective` class and implement the `ShouldBeComplete` method. Optionally you can also implement `ShouldBeFailed` to handle objective failures.

```csharp
public sealed class ExampleKeyObjective : QuestObjective
{
    private readonly KeyCode _key;
    
    public ExampleKeyObjective(KeyCode key)
    {
        _key = key;
    }
    
    public override bool ShouldBeComplete()
    {
        return Input.GetKey(_key);
    }

    protected internal override void OnQuestObjectiveStarted(QuestInstance quest)
    {
        base.OnQuestObjectiveStarted(quest);
        Debug.Log($"Objective to press key {_key} has been started. Press key now");
    }

    protected internal override void OnQuestObjectiveCompleted(QuestInstance quest)
    {
        base.OnQuestObjectiveCompleted(quest);
        Debug.Log($"Objective to press key {_key} has been completed");
    }

    protected internal override void OnQuestObjectiveFailed(QuestInstance quest)
    {
        base.OnQuestObjectiveFailed(quest);
        Debug.Log($"Objective to press key {_key} has been failed");
    }

    protected internal override void OnQuestObjectiveTick(QuestInstance questInstance, float deltaTime)
    {
        base.OnQuestObjectiveTick(questInstance, deltaTime);
        Debug.Log($"Objective to press key {_key} has been ticked");
    }
}
```

# Starting and Managing Quests

The quest system uses the `QuestAPI` utility class to manage quest instances. You can start quests using the `TryStartQuest` method with a type parameter.

```csharp
public sealed class ExampleQuestStarter : MonoBehaviour
{
    [ContextMenu("Start Quest")]
    private void StartQuest()
    {
        QuestAPI.TryStartQuest<ExampleQuest>();
    }

    private void OnDestroy()
    {
        QuestAPI.ClearAllQuests();
    }
}
```

# Quest Lifecycle

Quests follow a specific lifecycle with the following states:
- **Inactive**: Quest has not been started yet
- **InProgress**: Quest is currently active and objectives are being tracked
- **Completed**: All required objectives have been completed
- **Failed**: At least one required objective has failed

# Objective States

Objectives also have their own lifecycle states:
- **Inactive**: Objective is not yet active
- **InProgress**: Objective is currently being tracked
- **Completed**: Objective has been completed successfully
- **Failed**: Objective has failed

# Quest Management

The `QuestAPI` provides several methods for managing quests:

- `TryStartQuest<TQuest>()`: Starts a new quest instance
- `GetAllInstancesOf<TQuest>()`: Gets all instances of a specific quest type
- `GetFirstQuestInstanceOf<TQuest>()`: Gets the first instance of a specific quest type
- `ClearAllQuests()`: Removes all active quest instances

# Objective Management

Objectives are automatically managed by the quest system:
- Only one objective can be active at a time (unless you override this behavior like `CombinedQuestObjective`)
- When an required objective completes or fails, the next inactive required objective and all optional objectives before it becomes active
- Required objectives must be completed for the quest to succeed
- Optional objectives (when `IsRequired` returns false) can be skipped

# Custom Objective Validation

You can override the `ShouldBeFailed` method in your objectives to define custom failure conditions:

```csharp
public override bool ShouldBeFailed()
{
    // Example: Fail if player takes too much damage
    return player.Health < 10f;
}
```

# Quest Database Integration

Quests are automatically registered in the `QuestDatabase` using the `[AutoCreate]` attribute. This allows the system to find and instantiate quests when needed.

For operation construction and error handling, you can review the `QuestOperations` static class.
