using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubGoal {

    public Dictionary<string, int> sGoals;
    public bool remove;


    public SubGoal(string s, int i, bool r) {

        sGoals = new Dictionary<string, int>();
        sGoals.Add(s, i);
        remove = r;
    }
}

public class GAgent : MonoBehaviour {

    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    public GInventory inventory = new GInventory();
    public WorldStates beliefs = new WorldStates();
    bool invoked = false;

    GPlanner planner;
    Queue<GAction> actionQueue;
    public GAction currentAction;
    SubGoal currentGoal;

    protected virtual void Start() {

        GAction[] acts = this.GetComponents<GAction>();
        foreach (GAction a in acts) {

            actions.Add(a);
        }
    }

    void CompleteAction() {

        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
    }

    void LateUpdate() {

        if (currentAction != null && currentAction.running) {

            float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position);
            if (currentAction.agent.hasPath && distanceToTarget < 2.0f)//currentAction.agent.remainingDistance < 0.5f)
                {

                if (!invoked) {

                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }

        if (planner == null || actionQueue == null) {

            planner = new GPlanner();
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach (KeyValuePair<SubGoal, int> sg in sortedGoals) {

                actionQueue = planner.plan(actions, sg.Key.sGoals, beliefs);

                if (actionQueue != null) {

                    currentGoal = sg.Key;
                    break;
                }
            }
        }

        if (actionQueue != null && actionQueue.Count == 0) {

            if (currentGoal.remove) {

                goals.Remove(currentGoal);
            }
            planner = null;
        }

        if (actionQueue != null && actionQueue.Count > 0) {

            currentAction = actionQueue.Dequeue();

            if (currentAction.PrePerform()) {

                if (currentAction.target == null && currentAction.targetTag != "") {

                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }

                if (currentAction.target != null) {

                    currentAction.running = true;
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }
            } else {

                actionQueue = null;
            }
        }
    }
}
