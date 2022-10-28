using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

#region State

public enum AIFollowerStateId
{
    ChasePlayer,
    ChaseAndAttackPlayer
}
public interface AIFollowerState
{
    AIFollowerStateId GetId();
    void Enter(AIFollowerAgent agent);
    void Update(AIFollowerAgent agent);
    void Condition(AIFollowerAgent agent);
    void Exit(AIFollowerAgent agent);
}
public class AIFollowerChaseState : AIFollowerState
{
    public AIFollowerStateId GetId()
    {
        return AIFollowerStateId.ChasePlayer;
    }

    GameObject target;

    public void Enter(AIFollowerAgent agent)
    {
        target = agent.playerData.player;
    }

    public void Update(AIFollowerAgent agent)
    {

        if (!agent._active) return;
        agent._lastPosition = agent.transform.position;

        agent.RunCollisionChecks();

        agent.CalculateWalk();
        agent.CalculateJumpApex();
        agent.CalculateGravity();
        agent.CalculateJump();

        agent.MoveCharacter();

        if (target != null)
        {
            var Dir = agent.transform.position - target.transform.position;
            var targetAngle = Mathf.Atan2(Dir.x, -Dir.y) * Mathf.Rad2Deg;

            if (Dir.magnitude > 85) { Object.Destroy(agent.gameObject); ; }

            if (agent.AIstats.Canons != null)
                for (int i = 0; i < agent.AIstats.Canons.Count; i++)
                {
                    agent.AIstats.CanonsRotation[i].transform.rotation = Quaternion.Slerp(agent.AIstats.CanonsRotation[i].transform.rotation,
                                                                         Quaternion.Euler(0, 0, targetAngle),
                                                                         agent.AIstats.Speed * 10 * Time.deltaTime);
                }
        }
    }

    public void Condition(AIFollowerAgent agent)
    {
        if (target != null && (target.transform.position - agent.transform.position).magnitude < agent.AIstats.RangeDetectionShoot)
        {
            agent.stateMachine.ChangeState(AIFollowerStateId.ChaseAndAttackPlayer);
        }
    }

    public void Exit(AIFollowerAgent agent)
    {

    }
}
public class AIFollowerChaseAndAttackState : AIFollowerState
{
    public AIFollowerStateId GetId()
    {
        return AIFollowerStateId.ChaseAndAttackPlayer;
    }

    GameObject target;

    public void Enter(AIFollowerAgent agent)
    {
        target = agent.playerData.player;
    }

    public void Update(AIFollowerAgent agent)
    {

        if (!agent._active) return;
        agent._lastPosition = agent.transform.position;

        agent.RunCollisionChecks();

        agent.CalculateWalk();
        agent.CalculateJumpApex();
        agent.CalculateGravity();

        agent.MoveCharacter();

        if (target != null)
        {
            var Dir = agent.transform.position - target.transform.position;
            var targetAngle = Mathf.Atan2(Dir.x, -Dir.y) * Mathf.Rad2Deg;

            for (int i = 0; i < agent.AIstats.Canons.Count; i++)
            {
                agent.AIstats.CanonsRotation[i].transform.rotation = Quaternion.Slerp(agent.AIstats.CanonsRotation[i].transform.rotation,
                                                       Quaternion.Euler(0, 0, targetAngle),
                                                       agent.AIstats.Speed * Time.deltaTime);
            }

            foreach (var canon in agent.AIstats.CanonsScripts)
            {
                canon.Shoot();
            }
        }
    }
    public void Condition(AIFollowerAgent agent)
    {
        if (target != null && (target.transform.position - agent.transform.position).magnitude > agent.AIstats.RangeDetectionShoot)
        {
            agent.stateMachine.ChangeState(AIFollowerStateId.ChasePlayer);
        }
    }

    public void Exit(AIFollowerAgent agent)
    {

    }

}

#endregion

public class AIFollowerAgent : AIAgent
{
    public AIFollowerStateId initialState;
    public AIFollowerStateMachine stateMachine;

    public override void _Start()
    {
        stateMachine = new AIFollowerStateMachine(this);
        stateMachine.RegisterState(new AIFollowerChaseState());
        stateMachine.RegisterState(new AIFollowerChaseAndAttackState());
        stateMachine.StartingState(initialState);
    }

    private void FixedUpdate()
    {
        stateMachine.Update();
    }
}

[System.Serializable]
public class AIFollowerStateMachine
{
    public AIFollowerState[] states;
    public AIFollowerAgent agent;
    public AIFollowerStateId currentState;

    public AIFollowerStateMachine(AIFollowerAgent agent)
    {
        this.agent = agent;
        int numStates = System.Enum.GetNames(typeof(AIFollowerStateId)).Length;
        states = new AIFollowerState[numStates];
    }

    public void RegisterState(AIFollowerState state)
    {
        int index = (int)state.GetId();
        states[index] = state;
    }

    public AIFollowerState GetState(AIFollowerStateId stateId)
    {
        int index = (int)stateId;
        return states[index];
    }

    public void Update()
    {
        GetState(currentState)?.Update(agent);
        conditionForState();
    }

    public void conditionForState()
    {
        GetState(currentState)?.Condition(agent);
    }

    public void ChangeState(AIFollowerStateId newState)
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }

    public void StartingState(AIFollowerStateId newState)
    {
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
}
