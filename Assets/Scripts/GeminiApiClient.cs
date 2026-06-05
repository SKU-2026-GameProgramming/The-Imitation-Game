using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public sealed class GeminiApiClient : MonoBehaviour
{
    private const string GeminiApiKeyEnvironmentVariable = "GEMINI_API_KEY";

    [Header("Gemini API")]
    [SerializeField] private string model = "gemini-2.5-flash";

    [Tooltip("Prefer setting GEMINI_API_KEY as an environment variable. For a throwaway local demo, paste the key here in the Inspector.")]
    // 데모에서는 Inspector 입력도 허용하지만, 실제 배포에서는 API 키를 클라이언트에 포함하지 않는 것이 안전합니다.
    [SerializeField] private string apiKey = "";

    [Header("Generation")]
    [Range(0f, 2f)]
    [SerializeField] private float temperature = 0.7f;

    [Range(1, 8192)]
    [SerializeField] private int maxOutputTokens = 1024;

    [Range(1, 120)]
    [SerializeField] private int timeoutSeconds = 30;

    // 외부 스크립트가 프롬프트를 넘겨 Gemini 요청을 시작하는 진입점입니다.
    public Coroutine GenerateContent(string prompt, Action<string> onSuccess, Action<string> onError = null)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            onError?.Invoke("Prompt is empty.");
            return null;
        }

        if (!TryGetApiKey(out var resolvedApiKey))
        {
            onError?.Invoke($"Gemini API key is missing. Set {GeminiApiKeyEnvironmentVariable} or enter it in the Inspector.");
            return null;
        }

        return StartCoroutine(SendGenerateContentRequest(prompt, resolvedApiKey, onSuccess, onError));
    }

    // UI 입력창 등에서 데모용 키를 런타임에 주입하고 싶을 때 사용할 수 있습니다.
    public void SetApiKeyForDemo(string value)
    {
        apiKey = value?.Trim() ?? "";
    }

    // 환경변수를 우선 사용하고, 없을 때만 Inspector에 입력된 값을 사용합니다.
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

    // Gemini REST API가 요구하는 JSON 요청 본문을 만들고 POST 요청을 보냅니다.
    private System.Collections.IEnumerator SendGenerateContentRequest(
        string prompt,
        string resolvedApiKey,
        Action<string> onSuccess,
        Action<string> onError)
    {
        var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent";
        var requestBody = new GeminiGenerateRequest
        {
            contents = new[]
            {
                new GeminiContent
                {
                    role = "user",
                    parts = new[]
                    {
                        new GeminiPart { text = prompt }
                    }
                }
            },
            generationConfig = new GeminiGenerationConfig
            {
                temperature = temperature,
                maxOutputTokens = maxOutputTokens,
                candidateCount = 1
            }
        };

        var json = JsonUtility.ToJson(requestBody);
        using (var request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST)
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
            downloadHandler = new DownloadHandlerBuffer(),
            timeout = timeoutSeconds
        })
        {
            request.SetRequestHeader("Content-Type", "application/json");
            // API 키는 코드에 직접 쓰지 않고, 요청 직전에 확인된 값만 헤더로 전달합니다.
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
    }

    // Gemini 응답의 candidates[].content.parts[].text 값을 하나의 문자열로 합칩니다.
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

    // Gemini JSON 오류 메시지가 있으면 우선 사용하고, 없으면 원본 응답을 로그에 남깁니다.
    private static string FormatError(long responseCode, string requestError, string responseBody)
    {
        var message = string.IsNullOrWhiteSpace(requestError) ? "Request failed." : requestError;
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
            // 일부 실패 응답은 Gemini JSON 오류 형식이 아니라 일반 텍스트나 HTML일 수 있습니다.
        }

        if (!string.IsNullOrWhiteSpace(errorEnvelope?.error?.message))
        {
            return $"HTTP {responseCode}: {errorEnvelope.error.status} - {errorEnvelope.error.message}";
        }

        return $"HTTP {responseCode}: {message}\n{responseBody}";
    }

    // 아래 클래스들은 Unity JsonUtility가 직렬화/역직렬화할 Gemini 요청/응답 DTO입니다.
    [Serializable]
    private sealed class GeminiGenerateRequest
    {
        public GeminiContent[] contents;
        public GeminiGenerationConfig generationConfig;
    }

    [Serializable]
    private sealed class GeminiGenerationConfig
    {
        public float temperature;
        public int maxOutputTokens;
        public int candidateCount;
    }

    [Serializable]
    private sealed class GeminiGenerateResponse
    {
        public GeminiCandidate[] candidates;
    }

    [Serializable]
    private sealed class GeminiCandidate
    {
        public GeminiContent content;
        public string finishReason;
    }

    [Serializable]
    private sealed class GeminiContent
    {
        public string role;
        public GeminiPart[] parts;
    }

    [Serializable]
    private sealed class GeminiPart
    {
        public string text;
    }

    [Serializable]
    private sealed class GeminiErrorEnvelope
    {
        public GeminiError error;
    }

    [Serializable]
    private sealed class GeminiError
    {
        public int code;
        public string message;
        public string status;
    }
}
