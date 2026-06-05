using UnityEngine;

[RequireComponent(typeof(GeminiApiClient))]
// 플레이 모드에서 GeminiApiClient를 간단히 호출해 보는 데모 컴포넌트입니다.
public sealed class GeminiDemoRunner : MonoBehaviour
{
    [Header("Demo Prompt")]
    [TextArea(3, 8)]
    // Inspector에서 바꿀 수 있는 테스트용 프롬프트입니다.
    [SerializeField] private string prompt = "Write one short NPC line in Korean for a Unity demo project.";

    [Header("Controls")]
    [SerializeField] private bool runOnStart;
    // 기본값은 G 키입니다. KeyCode.None으로 두면 키 입력 실행을 끌 수 있습니다.
    [SerializeField] private KeyCode triggerKey = KeyCode.G;

    private GeminiApiClient geminiApiClient;
    private bool isRequesting;

    private void Awake()
    {
        // RequireComponent 덕분에 같은 GameObject에 GeminiApiClient가 항상 존재합니다.
        geminiApiClient = GetComponent<GeminiApiClient>();
    }

    private void Start()
    {
        // runOnStart를 켜면 플레이 시작과 동시에 한 번 요청합니다.
        if (runOnStart)
        {
            RequestGemini();
        }
    }

    private void Update()
    {
        // 지정한 키를 누르면 Inspector의 prompt 내용으로 Gemini 요청을 보냅니다.
        if (triggerKey != KeyCode.None && Input.GetKeyDown(triggerKey))
        {
            RequestGemini();
        }
    }

    [ContextMenu("Request Gemini")]
    public void RequestGemini()
    {
        // 이전 요청이 끝나기 전에는 중복 요청을 보내지 않습니다.
        if (isRequesting)
        {
            return;
        }

        isRequesting = true;
        geminiApiClient.GenerateContent(prompt, HandleSuccess, HandleError);
    }

    private void HandleSuccess(string responseText)
    {
        isRequesting = false;
        // 데모에서는 결과를 콘솔에 출력합니다. 실제 게임에서는 UI나 대사 시스템에 연결하면 됩니다.
        Debug.Log($"Gemini response:\n{responseText}", this);
    }

    private void HandleError(string error)
    {
        isRequesting = false;
        // 실패 원인을 Unity 콘솔에서 바로 확인할 수 있게 남깁니다.
        Debug.LogError($"Gemini request failed: {error}", this);
    }
}
