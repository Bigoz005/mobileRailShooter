using UnityEngine;
using System.Collections;

public class AnimationScript : MonoBehaviour
{

    public bool isAnimated = false;

    [SerializeField] private bool isRotating = false;
    [SerializeField] private bool isFloating = false;
    [SerializeField] private bool isScaling = false;

    [SerializeField] private Vector3 rotationAngle;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float floatSpeed;
    [SerializeField] private float floatRate;

    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;

    private bool scalingUp = true;
    [SerializeField] private float scaleSpeed;
    [SerializeField] private float scaleRate;
    private float scaleTimer;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimated)
        {
            if (isRotating)
            {
                transform.Rotate(rotationAngle * rotationSpeed * Time.deltaTime);
            }

            if (isFloating)
            {
                Vector3 moveDir = new Vector3(0.0f, 0.0f, floatSpeed * Time.deltaTime * 1000);
                transform.Translate(moveDir);
            }

            if (isScaling)
            {
                scaleTimer += Time.deltaTime;

                if (scalingUp)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, endScale, scaleSpeed * Time.deltaTime);
                }
                else if (!scalingUp)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, startScale, scaleSpeed * Time.deltaTime);
                }

                if (scaleTimer >= scaleRate)
                {
                    if (scalingUp) { scalingUp = false; }
                    else if (!scalingUp) { scalingUp = true; }
                    scaleTimer = 0;
                }
            }
        }
    }
}
