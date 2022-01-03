using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public int number = 1;
    public int score = 100;
    
    [Header("Reference")]
    public TextAsset Questions;
    public TextAsset Options;
    public TextAsset Answers;

    public TMP_Text numT;
    public TMP_Text scoreT;
    public TMP_Text questionT;
    public TMP_Text answerT;
    
    public Image rightI;
    public Image errorI;

    public SpriteRenderer answer1;
    public SpriteRenderer answer2;
    public SpriteRenderer answer3;
    public SpriteRenderer answer4;

    public AudioSource rightA;
    public AudioSource errorA;

    [Header("问题")]
    public string [] questions;
    [Header("选项答案")]
    public string [] options;
    [Header("答案")]
    public string [] answers;
    [Header("题目数量")]
    public int maxNum = 10;
    [Header("题目分数")]
    public int shootRightScore = 10;
    [Header("答案贴图存储")]
    public Sprite [] AFish;
    public Sprite [] BFish;
    public Sprite [] CFish;
    public Sprite [] DFish;
    [Header("鱼移动终点")]
    public Vector3 moveEndPos;
    [Header("是否处于回答结束状态")]
    public bool isAnswerDone = false;
    
    void Awake()
    {
        instance = this;

        questions = Questions.text.Split('\n');
        options = Options.text.Split(' ');
        answers = Answers.text.Split('\n');
        maxNum = questions.Length;

        AFish = new Sprite[maxNum];
        BFish = new Sprite[maxNum];
        CFish = new Sprite[maxNum];
        DFish = new Sprite[maxNum];

        for (int i = 0; i < maxNum; i++)
        {
            AFish[i] = Resources.Load<Sprite>("Fishs/AFish" + (i+1).ToString());
            BFish[i] = Resources.Load<Sprite>("Fishs/BFish" + (i+1).ToString());
            CFish[i] = Resources.Load<Sprite>("Fishs/CFish" + (i+1).ToString());
            DFish[i] = Resources.Load<Sprite>("Fishs/DFish" + (i+1).ToString());
        }
    }

    void Start()
    {
        Color tColor = rightI.color;
        tColor.a = 0;
        rightI.color = tColor;
        errorI.color = tColor;
        
        questionT.text = questions[0];
        answerT.text = answers[0];
        answer1.sprite = AFish[0];
        answer2.sprite = BFish[0];
        answer3.sprite = CFish[0];
        answer4.sprite = DFish[0];
    }

    void Update()
    {
        numT.text = "number: " + number.ToString();
        scoreT.text = "score: " + score.ToString();    
    }

    public void ShootRight(int index)
    {
        if (isAnswerDone)
        {
            return;
        }
        GameController.instance.isAnswerDone = true;
        switch (index)
        {
            case 1:
                StartCoroutine(MoveFish(answer1.gameObject));
                break;
            case 2:
                StartCoroutine(MoveFish(answer2.gameObject));
                break;
            case 3:
                StartCoroutine(MoveFish(answer3.gameObject));
                break;
            case 4:
                StartCoroutine(MoveFish(answer4.gameObject));
                break;
        }

        StartCoroutine(FadeIn(rightI));

        StartCoroutine(ShowAnswer());

        //播放音效
        rightA.Play();

        score += shootRightScore;
        scoreT.text = "score: " + score.ToString();
        StaticData.correct++;

        StartCoroutine(NextQuestion(number));
        StartCoroutine(NextFishs(number));

        if (number < maxNum)
        {
            StartCoroutine(AddNumber());
        }
    }

    public void ShootError()
    {
        if (isAnswerDone)
        {
            return;
        }
        GameController.instance.isAnswerDone = true;
        StartCoroutine(FadeIn(errorI));

        StartCoroutine(ShowAnswer());

        //播放音效
        errorA.Play();

        scoreT.text = "score: " + score.ToString();
        StaticData.error++;

        StartCoroutine(NextQuestion(number));
        StartCoroutine(NextFishs(number));

        if (number < maxNum)
        {
            StartCoroutine(AddNumber());
        }
    }

    public IEnumerator NextQuestion(int num)
    {
        if (num >= maxNum)
        {
            FadeIntoNextScene.instance.LoadNextScene();
        }
        yield return new WaitForSeconds(1f);

        questionT.text = questions[num];
    }

    public IEnumerator NextFishs(int num)
    {
        if (num >= maxNum)
        {
            FadeIntoNextScene.instance.LoadNextScene();
        }
        yield return new WaitForSeconds(1f);

        answer1.gameObject.SetActive(true);
        answer2.gameObject.SetActive(true);
        answer3.gameObject.SetActive(true);
        answer4.gameObject.SetActive(true);

        Color tColor = answer1.color;
        tColor.a = 1;
        answer1.sprite = AFish[num];
        answer1.color = tColor;
        answer2.sprite = BFish[num];
        answer2.color = tColor;
        answer3.sprite = CFish[num];
        answer3.color = tColor;
        answer4.sprite = DFish[num];
        answer4.color = tColor;
    }

    public IEnumerator FadeIn(Image i)
    {
        Color tColor = i.color;
        float fadeOutDuration = 0.8f;
        float fadeOutDone = Time.time + fadeOutDuration;
        while (Time.time < fadeOutDone)
        {
            tColor.a = 1 - (fadeOutDone - Time.time) / fadeOutDone;
            i.color = tColor;
            yield return null;
        }
        tColor.a = 1;
        i.color = tColor;

        StartCoroutine(FadeOut(i));
    }

    public IEnumerator FadeOut(Image i)
    {
        Color tColor = i.color;
        float fadeOutDuration = 0.2f;
        float fadeOutDone = Time.time + fadeOutDuration;
        while (Time.time < fadeOutDone)
        {
            tColor.a = (fadeOutDone - Time.time) / fadeOutDone;
            i.color = tColor;
            yield return null;
        }
        tColor.a = 0;
        i.color = tColor;
    }

    public IEnumerator ShowAnswer()
    {
        answerT.text = answers[number-1];
        float fadeDuration = 0.7f;
        float fadeDone = Time.time + fadeDuration;
        Color tColor = answerT.color;

        while (Time.time < fadeDone)
        {
            tColor.a = 1 - (fadeDone - Time.time) / fadeDuration;
            answerT.color = tColor;

            yield return null;
        }

        tColor.a = 1;
        answerT.color = tColor;

        StartCoroutine(LaterFadeAnswer());
    }

    public IEnumerator LaterFadeAnswer()
    {
        yield return new WaitForSeconds(0.3f);

        Color tColor = answerT.color;
        tColor.a = 0;
        answerT.color = tColor;
    }

    public IEnumerator MoveFish(GameObject fish)
    {
        float moveDuration = 0.4f;
        float moveDone = Time.time + moveDuration;
        Vector3 tFishPos = fish.transform.position;
        Vector3 dirMoveDistance = moveEndPos - fish.transform.position;
        Vector3 tPos = fish.transform.position;
        float u = (moveDone - Time.time) / moveDuration;
        while (u >= 0)
        {
            tPos = tFishPos + (1-u) * dirMoveDistance;
            fish.transform.position = tPos;

            u = (moveDone - Time.time) / moveDuration;
            yield return null;
        }

        float fadeDuration = 0.5f;
        float fadeDone = Time.time + fadeDuration;
        SpriteRenderer fishRender = fish.GetComponent<SpriteRenderer>();
        Color tColor = fishRender.color;
        while (Time.time < fadeDone)
        {
            tColor.a = (fadeDone - Time.time) / fadeDuration;
            fishRender.color = tColor;

            yield return null;
        }

        tColor.a = 0;
        fishRender.color = tColor;

        fish.transform.position = tFishPos;
        fish.SetActive(false);
    }

    public IEnumerator AddNumber()
    {
        yield return new WaitForSeconds(1f);

        number++;
        isAnswerDone = false;
    }
}