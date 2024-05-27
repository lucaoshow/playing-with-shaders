using UnityEngine;

public class FocusablesManager : MonoBehaviour
{
    private Focusable[] focusables;
    private bool focusing;
    private bool focusingSpecific;
    private float focusRadius;
    
    private Vector2 centerPoint;

    private float maxRadius;

    void Start()
    {
        this.focusables = GameObject.FindObjectsOfType<Focusable>();
        this.focusing = false;
        this.focusRadius = 0f;
    }

void Update()
    {
        if (this.focusing || this.focusingSpecific)
        {
            this.focusRadius = Mathf.Clamp(this.focusRadius + Time.deltaTime * 30f, 0f, this.maxRadius);
            foreach (var focusable in this.focusables)
            {
                focusable.ApplyFocus(this.centerPoint, this.focusRadius);
            }
        }
        else if (this.focusRadius > 0)
        {
            this.focusRadius = Mathf.Clamp(this.focusRadius - Time.deltaTime * 35f, 0f, this.maxRadius);
            foreach (var focusable in this.focusables)
            {
                focusable.ApplyFocus(this.centerPoint, this.focusRadius);
            }
        }
    }

    public void StartFocusing()
    {
        this.centerPoint = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
        this.maxRadius = 42f;
        this.focusing = true;
    }

    public void StartFocusingSpecific()
    {
        this.centerPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.maxRadius = 7f;
        this.focusingSpecific = true;
    }

    public void StopFocusing()
    {
        this.focusing = false;
        this.focusingSpecific = false;
    }
}
