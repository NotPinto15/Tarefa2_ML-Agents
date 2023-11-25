using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    public GameObject patientPrefab;
    public int numPatients;

    void Start() {

        for (int i = 0; i < numPatients; ++i) {

            Instantiate(patientPrefab, this.transform.position, Quaternion.identity);
        }

        Invoke("SpawnPatient", 5.0f);
    }

    void SpawnPatient() {

        Instantiate(patientPrefab, this.transform.position, Quaternion.identity);
        Invoke("SpawnPatient", Random.Range(2.0f, 10.0f));
    }

    void Update() {

    }
}