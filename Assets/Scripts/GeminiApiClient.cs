using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// Gemini REST API에 프롬프트를 전송하고 생성된 텍스트 응답을 Unity 코루틴으로 반환하는 클라이언트입니다.
public sealed class GeminiApiClient : MonoBehaviour
{
    /// Gemini API 키를 조회할 때 사용하는 환경 변수 이름입니다.
    private const string GeminiApiKeyEnvironmentVariable = "GEMINI_API_KEY";

    /// 요청을 보낼 Gemini 모델 이름입니다.
    [Header("Gemini API")]
    [Tooltip("Gemini 모델명")]
    [SerializeField] private string model = "gemini-2.5-flash";

    
    /// 환경 변수 대신 Inspector에서 임시로 입력할 Gemini API 키입니다.
    [Tooltip("일회성 로컬 테스트용 API 키입니다. 가능하면 환경 변수를 사용하세요.")]
    [SerializeField] private string apiKey = "";

    
    /// Gemini 모델 전체 응답 방식을 지시하는 시스템 인스트럭션입니다.
    [Header("Prompt")]
    [Tooltip("Gemini 요청 본문의 systemInstruction 값입니다. 비워두면 전송하지 않습니다.")]
    [TextArea(2, 8)]
    [SerializeField] private string systemInstruction = "";

    
    /// 생성 응답의 무작위성과 창의성을 조절하는 온도 값입니다.
    [Header("Generation")]
    [Tooltip(@"응답 무작위성
               0.0 ~ 0.3 : 매우 보수적
               0.7       : 일반적인 창의성
               1.0 이상  : 더 다양하고 실험적")]
    [Range(0f, 2f)]
    [SerializeField] private float temperature = 0.7f;

    
    /// Gemini가 생성할 수 있는 최대 출력 토큰 수입니다.
    [Tooltip("응답 길이 제한")]
    [Range(1, 8192)]
    [SerializeField] private int maxOutputTokens = 1024;

    
    /// Gemini API 요청이 완료될 때까지 기다리는 최대 시간(초)입니다.
    [Tooltip("응답 제한 시간")]
    [Range(1, 120)]
    [SerializeField] private int timeoutSeconds = 30;

    
    /// Gemini 모델에 프롬프트를 보내고 생성된 텍스트 응답을 비동기 코루틴으로 전달합니다.
    /// 
    /// <param name="prompt">Gemini 모델에 전달할 사용자 프롬프트입니다.</param>
    /// <param name="onSuccess">응답 텍스트를 성공적으로 추출했을 때 호출되는 콜백입니다.</param>
    /// <param name="onError">입력 검증, API 키 확인, 요청 또는 응답 파싱에 실패했을 때 호출되는 콜백입니다.</param>
    /// <returns>시작된 Unity 코루틴입니다. 프롬프트나 API 키가 유효하지 않으면 <c>null</c>을 반환합니다.</returns>
    public Coroutine GenerateContent(string prompt, Action<string> onSuccess, Action<string> onError = null)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            onError?.Invoke("프롬프트가 비어 있습니다.");
            return null;
        }

        if (!TryGetApiKey(out var resolvedApiKey))
        {
            onError?.Invoke($"Gemini API 키가 없습니다. 환경 변수 {GeminiApiKeyEnvironmentVariable}를 설정하거나 Inspector에 입력하세요.");
            return null;
        }

        Debug.Log($"{model} API 요청 시작", this);

        return StartCoroutine(SendGenerateContentRequest(prompt, resolvedApiKey, onSuccess, onError));
    }

    
    /// 데모 UI에서 입력한 API 키를 Inspector 직렬화 필드에 저장합니다.
    /// 
    /// <param name="value">데모 실행 중 사용할 Gemini API 키입니다.</param>
    public void SetApiKeyForDemo(string value)
    {
        apiKey = value?.Trim() ?? "";
    }

    
    /// Gemini API 키를 환경 변수에서 먼저 찾고, 없으면 Inspector에 입력된 값을 사용합니다.
    /// 
    /// <param name="resolvedApiKey">검증과 공백 제거를 마친 Gemini API 키입니다.</param>
    /// <returns>사용 가능한 API 키를 찾았으면 <c>true</c>, 없으면 <c>false</c>입니다.</returns>
    private bool TryGetApiKey(out string resolvedApiKey)
    {
        var environmentApiKey = Environment.GetEnvironmentVariable(GeminiApiKeyEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(environmentApiKey))
        {
            resolvedApiKey = environmentApiKey.Trim();
            return true;
        }

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            resolvedApiKey = apiKey.Trim();
            return true;
        }

