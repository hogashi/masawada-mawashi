using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mawashi : MonoBehaviour
{
  // individual number
  int number = 1;
  Vector2 goal;
  float phase;
  float phaseDiffY;

  // Start is called before the first frame update
  void Start()
  {
    // goal = Vector2();
    int initX = (int)(Camera.main.ScreenToWorldPoint(Input.mousePosition).x * 100);
    Random.InitState(initX);
    goal = new Vector2(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
    Debug.Log("goal : " + goal.x + ", " + goal.y + " : screen : " + Screen.width + ", " + Screen.height);
    Random.InitState(initX + number);
    phase = Random.Range(0, 2 * Mathf.PI);
    phaseDiffY = Mathf.Sin(phase);
  }

  // Update is called once per frame
  void Update()
  {
    Vector3 pos = Input.mousePosition;
    pos.x = (int)pos.x;
    pos.y = (int)pos.y;
    // Vector2
    Debug.Log("goal : " + goal.x + ", " + goal.y + " : pos : " + pos.x + ", " + pos.y);
    float ingrediantX = 3.0f * (pos.x - goal.x) / Screen.width;
    float ingrediantY = (pos.y - goal.y) / Screen.height;
    float xOfSin = (ingrediantX + ingrediantY) * 2.0f * Mathf.PI + phase;

    int newY = (int)((Mathf.Sin(xOfSin) - phaseDiffY) * 180.0f) + 180;
    transform.rotation = Quaternion.Euler(0, newY, 0);
  }
}
