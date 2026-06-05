using UnityEngine;

/// <summary>
/// 플레이 모드에서 <see cref="GeminiApiClient"/>를 간단히 호출해 보는 데모용 컴포넌트입니다.
/// </summary>
/// <remarks>
/// 같은 게임 오브젝트에 <see cref="GeminiApiClient"/>가 필요하며, 시작 시 자동 실행하거나 지정된 키 입력으로 요청을 보낼 수 있습니다.
/// </remarks>
[RequireComponent(typeof(GeminiApiClient))]
public sealed class GeminiRunner : MonoBehaviour
{
    /// <summary>
    /// Gemini에 전달할 프롬프트 문장입니다.
    /// </summary>
    [Header("Prompt")]
    [Tooltip("프롬프트")]
    [TextArea(3, 8)]
    [SerializeField] private string prompt = "Write one short NPC line in Korean for a Unity demo project.";

    /// <summary>
    /// 게임 시작과 동시에 Gemini 요청을 실행할지 여부입니다.
    /// </summary>
    [Header("Controls")]
    [Tooltip("게임 실행과 동시에 API 호출 여부")]
    [SerializeField] private bool runOnStart;

    /// <summary>
    /// Gemini 요청을 실행하는 입력 키입니다.
    /// </summary>
    /// <remarks>
    /// 기본값은 <see cref="KeyCode.G"/>이며, <see cref="KeyCode.None"/>으로 설정하면 키 입력 실행이 비활성화됩니다.
    /// </remarks>
    [Tooltip("Gemini를 호출하는 키 입력값. KeyCode.None으로 설정하면 키 입력 실행이 비활성화.")]
    [SerializeField] private KeyCode triggerKey = KeyCode.G;

    /// <summary>
    /// 현재 Gemini 요청이 진행 중인지 나타냅니다.
    /// </summary>
    private bool _isRequesting;

    /// <summary>
    /// Gemini API 호출을 담당하는 클라이언트 컴포넌트입니다.
    /// </summary>
    private GeminiApiClient _geminiApiClient;

    /// <summary>
    /// 같은 게임 오브젝트에 연결된 <see cref="GeminiApiClient"/> 참조를 초기화합니다.
    /// </summary>
    private void Awake()
    {
        _geminiApiClient = GetComponent<GeminiApiClient>();
    }

    /// <summary>
    /// 설정에 따라 플레이 시작 시 Gemini 요청을 실행합니다.
    /// </summary>
    private void Start()
    {
        if (runOnStart)
        {
            RequestGemini();
        }
    }

    /// <summary>
    /// 매 프레임 지정된 트리거 키 입력을 확인하고 Gemini 요청을 실행합니다.
    /// </summary>
    private void Update()
    {
        if (triggerKey != KeyCode.None && Input.GetKeyDown(triggerKey))
        {
            RequestGemini();
        }
    }

    /// <summary>
    /// 현재 프롬프트를 사용해 Gemini 콘텐츠 생성을 요청합니다.
    /// </summary>
    /// <remarks>
    /// 이미 요청이 진행 중이면 중복 요청을 보내지 않습니다. Unity Inspector의 컨텍스트 메뉴에서도 호출할 수 있습니다.
    /// </remarks>
    [ContextMenu("Request Gemini")]
    public void RequestGemini()
    {
        if (_isRequesting)
        {
            Debug.Log("응답 대기 중", this);
            return;
        }

        _isRequesting = true;
        _geminiApiClient.GenerateContent(prompt, HandleSuccess, HandleError);
    }

    /// <summary>
    /// Gemini 요청이 성공했을 때 호출되는 콜백입니다.
    /// </summary>
    /// <param name="responseText">Gemini API가 반환한 응답 텍스트입니다.</param>
    private void HandleSuccess(string responseText)
    {
        _isRequesting = false;
        Debug.Log($"Gemini 응답:\n{responseText}", this);
    }

    /// <summary>
    /// Gemini 요청이 실패했을 때 호출되는 콜백입니다.
    /// </summary>
    /// <param name="error">실패 원인 또는 오류 메시지입니다.</param>
    private void HandleError(string error)
    {
        _isRequesting = false;
        Debug.LogError($"Gemini 요청 실패: {error}", this);
    }
}
