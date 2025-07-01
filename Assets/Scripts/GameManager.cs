using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [Header("Grid Variables")]
    [SerializeField] private int length = 4;
    [SerializeField] private int width = 4;
    [SerializeField] private int height = 4;
    public static MasterGrid masterGrid;

    [Header("Game Variables")]
    [SerializeField] private float timer = 30f;
    private float counter;
    private string currentColor;
    [SerializeField] private List<string> colorPool;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text currentColorText;

    private int _score;
    private bool win;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        masterGrid = new MasterGrid(length, width, height);
    }
    private void Start() {
        PickNewColor();
        counter = timer;
        _score = 0;
        win = false;
    }

    private void Update() {
        if (counter > 0) counter -= Time.deltaTime;
        else {
            PickNewColor();
            counter = timer;
        }
        timerText.text = "Time: " + Mathf.CeilToInt(counter);
        CheckScore();

        if (win) Debug.Log("WON");
    }

    public string GetCurrentColor() {
        return currentColor;
    }

    public void SetScore(int score) {
        if (win) return;
        _score += score;
    }

    public bool CheckWinChance() {
        return Random.Range(1, 101) == 1;
    }

    private void CheckScore() {
        if (_score >= 10) {
            win = true;
        }
    }

    private void PickNewColor() {
        currentColor = colorPool[Random.Range(0, colorPool.Count)];
        currentColorText.text = "Current: " + currentColor;
        _score = 0;
    }
}