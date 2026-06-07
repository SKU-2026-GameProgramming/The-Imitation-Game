using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CipherManager : MonoBehaviour
{
    public TextMeshProUGUI[] cipherTexts = new TextMeshProUGUI[6];
    public TextMeshProUGUI[] powerTexts = new TextMeshProUGUI[6];
    public TextMeshProUGUI[] keyTexts = new TextMeshProUGUI[6];
    public Province[] provinces;
    int day = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        provinces = GameObject.Find("Provinces").GetComponent<MapManager>().provinces;
    }

    public void Encrypt(GeminiResponse aiResult)
    {
        day++;
        Debug.Log("day" + day);

        if (aiResult == null)
        {
            Debug.LogWarning("[Encrypt] 전달받은 AI 결과 데이터가 Null입니다.");
        }

        // 2. 임시 믹싱 탱크(List) 생성
        List<ActionData> mixingTank = new List<ActionData>();

        // 3. 공격(attacks) 리스트에서 최대 3개 선별해서 탱크에 주입
        if (aiResult.attacks != null)
        {
            int attackCountToTake = Mathf.Min(3, aiResult.attacks.Count);
            for (int i = 0; i < attackCountToTake; i++)
            {
                mixingTank.Add(aiResult.attacks[i]);
            }
        }

        // 4. 방어(defends) 리스트에서 최대 3개 선별해서 탱크에 주입
        if (aiResult.defends != null)
        {
            int defendCountToTake = Mathf.Min(3, aiResult.defends.Count);
            for (int i = 0; i < defendCountToTake; i++)
            {
                mixingTank.Add(aiResult.defends[i]);
            }
        }

        // 5. [핵심] 셔플 알고리즘 (Fisher-Yates Shuffle)
        // 리스트 내부 요소들을 컴퓨터 공학 표준 방식으로 완벽하게 무작위로 뒤섞습니다.
        System.Random prng = new System.Random();
        for (int i = mixingTank.Count - 1; i > 0; i--)
        {
            int randomIndex = prng.Next(0, i + 1);

            // 값 스와프(Swap) 처리
            ActionData temp = mixingTank[i];
            mixingTank[i] = mixingTank[randomIndex];
            mixingTank[randomIndex] = temp;
        }

        // 6. 키 랜덤 지정 (0을 제외하고 1~25 범위로 안전하게 지정)
        int[] key = new int[mixingTank.Count];
        if (day >= 2)
        {
            for (int i = 0; i < mixingTank.Count; i++)
            {
                // 💡 [수정] 1부터 25까지 정상적으로 다 나오도록 범위를 변경합니다.
                key[i] = Random.Range(1, 26);

                // ⚠️ 여기서 주의: keyTexts 배열도 크기가 6인 고정 배열이라면 
                // mixingTank.Count가 6보다 클 때 에러가 날 수 있으니 안전장치를 걸어줍니다.
                if (i < keyTexts.Length)
                {
                    keyTexts[i].text = key[i].ToString();
                }
            }
        }

        // 7. 최종 '배열' 형태로 변환하여 반환
        ActionData[] encryptedArray = mixingTank.ToArray();
        int activeLimit = Mathf.Min(6, encryptedArray.Length);

        for (int i = 0; i < 6; i++)
        {
            if (i < activeLimit)
            {
                ActionData currentTarget = encryptedArray[i];
                string originalNodeName = provinces[currentTarget.nodeID].nodeName.ToUpper();

                cipherTexts[i].gameObject.SetActive(true);
                powerTexts[i].gameObject.SetActive(true);

                // 💡 [체크] 만약 Day 2 이상인데 keyTexts가 남는 칸이 있다면 켜주기
                if (day >= 2 && i < keyTexts.Length) keyTexts[i].gameObject.SetActive(true);

                switch (day)
                {
                    case 1:
                        cipherTexts[i].text = GetRandomFourLetters(originalNodeName);
                        break;
                    case 2:
                        // 정방향 플러스 시프트
                        string shiftedFull = ShiftCharacters(originalNodeName, key[i]);
                        cipherTexts[i].text = shiftedFull.Substring(0, 4);
                        Debug.Log($"[{i}번째 칸 세팅] 지역: {originalNodeName} | 정답 Key: {key[i]} | 출력된 암호문: {cipherTexts[i].text}");
                        break;
                    default:
                        // Day 3+: 정방향 플러스 시프트 후 셔플
                        shiftedFull = ShiftCharacters(originalNodeName, key[i]);
                        cipherTexts[i].text = GetRandomFourLetters(shiftedFull);
                        break;
                }

                if (day >= 4)
                    powerTexts[i].text = (currentTarget.power / 100 + 1).ToString();
                else
                    powerTexts[i].text = currentTarget.power.ToString();
            }
            else
            {
                cipherTexts[i].gameObject.SetActive(false);
                powerTexts[i].gameObject.SetActive(false);
                // 데이터가 없는 칸은 키 텍스트도 같이 꺼줍니다.
                if (i < keyTexts.Length) keyTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public string GetRandomFourLetters(string provinceName)
    {
        // 1. 공백 제거 및 방어 코드
        // 앞뒤 공백을 자르고, 만약 공백을 지웠는데 빈 문자열이면 기본값 반환
        string cleanName = provinceName.Trim();
        if (string.IsNullOrEmpty(cleanName))
        {
            return "ABCD"; // 예외 케이스용 더미 데이터
        }

        // 2. 글자 수가 4글자 이하인 경우 처리
        // 원본 글자 수가 이미 4글자 이하라면 굳이 뽑을 필요 없이 
        // 순서만 가볍게 섞거나 그대로 반환하여 글자 수가 모자라지 않게 보호합니다.
        if (cleanName.Length <= 4)
        {
            return ShuffleString(cleanName);
        }

        // 3. 중복 없는 무작위 인덱스(위치) 선별
        // 글자 수가 4글자보다 많을 때, 글자가 위치한 방 번호(인덱스) 리스트를 만듭니다.
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < cleanName.Length; i++)
        {
            availableIndices.Add(i);
        }

        // Fisher-Yates 방식으로 인덱스 자체를 무작위로 섞어버립니다.
        System.Random random = new System.Random();
        for (int i = availableIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            int temp = availableIndices[i];
            availableIndices[i] = availableIndices[randomIndex];
            availableIndices[randomIndex] = temp;
        }

        // 4. 앞의 4개 인덱스에 해당하는 글자들을 조립
        // 텍스트 조립 시 성능 저하를 막기 위해 StringBuilder를 사용합니다.
        StringBuilder resultBuilder = new StringBuilder();
        for (int i = 0; i < 4; i++)
        {
            int targetIndex = availableIndices[i];
            resultBuilder.Append(cleanName[targetIndex]);
        }

        return resultBuilder.ToString();
    }

    private string ShiftCharacters(string input, int shift)
    {
        StringBuilder sb = new StringBuilder();

        foreach (char c in input)
        {
            // 대문자 암호화 처리
            if (c >= 'A' && c <= 'Z')
            {
                // 알파벳 26개 범위 안에서 뺑뺑이 돌기 (오버플로우 방지)
                char shifted = (char)((((c - 'A') + shift) % 26) + 'A');
                sb.Append(shifted);
            }
            // 소문자 암호화 처리
            else if (c >= 'a' && c <= 'z')
            {
                char shifted = (char)((((c - 'a') + shift) % 26) + 'a');
                sb.Append(shifted);
            }
            // 알파벳이 아닌 문자(한글, 공백 등)는 안전하게 그대로 유지
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 문자열 전체의 순서를 무작위로 섞어주는 보조 함수 (4글자 이하 방어용)
    /// </summary>
    private string ShuffleString(string target)
    {
        char[] chars = target.ToCharArray();
        System.Random random = new System.Random();

        for (int i = chars.Length - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            char temp = chars[i];
            chars[i] = chars[randomIndex];
            chars[randomIndex] = temp;
        }

        return new string(chars);
    }
}
