using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mawashi : MonoBehaviour
{
  // 定数
  int numberN;
  float dpi;
  float weakenNumber;
  float weakenPeriod;
  float angleAtPeriod;
  float resolution;
  int hintRadius;

  float startTime;
  float goalTime;
  int isParade;
  // GameObject[] hint;
  GameObject hint;
  GameObject[] masawadas;
  Vector2 goal;
  float[] phase;
  float[] phaseDiffY;

  // Start is called before the first frame update
  void Start()
  {
    // 定数
    // 6人いる
    numberN = 6;
    // 2 \pi
    dpi = 2.0f * Mathf.PI;
    // 連番は6まであって6倍は効きすぎるので弱める
    weakenNumber = 3.0f;
    // 幅や高さで割ることで端から端までの周期を同じにするが,
    // 幅や高さは数百あるので弱めて周期を減らす
    weakenPeriod = 3.0f;
    // 連番1が1周期で何度回るか(連番次第で回転数は上がる)
    angleAtPeriod = 360.0f;
    // マウス操作にあまりにシビアだとクリアできないので,
    // 回転角の解像度を落とす
    resolution = 12.0f;
    // ヒントの半径
    hintRadius = 80;

    // 始めた時間
    startTime = Time.time;
    // 最初にゴールした時間
    goalTime = 0.0f;
    // パレード中かどうか
    isParade = 0;

    // ゴール座標は毎回違うけど6人全員同じにするために,
    // マウス位置を乱数シードにする
    // (細かな違いまで使って偶然性を出すために小数第2位まで使う)
    int initX = (int)(
      Camera.main.ScreenToWorldPoint(Input.mousePosition).x * 100.0f
    );
    Random.InitState(initX);
    // これより端にゴールを設定しない
    int padding = 15;
    goal = new Vector2(
      (int)(Random.Range(padding, Screen.width - padding)),
      (int)(Random.Range(padding, Screen.height - padding))
    );
    Debug.Log("goal : " + goal.x + ", " + goal.y + " : screen : " + Screen.width + ", " + Screen.height);

    hint = GameObject.FindWithTag("Hint");
    int hintX = (int)(Random.Range(goal.x - hintRadius, goal.x + hintRadius));
    int hintY = (int)(Random.Range(goal.y - hintRadius, goal.y + hintRadius));
    hint.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(hintX, hintY, 10));
    // 最初はヒント消しておく
    hint.SetActive(false);

    masawadas = GameObject.FindGameObjectsWithTag("masawada");

    phase = new float[numberN];
    phaseDiffY = new float[numberN];
    for (int i = 0; i < numberN; i++) {
      int number = i + 1;
      // 位相のずれは毎回かつ人によって違うものにするために,
      // マウス位置と連番の和を乱数シードにする
      Random.InitState(initX + number);
      phase[i] = Random.Range(0, dpi);
      // Yで補正すべき値も計算しておいて使う
      phaseDiffY[i] = Mathf.Sin(phase[i]);
    }
  }

  // Update is called once per frame
  void Update()
  {
    // パレード開催中ならパレードだけやる
    if (isParade == 1) {
      parade();
      return;
    }

    // 始めて90秒経ったらヒント出す
    if ((int)(Time.time - startTime) > 90) {
      hint.SetActive(true);
    }

    Vector3 pos = Input.mousePosition;
    pos.x = (int)pos.x;
    pos.y = (int)pos.y;

    int shouldParade = 1;
    for (int i = 0; i < numberN; i++) {
      int newY = getY(pos, i);
      masawadas[i].transform.rotation = Quaternion.Euler(0, newY, 0);
      if (newY != 180) {
        // パレードすべきなのは全員が前を向いているときだけ
        shouldParade = 0;
      }
      Debug.Log(i + ": goal: " + goal.x + ", " + goal.y + " :pos: " + pos.x + ", " + pos.y + " :y: " + newY);
    }

    if (shouldParade == 1) {
      float time = Time.time;
      if (goalTime == 0) {
        goalTime = time;
        // ゴールしたらその旨即座に示す
        makeAllMasawadaGreatAgain();
      } else {
        // 0.5秒間ゴールし続けられたらパレード開催
        if (time - goalTime > 0.5f) {
          isParade = 1;
        }
      }
    } else {
      goalTime = 0.0f;
      makeAllMasawadaNormal();
    }
  }

  int getY(Vector3 pos, int i) {
    // 連番
    int number = i + 1;
    // ゴールで前を向くために,
    // ゴールにたどり着いたら0になるように引き算をする
    // 連番の大小の効きはX,Yで逆にする
    float ingrediantX =
      ((float)number / weakenNumber) * (pos.x - goal.x)
      / (Screen.width * weakenPeriod);
    float ingrediantY =
      ((float)((numberN + 1) - number) / weakenNumber) * (pos.y - goal.y)
      / (Screen.height * weakenPeriod);

    float xOfSin =
      (ingrediantX + ingrediantY) * dpi + phase[i];
    // 連番次第で回転数を上げる(6倍は効きすぎるので弱める)
    // 最低でも1回転はするために,最後に1を足す(気持ち程度に弱める前に1引く)
    float multiplyAngleAtPeriod = ((float)number - 1.0f) / weakenNumber + 1.0f;
    // 周期をずらした分だけYも0からずれるので,0補正する
    // 回転角の解像度を荒くするために一旦割ってintにしてからかけ戻す
    return (int)(
      (
        (Mathf.Sin(xOfSin) - phaseDiffY[i])
        * angleAtPeriod * multiplyAngleAtPeriod
      ) / resolution
    ) * ((int)resolution) + 180;  // ゴール(sin==0)のとき前(180)
  }

  void parade() {
    float diff = Time.time - goalTime;
    int paradeTime = (int)diff;

    if (paradeTime >= 5) {
      // 5秒パレードしたらタイトルに戻す
      SceneManager.LoadScene("title");
    } else {
      for (int i = 0; i < numberN; i++) {
        if ((int)((diff - paradeTime) * 10.0f) > i) {
          if ((paradeTime % 2) == 0) {
            makeMasawadaNormal(i);
          } else {
            makeMasawadaGreatAgain(i);
          }
        }
      }
    }
  }

  void setRotation(int i, string tag, int x, int y, int z) {
      // GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
      // foreach (GameObject obj in objs) {
      //   obj.transform.localRotation = Quaternion.Euler(x, y, z);
      // }
      GameObject obj = GameObject.FindGameObjectsWithTag(tag)[i];
      obj.transform.localRotation = Quaternion.Euler(x, y, z);
  }

  void makeAllMasawadaGreatAgain() {
    for (int i = 0; i < numberN; i++) {
      makeMasawadaGreatAgain(i);
    }
  }

  void makeAllMasawadaNormal() {
    for (int i = 0; i < numberN; i++) {
      makeMasawadaNormal(i);
    }
  }

  void makeMasawadaGreatAgain(int i) {
    Debug.Log("makeMasawadaGreatAgain");

    // 左腕
    setRotation(i, "LA", 0, 0, 77);
    setRotation(i, "LE", -7, 6, 10);
    setRotation(i, "LW", 0, 0, 0);
    // 左手指
    setRotation(i, "LI1", 0, 0, 5);
    setRotation(i, "LI2", 0, 0, 20);
    setRotation(i, "LI3", 0, 0, 15);
    setRotation(i, "LL1", 0, 0, 0);
    setRotation(i, "LL2", 0, 0, 40);
    setRotation(i, "LL3", 0, 0, 20);
    setRotation(i, "LM1", 0, 0, 2);
    setRotation(i, "LM2", 0, 0, 40);
    setRotation(i, "LM3", 0, -10, 20);
    setRotation(i, "LR1", 0, 0, 3);
    setRotation(i, "LR2", 0, 0, 30);
    setRotation(i, "LR3", 0, 0, 30);
    setRotation(i, "LT1", 30, -40, -10);
    setRotation(i, "LT2", 0, 0, 0);
    setRotation(i, "LT3", 0, 0, 0);

    // 右腕
    setRotation(i, "RA", 0, 0, -70);
    setRotation(i, "RE", 0, -100, -20);
    setRotation(i, "RW", -15, 5, 0);
    // 右手指
    setRotation(i, "RI1", 25, -5, -30);
    setRotation(i, "RI2", 0, 0, -100);
    setRotation(i, "RI3", 0, 0, -60);
    setRotation(i, "RL1", 0, 0, -50);
    setRotation(i, "RL2", 0, 0, -120);
    setRotation(i, "RL3", 0, 0, -60);
    setRotation(i, "RM1", -5, -14, -85);
    setRotation(i, "RM2", 0, 0, -100);
    setRotation(i, "RM3", 0, 0, -60);
    setRotation(i, "RR1", -17, -5, -60);
    setRotation(i, "RR2", 0, 0, -100);
    setRotation(i, "RR3", 0, 0, -60);
    setRotation(i, "RT1", 0, 0, 0);
    setRotation(i, "RT2", 0, 0, 0);
    setRotation(i, "RT3", 0, 0, 0);
  }

  void makeMasawadaNormal(int i) {
    Debug.Log("makeMasawadaNormal");

    // 左腕
    setRotation(i, "LA", 0, 0, 81);
    setRotation(i, "LE", 0, 0, 0);
    setRotation(i, "LW", 0, 0, 0);
    // 左手指
    setRotation(i, "LI1", 0, 0, 0);
    setRotation(i, "LI2", 0, 0, 0);
    setRotation(i, "LI3", 0, 0, 0);
    setRotation(i, "LL1", 0, 0, 0);
    setRotation(i, "LL2", 0, 0, 0);
    setRotation(i, "LL3", 0, 0, 0);
    setRotation(i, "LM1", 0, 0, 0);
    setRotation(i, "LM2", 0, 0, 0);
    setRotation(i, "LM3", 0, 0, 0);
    setRotation(i, "LR1", 0, 0, 0);
    setRotation(i, "LR2", 0, 0, 0);
    setRotation(i, "LR3", 0, 0, 0);
    setRotation(i, "LT1", 0, 0, 0);
    setRotation(i, "LT2", 0, 0, 0);
    setRotation(i, "LT3", 0, 0, 0);

    // 右腕
    setRotation(i, "RA", 0, 0, -80);
    setRotation(i, "RE", 0, 0, 0);
    setRotation(i, "RW", 0, 0, 0);
    // 右手指
    setRotation(i, "RI1", 0, 0, 0);
    setRotation(i, "RI2", 0, 0, 0);
    setRotation(i, "RI3", 0, 0, 0);
    setRotation(i, "RL1", 0, 0, 0);
    setRotation(i, "RL2", 0, 0, 0);
    setRotation(i, "RL3", 0, 0, 0);
    setRotation(i, "RM1", 0, 0, 0);
    setRotation(i, "RM2", 0, 0, 0);
    setRotation(i, "RM3", 0, 0, 0);
    setRotation(i, "RR1", 0, 0, 0);
    setRotation(i, "RR2", 0, 0, 0);
    setRotation(i, "RR3", 0, 0, 0);
    setRotation(i, "RT1", 0, 0, 0);
    setRotation(i, "RT2", 0, 0, 0);
    setRotation(i, "RT3", 0, 0, 0);
  }
}
