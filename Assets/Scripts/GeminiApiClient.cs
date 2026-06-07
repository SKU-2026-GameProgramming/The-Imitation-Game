using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
// [추가] Newtonsoft.Json 네임스페이스 로드
using Newtonsoft.Json;

/// Gemini REST API에 프롬프트를 전송하고 생성된 텍스트 응답을 Unity 코루틴으로 반환하는 클라이언트입니다.
public sealed class GeminiApiClient : MonoBehaviour
{
    private const string GeminiApiKeyEnvironmentVariable = "GEMINI_API_KEY";

    [Header("Gemini API")]
    [Tooltip("Gemini 모델명")]
    [SerializeField] private string model = "gemini-2.5-flash";

    [Tooltip("일회성 로컬 테스트용 API 키입니다. 가능하면 환경 변수를 사용하세요.")]
    [SerializeField] private string apiKey = "";

    [Header("Prompt")]
    [Tooltip("Gemini 요청 본문의 systemInstruction 값입니다. 비워두면 전송하지 않습니다.")]
    [TextArea(2, 8)]
    [SerializeField] private string systemInstruction = "";

    [Header("Generation")]
    [Tooltip(@"응답 무작위성")]
    [Range(0f, 2f)]
    [SerializeField] private float temperature = 0.7f;

    [Tooltip("응답 길이 제한")]
    [Range(1, 8192)]
    [SerializeField] private int maxOutputTokens = 16384; // 여유 있게 늘려두는 것을 권장합니다.

    [Tooltip("응답 제한 시간")]
    [Range(1, 120)]
    [SerializeField] private int timeoutSeconds = 30;

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

    public void SetApiKeyForDemo(string value)
    {
        apiKey = value?.Trim() ?? "";
    }

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

        // [변경] 일반 버퍼 대신 대용량 가차없이 받아내는 바이트 핸들러 지정
        var chunkHandler = new DownloadHandlerBuffer();
        request.downloadHandler = chunkHandler;

        request.timeout = timeoutSeconds;
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-goog-api-key", resolvedApiKey);

        yield return request.SendWebRequest();

        // 수신 완료 후 강제로 한번 더 인코딩 버퍼 동기화
        string responseBody = "";
        if (chunkHandler.data != null && chunkHandler.data.Length > 0)
        {
            responseBody = Encoding.UTF8.GetString(chunkHandler.data);
        }

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

    /// [수정] JsonUtility 대신 Newtonsoft.Json을 사용하여 안전하게 빌드합니다.
    private string BuildGenerateContentRequestJson(string prompt)
    {
        var contents = new[]
        {
            new GeminiContent
            {
                role = "user",
                parts = new[] { new GeminiPart { text = prompt } }
            }
        };

        // [핵심 변경] responseMimeType을 application/json으로 강제 지정
        var generationConfig = new
        {
            temperature = temperature,
            //maxOutputTokens = maxOutputTokens,
            candidateCount = 1,
            responseMimeType = "application/json",

            // 구글 서버에 우리가 받을 JSON의 구조(설계도)를 명확하게 주입합니다.
            responseSchema = new
            {
                type = "OBJECT",
                properties = new
                {
                    attacks = new
                    {
                        type = "ARRAY",
                        items = new
                        {
                            type = "OBJECT",
                            properties = new
                            {
                                nodeID = new { type = "INTEGER" },
                                power = new { type = "INTEGER" }
                            },
                            required = new[] { "nodeID", "power" }
                        }
                    },
                    defends = new
                    {
                        type = "ARRAY",
                        items = new
                        {
                            type = "OBJECT",
                            properties = new
                            {
                                nodeID = new { type = "INTEGER" },
                                power = new { type = "INTEGER" }
                            },
                            required = new[] { "nodeID", "power" }
                        }
                    }
                },
                required = new[] { "attacks", "defends" }
            }
        };

        var trimmedSystemInstruction = systemInstruction?.Trim();

        if (string.IsNullOrWhiteSpace(trimmedSystemInstruction))
        {
            var requestBody = new { contents = contents, generationConfig = generationConfig };
            return JsonConvert.SerializeObject(requestBody);
        }
        else
        {
            var requestBody = new
            {
                systemInstruction = new GeminiSystemInstruction
                {
                    parts = new[] { new GeminiPart { text = trimmedSystemInstruction } }
                },
                contents = contents,
                generationConfig = generationConfig
            };
            return JsonConvert.SerializeObject(requestBody);
        }
    }

    /// [수정] 대용량 줄바꿈 특수문자가 섞여도 절대 터지지 않도록 Newtonsoft.Json 파싱 적용
    private static string ExtractText(string responseBody)
    {
        try
        {
            // 유연하게 dynamic(혹은 JObject) 스타일로 파싱 후 추출
            var response = JsonConvert.DeserializeObject<GeminiGenerateResponse>(responseBody);

            if (response?.candidates == null || response.candidates.Length == 0)
            {
                return "";
            }

            var builder = new StringBuilder();
            foreach (var candidate in response.candidates)
            {
                var parts = candidate?.content?.parts;
                if (parts == null) continue;

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
        catch (Exception ex)
        {
            Debug.LogError($"[JSON 파싱 에러] 원인: {ex.Message}");
            return "";
        }
    }

    private static string FormatError(long responseCode, string requestError, string responseBody)
    {
        var message = string.IsNullOrWhiteSpace(requestError) ? "요청에 실패했습니다." : requestError;
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return $"HTTP {responseCode}: {message}";
        }

        try
        {
            var errorEnvelope = JsonConvert.DeserializeObject<GeminiErrorEnvelope>(responseBody);
            if (!string.IsNullOrWhiteSpace(errorEnvelope?.error?.message))
            {
                return $"HTTP {responseCode}: {errorEnvelope.error.status} - {errorEnvelope.error.message}";
            }
        }
        catch { }

        return $"HTTP {responseCode}: {message}\n{responseBody}";
    }

    // --- DTO 구조 유지 (대소문자 매핑을 위해 Newtonsoft 호환 구조) ---
    [Serializable] private sealed class GeminiGenerationConfig { public float temperature; public int maxOutputTokens; public int candidateCount; }
    [Serializable] private sealed class GeminiGenerateResponse { public GeminiCandidate[] candidates; }
    [Serializable] private sealed class GeminiCandidate { public GeminiContent content; public string finishReason; }
    [Serializable] private sealed class GeminiContent { public string role; public GeminiPart[] parts; }
    [Serializable] private sealed class GeminiSystemInstruction { public GeminiPart[] parts; }
    [Serializable] private sealed class GeminiPart { public string text; }
    [Serializable] private sealed class GeminiErrorEnvelope { public GeminiError error; }
    [Serializable] private sealed class GeminiError { public int code; public string message; public string status; }
}
