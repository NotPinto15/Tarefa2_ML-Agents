using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldState {

    public string key;
    public int value;
}

public class WorldStates {

    public Dictionary<string, int> states;

    public WorldStates() {

        states = new Dictionary<string, int>();
    }

    bool HasState(string key) => states.ContainsKey(key);

    void AddState(string key, int value) {

        states.Add(key, value);
    }

    void RemoveState(string key) {

        if (HasState(key)) states.Remove(key);
    }

    public void ModifyState(string key, int value) {

        if (HasState(key)) {

            states[key] += value;
            if (states[key] <= 0) RemoveState(key);
        } else {

            AddState(key, value);
        }
    }

    void SetState(string key, int value) {

        if (HasState(key)) {

            states[key] = value;
        } else {

            AddState(key, value);
        }
    }

    public Dictionary<string, int> GetStates() => states;
}