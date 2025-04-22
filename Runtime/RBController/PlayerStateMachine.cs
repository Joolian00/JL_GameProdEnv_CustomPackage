using System;
using System.Collections.Generic;

using Julian.RBController.States;

using UnityEngine;

namespace Julian.RBController
{
    public enum PlayerStateType
    {
        Idle,
        Moving,
        Jumping,
        Falling,
        Stunned,
        Lobby
    }
    
    public class PlayerStateMachine
{
    /// <summary>
    /// The controller instance associated with this state machine.
    /// Provides access to components and context.
    /// </summary>
    public RigidbodyController Controller { get; private set; }

    /// <summary>
    /// Gets the type of the currently active state.
    /// </summary>
    public PlayerStateType CurrentStateType { get; private set; }

    private PlayerBaseState _currentState;
    private readonly Dictionary<PlayerStateType, PlayerBaseState> _states = new();

    /// <summary>
    /// Initializes a new instance of the PlayerStateMachine class.
    /// </summary>
    /// <param name="controller">The RigidbodyController this state machine belongs to.</param>
    public PlayerStateMachine(RigidbodyController controller)
    {
        Controller = controller ?? throw new ArgumentNullException(nameof(controller));
        CurrentStateType = PlayerStateType.Idle; // Default, will be overwritten by first ChangeState
        _currentState = null; // Start with no state explicitly active
    }

    /// <summary>
    /// Registers a state instance with the state machine.
    /// </summary>
    /// <param name="stateType">The enum type representing the state.</param>
    /// <param name="state">The concrete state instance.</param>
    public void RegisterState(PlayerStateType stateType, PlayerBaseState state)
    {
        if (state == null)
        {
            Debug.LogError($"Attempted to register a null state for type {stateType}.");
            return;
        }
        if (_states.ContainsKey(stateType))
        {
            Debug.LogWarning($"State type {stateType} already registered. Overwriting.");
        }
        _states[stateType] = state;
    }

    /// <summary>
    /// Transitions the state machine to a new state.
    /// Calls ExitState() on the current state and EnterState() on the new state.
    /// </summary>
    /// <param name="newStateType">The type of the state to transition to.</param>
    public void ChangeState(PlayerStateType newStateType)
    {
        // Prevent transitioning to the same state (optional optimization)
        if (CurrentStateType == newStateType && _currentState != null) return;

        if (!_states.TryGetValue(newStateType, out PlayerBaseState newState))
        {
            Debug.LogError($"State {newStateType} not registered in the state machine. Cannot transition.");
            return;
        }

        // Exit the current state, if one exists
        _currentState?.ExitState();

        // Set the new state
        _currentState = newState;
        CurrentStateType = newStateType;

        // Enter the new state
        // Debug.Log($"Entering State: {newStateType}"); // Useful for debugging
        _currentState.EnterState();
    }

    /// <summary>
    /// Called every fixed physics frame. Delegates the FixedUpdate call to the current state.
    /// </summary>
    public void FixedUpdateState()
    {
        _currentState?.FixedUpdateState();
    }
    
    /// <summary>
    /// Called every frame. Delegates the Update call to the current state.
    /// Also allows the current state to check for potential transitions.
    /// </summary>
    public void UpdateState()
    {
        _currentState?.UpdateState();
        _currentState?.CheckSwitchState(); // Check for transitions after update logic
    }
    
    /// <summary>
    /// Called every frame after all Update calls. Delegates the LateUpdate call to the current state.
    /// Typically used for final adjustments or cleanup after all other updates.
    /// </summary>
    public void LateUpdateState()
    {
        _currentState?.LateUpdateState();
    }


    /// <summary>
    /// Gets the instance of the currently active state.
    /// </summary>
    /// <returns>The current PlayerBaseState instance, or null if no state is active.</returns>
    public PlayerBaseState GetCurrentState()
    {
        return _currentState;
    }
}
}