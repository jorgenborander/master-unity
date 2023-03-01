using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;

[Serializable]
[AddRandomizerMenu("Perception/Dice Placement Randomizer")]
public class DicePlacementRandomizer : Randomizer
{
    public FloatParameter diceScale;
    public Vector3Parameter placementLocation;
    public Vector3Parameter diceRotation;

    public CategoricalParameter<GameObject> prefabs;

    private GameObject currentInstance;

    protected override void OnIterationStart()
    {
        currentInstance = GameObject.Instantiate(prefabs.Sample());

        currentInstance.transform.position = placementLocation.Sample();
        currentInstance.transform.rotation = Quaternion.Euler(diceRotation.Sample());
        currentInstance.transform.localScale = Vector3.one * diceScale.Sample();

    }


    protected override void OnIterationEnd()
    {
        GameObject.Destroy(currentInstance);
    }





    
}
