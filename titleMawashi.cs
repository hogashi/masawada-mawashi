using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class titleMawashi : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    float time = Time.time;
    float xOfSin = time * 2.0f * Mathf.PI / 20.0f;
    int newY = getY(xOfSin);
    transform.rotation = Quaternion.Euler(0, newY, 0);
    Debug.Log("time: " + time + " :y: " + newY);
  }

  int getY(float x) {
    return (int)(Mathf.Sin(x) * 360.0f + 180);
  }
}
