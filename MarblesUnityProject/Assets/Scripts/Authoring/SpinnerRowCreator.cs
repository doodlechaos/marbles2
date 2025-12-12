using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpinnerRowCreator : MonoBehaviour
{
    public int NumbSpinners = 7;
    public GameObject SpinnerPrefab;
    public Vector3 SpinnerScale = new Vector3(1, 1, 1);
    public float SpanWidth = 10f;

    void OnValidate()
    {
        EditorApplication.delayCall += () =>
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < NumbSpinners; i++)
            {
                GameObject spinner = Instantiate(SpinnerPrefab, transform);
                float t = (float)i / (NumbSpinners - 1);
                spinner.transform.localPosition = Vector3.Lerp(
                    new Vector3(-SpanWidth / 2, 0, 0),
                    new Vector3(SpanWidth / 2, 0, 0),
                    t
                );
                spinner.transform.localScale = SpinnerScale;
            }
        };
    }
}
