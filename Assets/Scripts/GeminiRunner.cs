using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json; // Newtonsoft 추가

// --- JSON 파싱을 위한 클래스 구조 정의 ---
[Serializable]
public class ActionData
{
    public int nodeID;
    public int power;
}

[Serializable]
public class GeminiResponse
{
    public List<ActionData> attacks = new List<ActionData>();
    public List<ActionData> defends = new List<ActionData>();
}
// ---------------------------------------

[RequireComponent(typeof(GeminiApiClient))]
public sealed class GeminiRunner : MonoBehaviour
{
    [Header("Prompt")]
    [TextArea(3, 8)]
    [SerializeField] public string prompt;

    [Header("Controls")]
    [SerializeField] private bool runOnStart;
    [SerializeField] private KeyCode triggerKey;

    [Header("UI Controls")]
    // ★ 유니티 캔버스(Canvas) 내부의 화면을 가릴 패널 오브젝트를 여기에 드래그 앤 드롭 하세요.
    [Tooltip("AI 연산 중 플레이어 조작을 막을 UI 패널")]
    [SerializeField] private GameObject blockingPanel;

    public GeminiApiClient _geminiApiClient;
    private bool _isRequesting;

    // ★ 다른 스크립트(예: TurnManager)가 구독할 이벤트
    // 파싱이 완료된 GeminiResponse 객체를 통째로 넘겨줍니다.
    public event Action<GeminiResponse> OnResponseParsed;

    private void Start()
    {
        if (runOnStart) RequestGemini();
    }

    private void Update()
    {
        
    }

    [ContextMenu("Request Gemini")]
    public void RequestGemini()
    {
        if (_isRequesting) return;

        _isRequesting = true;

        // 1. 구글 서버에 요청하기 직전에 화면 가리기 패널 활성화!
        if (blockingPanel != null)
        {
            blockingPanel.SetActive(true);
        }

        _geminiApiClient.GenerateContent(prompt, HandleSuccess, HandleError);
    }

    private void HandleSuccess(string responseText)
    {
        if (blockingPanel != null)
        {
            blockingPanel.SetActive(false);
        }

        _isRequesting = false;

        // 파일 저장은 백업용으로 유지
        File.WriteAllText(Path.Combine(Application.dataPath, "Gemini_response.txt"), responseText, System.Text.Encoding.UTF8);

        //try
        //{
            // 1. Newtonsoft.Json으로 유니티 객체화
            GeminiResponse parsedData = JsonConvert.DeserializeObject<GeminiResponse>(responseText);

            if (parsedData != null)
            {
                Debug.Log($"[GeminiRunner] 파싱 성공! 공격 노드 개수: {parsedData.attacks.Count}");

                // 2. 이 이벤트를 기다리고 있는 다른 게임 스크립트들에게 데이터 토스!
                OnResponseParsed?.Invoke(parsedData);
            }
            else
            {
                Debug.Log(parsedData);
            }
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError($"[GeminiRunner] JSON 파싱 실패: {ex.Message}\n원본: {responseText}");
        //}
    }

    private void HandleError(string error)
    {
        if (blockingPanel != null)
        {
            blockingPanel.SetActive(false);
        }

        _isRequesting = false;
        Debug.LogError($"Gemini 요청 실패: {error}", this);
    }
}