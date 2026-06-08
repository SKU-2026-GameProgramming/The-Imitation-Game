using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 시작화면으로 화면을 전환하기 위한 필수 기능

public class DefeatSceneController : MonoBehaviour
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

        // 코드 내부에 패배 대사 완벽하게 고정!
        dialogueLines.Add("당신은 병사들과 함께 끝까지 항전하였으나,\n끝내 독일군을 막아내지 못했습니다.");
        dialogueLines.Add("철문 너머로 들이닥치는 적들의 군화 소리...\n기회는 영원히 사라졌습니다.");
        dialogueLines.Add("다시 시작하시겠습니까?");

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
                // 글자 나오는 중이면 한방에 전부 보여주기 (스킵)
                StopAllCoroutines();
                endingText.text = currentFullText;
                isTyping = false;
            }
            else
            {
                // 글자가 다 나왔으면 다음 문장으로 진행
                currentLineIndex++;
                if (currentLineIndex < dialogueLines.Count)
                {
                    StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
                }
                else
                {
                    // ★ 패배 대사가 모두 끝난 상태(다시 시작하시겠습니까?)에서 누르면 시작화면(StartScene)으로 이동!
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

            // 공백(띄어쓰기)이 아닐 때만 찰지게 타자기 소리 내기
            if (letter != ' ' && audioSource != null && typewriterSound != null)
            {
                audioSource.PlayOneShot(typewriterSound);
            }
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }
}