using UnityEngine;

public class Focusable : MonoBehaviour
{
    [SerializeField]
    private Material focusMaterial;
    private Renderer rend;

    private void Start()
    {
        this.rend = GetComponent<Renderer>();
        this.rend.enabled = true;
        this.rend.sharedMaterial = focusMaterial;
    }
    
    public void ApplyFocus(Vector2 centerPoint, float focusRadius)
    {
        focusMaterial.SetVector("_CenterPoint", centerPoint);
        focusMaterial.SetFloat("_FocusRadius", focusRadius);
    }
}
