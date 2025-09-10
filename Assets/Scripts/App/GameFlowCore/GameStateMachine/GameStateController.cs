using System;
using System.Collections.Generic;

using UnityEngine;

public class GameStateController
{
    private IGameState current;
    private readonly Dictionary<GameStateId, IGameState> states = new();
    public GameStateId CurrentStateId { get; private set; } = GameStateId.None;

    public GameStateController(GameStateContext gameStateContext)
    {
        if (gameStateContext == null)
        {
            throw new ArgumentNullException(nameof(gameStateContext));
        }

        states[GameStateId.Pause] = new PausedState(gameStateContext);
        states[GameStateId.Gameplay] = new GameplayState(gameStateContext);
        states[GameStateId.GameOver] = new GameOverState(gameStateContext);
    }

    public void Update()
    {
        current?.UpdateState();
    }

    public void SetState(GameStateId stateId)
    {
        if (!states.TryGetValue(stateId, out var next))
        {
            Debug.LogError($"GameStateController: state {stateId} not registered.");
            return;
        }
        ChangeState(next, stateId);
    }

    private void ChangeState(IGameState next, GameStateId stateId)
    {
        current?.ExitState();
        current = next;
        CurrentStateId = stateId;
        current.EnterState();
        //Debug.Log($"GameStateMachine: state -> {CurrentStateId}");
    }

    public bool IsInState(GameStateId stateId)
    {
        return CurrentStateId == stateId;
    }

    public void ToGameplay()
    {
        SetState(GameStateId.Gameplay);
    }

    public void ToGameOver()
    {
        SetState(GameStateId.GameOver);
    }

    public void ToPause()
    {
        SetState(GameStateId.Pause);
    }
}
