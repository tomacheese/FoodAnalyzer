using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using FoodAnalyzer.Core.Azure.Models;
using OpenAI.Chat;

namespace FoodAnalyzer.Core.Azure;

/// <summary>
/// Azure OpenAI サービスを利用して食事画像の分析を行うクラスです。
/// </summary>
internal class AzureOpenAI
{
    private readonly ChatClient _chatClient;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// <see cref="AzureOpenAI"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="endpoint">Azure OpenAI エンドポイントの URI。</param>
    /// <param name="apiKey">API キー。</param>
    /// <param name="deploymentName">デプロイメント名。</param>
    public AzureOpenAI(string endpoint, string apiKey, string deploymentName)
    {
        var azureClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        _chatClient = azureClient.GetChatClient(deploymentName);
    }

    /// <summary>
    /// 指定された画像 URL の食事画像を分析し、食品ごとの栄養情報や位置情報を推定します。
    /// </summary>
    /// <param name="url">分析対象の画像 URL。</param>
    /// <param name="width">画像の幅（ピクセル）。</param>
    /// <param name="height">画像の高さ（ピクセル）。</param>
    /// <returns>分析結果を含む <see cref="FoodAnalysisResponse"/> のタスク。</returns>
    public async Task<FoodAnalysisResponse> AnalyzeFoodAsync(string url, int width, int height)
    {
        // Generate schema from FoodAnalysisResponse class
        var foodSchemaJson = JsonSchemaGenerator.GenerateSchema<FoodAnalysisResponse>();

        var promptText =
$@"この食事の画像を分析してください。

重要な指示:
1. 画像内の各食品項目を識別し、その栄養情報を推定してください。値は日本語で記載します。
2. バウンディングボックスの座標は、画像の左上を原点(0,0)、右下を(1,1)とする正規化座標で指定してください。
3. bbox のフィールドは以下の通りです：
   - x_ratio: 食品の左端のX座標（0～1の範囲）
   - y_ratio: 食品の上端のY座標（0～1の範囲）
   - width_ratio: 食品の幅（0～1の範囲）
   - height_ratio: 食品の高さ（0～1の範囲）
4. 例：画像の中央にある食品なら、x_ratio=0.25, y_ratio=0.25, width_ratio=0.5, height_ratio=0.5 のようになります。
5. 各食品が画像内のどこにあるか、正確に位置を特定してください。
6. 出力は必ず指定されたJSONスキーマに厳密に従ってください。
7. total.message フィールドには、画像内の食品群について栄養情報の面からの総評を日本語で記載してください。

画像の実際のサイズ: {width}x{height}ピクセル（この情報は参考用です。座標は必ず0-1の正規化座標で返してください）";
        var textPart = ChatMessageContentPart.CreateTextPart(promptText);

        var imagePart = ChatMessageContentPart.CreateImagePart(new Uri(url));

        var messages = new List<ChatMessage>
        {
            new UserChatMessage(textPart, imagePart),
        };

        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "food_schema",
                jsonSchema: BinaryData.FromString(foodSchemaJson),
                jsonSchemaIsStrict: true
            ),
        };

        ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options).ConfigureAwait(false);

        FoodAnalysisResponse? result = JsonSerializer.Deserialize<FoodAnalysisResponse>(completion.Content[0].Text, _jsonOptions);
        return result ?? new FoodAnalysisResponse();
    }
}
