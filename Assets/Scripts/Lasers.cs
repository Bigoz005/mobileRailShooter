using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasers : MonoBehaviour
{
    public Material[] lasersMaterial;

    public GameObject laser1object;
    public GameObject laser2object;
    public GameObject laser3object;

    void OnEnable()
    {
        StartCoroutine(SwapMaterials(0.5f));
    }

    private IEnumerator SwapMaterials(float time)
    {
        int i = 1;
        while (transform.gameObject.activeInHierarchy)
        {
            switch (i % 3)
            {
                case 0:
                    laser1object.GetComponent<MeshRenderer>().material = lasersMaterial[0];
                    laser2object.GetComponent<MeshRenderer>().material = lasersMaterial[1];
                    laser3object.GetComponent<MeshRenderer>().material = lasersMaterial[2];
                    break;
                case 1:
                    laser1object.GetComponent<MeshRenderer>().material = lasersMaterial[1];
                    laser2object.GetComponent<MeshRenderer>().material = lasersMaterial[2];
                    laser3object.GetComponent<MeshRenderer>().material = lasersMaterial[0];
                    break;
                case 2:
                    laser1object.GetComponent<MeshRenderer>().material = lasersMaterial[2];
                    laser2object.GetComponent<MeshRenderer>().material = lasersMaterial[1];
                    laser3object.GetComponent<MeshRenderer>().material = lasersMaterial[0];
                    break;
            }

            i++;
            yield return new WaitForSeconds(time);
        }

        yield return null;
    }
}
