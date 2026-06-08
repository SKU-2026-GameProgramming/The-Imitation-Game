using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // ★ 씬 전환(화면 이동)을 위해 꼭 필요한 유니티 기능

public class NewIntroController : MonoBehaviour
{
    public TextMeshProUGUI introText; // 유니티 인스펙터창에 연결할 글자창
    public AudioSource audioSource;   // 소리를 재생해 줄 스피커
    public AudioClip typewriterSound; // 타자기 탁! 탁! 소리 파일
    public float typeSpeed = 0.05f;   // 글자 찍히는 속도

    // [꼼수 방어막] 다음으로 넘어갈 씬 이름을 인스펙터에서 직접 타이핑할 수 있게 수정!
    [Header("스토리가 끝난 후 로드할 씬 이름")]
    public string nextSceneName = "MainScene";

    private List<string> dialogueLines = new List<string>();
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private string currentFullText = "";

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // 화면에 뿌려줄 벙커 스토리 대사 세팅
        dialogueLines.Add("이건 아무도 모르는 한 영웅의 이야기.");
        dialogueLines.Add("19XX년, 파리 외곽의 어딘가.\n이미 전선의 절반이 독일군의 군화발에 짓밟혔다.");
        dialogueLines.Add("우리가 배수진을 친 곳은 이곳, 컴컴한 지하 벙커뿐.\n놈들이 다음엔 어디를 들이칠지, 저 암호문 속에 정답이 있다.");
        dialogueLines.Add("한정된 병력, 한 번의 실수.\n너의 선택이 병사의 목숨을 좌우한다는것을 명심해라.");

        // 🚨 변경된 부분: 바로 시작하지 않고, 3초 대기 연출 코루틴을 실행합니다!
        if (introText != null && dialogueLines.Count > 0)
        {
            // 처음에 글자창을 깨끗하게 비워두고 시작
            introText.text = "";
            StartCoroutine(StartIntroWithDelay(3.0f));
        }
    }

    // 🎬 연출용 코루틴: 지정된 시간(초)만큼 기다린 후 첫 대사를 띄웁니다.
    IEnumerator StartIntroWithDelay(float delaySeconds)
    {
        // 3초 동안 벙커의 정막함을 유지... (OS의 Sleep 연산과 같습니다!)
        yield return new WaitForSeconds(delaySeconds);

        // 3초가 지나면 첫 번째 대사 타이핑 시작!
        StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 🚨 [꼼수 방어] 아직 3초가 안 지나서 첫 대사도 안 나왔을 때 클릭하면 무시하기!
            if (introText.text == "" && !isTyping) return;

            if (isTyping)
            {
                StopAllCoroutines();
                introText.text = currentFullText;
                isTyping = false;
            }
            else
            {
                currentLineIndex++;
                if (currentLineIndex < dialogueLines.Count)
                {
                    StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
                }
                else
                {
                    SceneManager.LoadScene(nextSceneName);
                }
            }
        }
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        currentFullText = line;
        introText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            introText.text += letter;

            if (letter != ' ' && audioSource != null && typewriterSound != null)
            {
                audioSource.PlayOneShot(typewriterSound);
            }

            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }
}