using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    private bool hasBeenVisible = false;

    public void Initialize()
    {
        hasBeenVisible = false;
    }

    private void OnBecameVisible()
    {
        if (!hasBeenVisible)
        {
            hasBeenVisible = true;
            Debug.Log($"{gameObject.name} ‚ªƒvƒŒƒCƒ„[‚Ì‹ŠE‚É“ü‚è‚Ü‚µ‚½");
        }
    }
}