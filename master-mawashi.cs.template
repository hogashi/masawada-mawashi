using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mawashi__namei__ : MonoBehaviour
{
  // individual number
  int number = __numberi__;
  Vector2 goal;
  int goalY;
  float phase;
  float phaseDiffY;

  // Start is called before the first frame update
  void Start()
  {
    int initX = (int)(Camera.main.ScreenToWorldPoint(Input.mousePosition).x * 100);
    Random.InitState(initX);
    goal = new Vector2((int)(Random.Range(0, Screen.width)), (int)(Random.Range(0, Screen.height)));
    Debug.Log("goal : " + goal.x + ", " + goal.y + " : screen : " + Screen.width + ", " + Screen.height);

    Random.InitState(initX + number);
    phase = Random.Range(0, 2 * Mathf.PI);
    phaseDiffY = Mathf.Sin(phase);
    float xOfSin = phase;
    goalY = getY(xOfSin);
  }

  // Update is called once per frame
  void Update()
  {
    Vector3 pos = Input.mousePosition;
    pos.x = (int)pos.x;
    pos.y = (int)pos.y;
    // Vector2
    float ingrediantX = ((float)number / 3.0f) * (pos.x - goal.x) / (Screen.width * 3.0f);
    float ingrediantY = ((float)(7 - number) / 3.0f) * (pos.y - goal.y) / (Screen.height * 3.0f);
    float xOfSin = (ingrediantX + ingrediantY) * 2.0f * Mathf.PI + phase;
    int newY = getY(xOfSin);
    transform.rotation = Quaternion.Euler(0, newY, 0);
    Debug.Log("goal: " + goal.x + ", " + goal.y + " :pos: " + pos.x + ", " + pos.y + " :y: " + goalY + ", " + newY);
  }

  int getY(float x) {
    return (int)(((Mathf.Sin(x) - phaseDiffY) * 360.0f * ((number - 1.0f) / 3.0f + 1.0f)) / 12.0f) * 12 + 180;
  }
}
