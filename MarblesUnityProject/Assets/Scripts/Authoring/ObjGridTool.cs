using UnityEditor;
using UnityEngine;

public class ObjGridTool : MonoBehaviour
{
    public int Rows = 1;
    public int Columns = 9;

    public GameObject ObjectPrefab;

    public Vector3 ObjectScale = new Vector3(1, 1, 1);
    public float VerticalSpacing = 1f;
    public float HorizontalSpacing = 1f;

    void OnValidate()
    {
        if (!enabled)
            return;

        EditorApplication.delayCall += () =>
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    GameObject obj =
                        PrefabUtility.InstantiatePrefab(ObjectPrefab, transform) as GameObject; //Instantiate(ObjectPrefab, transform);
                    float xCenter = (Columns - 1) * 0.5f;
                    float yCenter = (Rows - 1) * 0.5f;

                    float xPos = (x - xCenter) * HorizontalSpacing;
                    float yPos = (y - yCenter) * VerticalSpacing;
                    obj.transform.localPosition = new Vector3(xPos, yPos, 0);
                    obj.transform.localScale = ObjectScale;
                }
            }
        };
    }
}
