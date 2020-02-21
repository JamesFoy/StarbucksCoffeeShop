using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepManagement : MonoBehaviour
{
    private void Start()
    {
        transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        StartCoroutine(RandomAnimationRoutine());
    }
    private IEnumerator RandomAnimationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 7f));
            GetComponent<Animation>().Play();
        }
    }
}