        resolvedApiKey = "";
        return false;
    }

    
    /// Gemini REST API가 요구하는 JSON 요청 본문을 만들고 생성 요청을 전송합니다.
    /// 
    /// <param name="prompt">Gemini 모델에 전달할 사용자 프롬프트입니다.</param>
    /// <param name="resolvedApiKey">환경 변수 또는 Inspector에서 확인한 Gemini API 키입니다.</param>
    /// <param name="onSuccess">응답 텍스트를 성공적으로 추출했을 때 호출되는 콜백입니다.</param>
    /// <param name="onError">요청 실패 또는 응답 파싱 실패 시 오류 메시지와 함께 호출되는 콜백입니다.</param>
    /// <returns>Unity 코루틴 실행을 위한 IEnumerator입니다.</returns>
    private System.Collections.IEnumerator SendGenerateContentRequest(
        string prompt,
        string resolvedApiKey,
        Action<string> onSuccess,
        Action<string> onError)
    {
        var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent";
        var json = BuildGenerateContentRequestJson(prompt);

        using var request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.timeout = timeoutSeconds;
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-goog-api-key", resolvedApiKey);

        yield return request.SendWebRequest();

        var responseBody = request.downloadHandler?.text ?? "";
        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(FormatError(request.responseCode, request.error, responseBody));
            yield break;
        }

        var responseText = ExtractText(responseBody);
        if (string.IsNullOrWhiteSpace(responseText))
        {
            onError?.Invoke($"Gemini returned no text. Raw response: {responseBody}");
            yield break;
        }

        onSuccess?.Invoke(responseText);
    }

    
    /// Gemini REST API 요청 본문 JSON을 만듭니다.
    /// 
    /// <param name="prompt">Gemini 모델에 전달할 사용자 프롬프트입니다.</param>
    /// <returns>Gemini 생성 요청 JSON 문자열입니다.</returns>
    private string BuildGenerateContentRequestJson(string prompt)
    {
        var contents = new[]
        {
            new GeminiContent
            {
                role = "user",
                parts = new[]
                {
                    new GeminiPart { text = prompt }
                }
            }
        };

        var generationConfig = new GeminiGenerationConfig
        {
            temperature = temperature,
            maxOutputTokens = maxOutputTokens,
            candidateCount = 1
        };

        var trimmedSystemInstruction = systemInstruction?.Trim();
        if (string.IsNullOrWhiteSpace(trimmedSystemInstruction))
        {
            return JsonUtility.ToJson(new GeminiGenerateRequest
            {
                contents = contents,
                generationConfig = generationConfig
            });
        }

        return JsonUtility.ToJson(new GeminiGenerateRequestWithSystemInstruction
        {
            systemInstruction = new GeminiSystemInstruction
            {
                parts = new[]
                {
                    new GeminiPart { text = trimmedSystemInstruction }
                }
            },
            contents = contents,
            generationConfig = generationConfig
        });
    }

    
    /// Gemini 응답 JSON에서 모든 후보의 텍스트 파트를 순서대로 이어 붙입니다.
    /// 
    /// <param name="responseBody">Gemini API가 반환한 원본 JSON 응답 본문입니다.</param>
    /// <returns>추출한 응답 텍스트입니다. 파싱에 실패하거나 텍스트가 없으면 빈 문자열을 반환합니다.</returns>
    private static string ExtractText(string responseBody)
    {
        GeminiGenerateResponse response;
        try
        {
            response = JsonUtility.FromJson<GeminiGenerateResponse>(responseBody);
        }
        catch (ArgumentException)
        {
            return "";
        }

        if (response?.candidates == null || response.candidates.Length == 0)
        {
            return "";
        }

        var builder = new StringBuilder();
        foreach (var candidate in response.candidates)
        {
            var parts = candidate?.content?.parts;
            if (parts == null)
            {
                continue;
            }

            foreach (var part in parts)
            {
                if (!string.IsNullOrEmpty(part?.text))
                {
                    builder.Append(part.text);
                }
            }
        }

        return builder.ToString().Trim();
    }

    
    /// Gemini 오류 응답을 우선 파싱하고, 실패하면 HTTP 오류와 원본 응답 본문을 포함한 메시지를 만듭니다.
    /// 
    /// <param name="responseCode">HTTP 응답 코드입니다.</param>
    /// <param name="requestError">UnityWebRequest가 보고한 오류 메시지입니다.</param>
    /// <param name="responseBody">서버가 반환한 원본 응답 본문입니다.</param>
    /// <returns>로그 또는 UI에 표시할 수 있는 오류 메시지입니다.</returns>
    private static string FormatError(long responseCode, string requestError, string responseBody)
    {
        var message = string.IsNullOrWhiteSpace(requestError) ? "요청에 실패했습니다." : requestError;
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return $"HTTP {responseCode}: {message}";
        }

        GeminiErrorEnvelope errorEnvelope = null;
        try
        {
            errorEnvelope = JsonUtility.FromJson<GeminiErrorEnvelope>(responseBody);
        }
        catch (ArgumentException)
        {
            // 일부 실패 응답은 Gemini JSON 오류 형식이 아닌 일반 텍스트나 HTML일 수 있습니다.
        }

        if (!string.IsNullOrWhiteSpace(errorEnvelope?.error?.message))
        {
            return $"HTTP {responseCode}: {errorEnvelope.error.status} - {errorEnvelope.error.message}";
        }

        return $"HTTP {responseCode}: {message}\n{responseBody}";
    }

    
    /// Gemini 생성 요청 JSON의 최상위 DTO입니다.
    [Serializable]
    private sealed class GeminiGenerateRequest
    {
        
        /// 모델에 전달할 대화 콘텐츠 목록입니다.
        public GeminiContent[] contents;

        
        /// 생성 방식과 출력 제한을 정의하는 설정입니다.
        public GeminiGenerationConfig generationConfig;
    }

    
    /// 시스템 인스트럭션을 포함하는 Gemini 생성 요청 JSON의 최상위 DTO입니다.
    [Serializable]
    private sealed class GeminiGenerateRequestWithSystemInstruction
    {
        
        /// 모델 전체 응답 방식을 지시하는 시스템 인스트럭션입니다.
        public GeminiSystemInstruction systemInstruction;

        
        /// 모델에 전달할 대화 콘텐츠 목록입니다.
        public GeminiContent[] contents;

        
        /// 생성 방식과 출력 제한을 정의하는 설정입니다.
        public GeminiGenerationConfig generationConfig;
    }

    
    /// Gemini 텍스트 생성 설정 DTO입니다.
    [Serializable]
    private sealed class GeminiGenerationConfig
    {
        
        /// 응답의 무작위성과 다양성을 조절하는 값입니다.
        public float temperature;

        
        /// 생성 가능한 최대 출력 토큰 수입니다.
        public int maxOutputTokens;

        
        /// 요청할 응답 후보 개수입니다.
        public int candidateCount;
    }

    
    /// Gemini 생성 응답 JSON의 최상위 DTO입니다.
    [Serializable]
    private sealed class GeminiGenerateResponse
    {
        
        /// Gemini가 반환한 응답 후보 목록입니다.
        public GeminiCandidate[] candidates;
    }

    
    /// Gemini 응답 후보 하나를 나타내는 DTO입니다.
    [Serializable]
    private sealed class GeminiCandidate
    {
        
        /// 후보 응답의 실제 콘텐츠입니다.
        public GeminiContent content;

        
        /// 모델이 응답 생성을 종료한 이유입니다.
        public string finishReason;
    }

    
    /// Gemini 요청 또는 응답의 대화 콘텐츠 DTO입니다.
    [Serializable]
    private sealed class GeminiContent
    {
        
        /// 콘텐츠 작성자의 역할입니다.
        public string role;

        
        /// 콘텐츠를 구성하는 파트 목록입니다.
        public GeminiPart[] parts;
    }

    
    /// Gemini 시스템 인스트럭션 DTO입니다.
    [Serializable]
    private sealed class GeminiSystemInstruction
    {
        
        /// 시스템 인스트럭션을 구성하는 텍스트 파트 목록입니다.
        public GeminiPart[] parts;
    }

    
    /// Gemini 콘텐츠 안의 개별 텍스트 파트 DTO입니다.
    [Serializable]
    private sealed class GeminiPart
    {
        
        /// 모델에 전달하거나 모델이 반환한 텍스트입니다.
        public string text;
    }

    
    /// Gemini 오류 응답 JSON의 최상위 DTO입니다.
    [Serializable]
    private sealed class GeminiErrorEnvelope
    {
        
        /// Gemini API가 반환한 오류 상세 정보입니다.
        public GeminiError error;
    }

    
    /// Gemini API 오류 상세 DTO입니다.
    [Serializable]
    private sealed class GeminiError
    {
        
        /// HTTP 또는 API 수준의 오류 코드입니다.
        public int code;

        
        /// 사람이 읽을 수 있는 오류 메시지입니다.
        public string message;

        
        /// Gemini API가 제공하는 오류 상태 문자열입니다.
        public string status;
    }
}
