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

  float goalTime;
  int isParade;
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

    masawadas = new GameObject[numberN];
    phase = new float[numberN];
    phaseDiffY = new float[numberN];
    for (int i = 0; i < numberN; i++) {
      int number = i + 1;
      masawadas[i] = GameObject.FindWithTag("wada" + number);
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
        makeMasawadaGreatAgain();
      } else {
        // 0.5秒間ゴールし続けられたらパレード開催
        if (time - goalTime > 0.5f) {
          isParade = 1;
        }
      }
    } else {
      goalTime = 0.0f;
      makeMasawadaNormal();
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
    int paradeTime = (int)(Time.time - goalTime);

    if (paradeTime >= 5) {
      // 5秒パレードしたらタイトルに戻す
      SceneManager.LoadScene("title");
    } else if ((paradeTime % 2) == 0) {
      makeMasawadaNormal();
    } else {
      makeMasawadaGreatAgain();
    }
  }

  void makeMasawadaGreatAgain() {
    Debug.Log("makeMasawadaGreatAgain");
  }
  void makeMasawadaNormal() {
    Debug.Log("makeMasawadaNormal");
  }
}
