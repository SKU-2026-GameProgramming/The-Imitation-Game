using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 시작화면으로 이동하기 위한 유니티 필수 기능

public class VictorySceneController : MonoBehaviour
{
    public TextMeshProUGUI endingText; // 대사가 찍힐 글자창 (Text-TMP)
    public AudioSource audioSource;   // 소리를 내줄 스피커
    public AudioClip typewriterSound; // 타자기 탁! 탁! 소리 파일
    public float typeSpeed = 0.05f;   // 글자 속도

    private List<string> dialogueLines = new List<string>();
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private string currentFullText = "";

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // 코드 내부에 승리 대사 완벽하게 고정!
        dialogueLines.Add("당신은 독일군으로부터 프랑스를 지켜내는데 성공하였습니다.");
        dialogueLines.Add("비록 세상은 당신들의 활약을 영원히 기억하지 못할지라도");
        dialogueLines.Add("이 어두운 벙커에서 피어난 위대한 희생을, \n우리는 영원히 가슴속에 기릴 것입니다.");
        dialogueLines.Add("- 암호 해독 작전 대성공 -");

        // 첫 대사 시작
        if (endingText != null && dialogueLines.Count > 0)
        {
            StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
        }
    }

    void Update()
    {
        // 마우스 왼쪽 클릭 시 작동
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // 글자 나오는 중이면 한방에 보여주기
                StopAllCoroutines();
                endingText.text = currentFullText;
                isTyping = false;
            }
            else
            {
                // 다 나왔으면 다음 문장으로 진행
                currentLineIndex++;
                if (currentLineIndex < dialogueLines.Count)
                {
                    StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
                }
                else
                {
                    // ★ 승리 대사가 완전히 끝나고 클릭하면 메인 시작화면(StartScene)으로 이동!
                    SceneManager.LoadScene("StartScene");
                }
            }
        }
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        currentFullText = line;
        endingText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            endingText.text += letter;

            // 공백이 아닐 때만 타자기 소리 재생
            if (letter != ' ' && audioSource != null && typewriterSound != null)
            {
                audioSource.PlayOneShot(typewriterSound);
            }
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }
}