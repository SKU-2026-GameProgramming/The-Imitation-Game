using System.Text;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ShifterButtonManager : MonoBehaviour
{
    RectTransform shifter;
    AudioSource aud;
    TMP_InputField key;
    public TextMeshProUGUI[] cipherChars = new TextMeshProUGUI[4];
    MainSceneDirector director;

    public AudioClip clickSF;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shifter = GameObject.Find("ShifterUI").GetComponent<RectTransform>();
        key = GameObject.Find("ShiftField").GetComponent<TMP_InputField>();
        director = GameObject.Find("GameDirector").GetComponent<MainSceneDirector>();
        aud = GetComponent<AudioSource>(); 
    }

    public void OnQuitButtonClicked()
    {
        aud.PlayOneShot(clickSF);
        shifter.anchoredPosition = Vector3.left * 2000;
        director.isShifterOpened = false;
    }

    public void OnShiftButtonClick()
    {
        aud.PlayOneShot(clickSF);

        // 1. 현재 UI에 입력된 키값 파싱
        if (!int.TryParse(key.text, out int k)) return; // 방어 코드: 혹시 숫자가 아니면 리턴

        // 2. 4개의 다이얼 칸을 하나씩 돌리기
        for (int i = 0; i < 4; i++)
        {
            // 텍스트가 비어있지 않은지 체크
            if (string.IsNullOrEmpty(cipherChars[i].text)) continue;

            // 원본 글자 가져오기 (대문자 기준)
            char originalChar = cipherChars[i].text[0];

            // 3. 알파벳(A~Z)인 경우에만 시프트 연산 처리
            if (originalChar >= 'A' && originalChar <= 'Z')
            {
                // 대문자 'A'를 기준으로 k만큼 밀고, 26을 넘어갈 때를 대비해 나머지 연산(%) 처리
                char shiftedChar = (char)((((originalChar - 'A') - k + 26) % 26) + 'A');

                // 4. 일반 string 형태로 다시 텍스트 컴포넌트에 주입
                cipherChars[i].text = shiftedChar.ToString();
            }
            // 소문자도 혹시 들어올 수 있다면 안전장치 추가
            else if (originalChar >= 'a' && originalChar <= 'z')
            {
                char shiftedChar = (char)((((originalChar - 'a') + k) % 26) + 'a');
                cipherChars[i].text = shiftedChar.ToString();
            }
        }
    }
}
