using UnityEngine;

public class AttackUI : MonoBehaviour
{
    [SerializeField]
    private Transform _ThroneOrigin;

    // This seems dumb and like I can do it better with parenting, but I'll revisit this later
    void Update()
    {
        // For a worldspace canvas, we can directly lerp the transform's position
        transform.position = _ThroneOrigin.position;
    }

    public void OnAttackButtonClicked()
    {
        STDB.Conn.Reducers.AttackThrone();
        Debug.Log("Called reducer to attack.");
    }
}
