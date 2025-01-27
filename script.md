This uses OpenAI, get key and set "gpt-4o" as the model: https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/get-started-openai?tabs=azd&pivots=openai follow these instructions for secrets

## Demo 1

```csharp
// Create the IChatClient
IChatClient chatClient =
	new OpenAIClient(key).AsChatClient(model);
```

```csharp
// Start the conversation with context for the AI model
List<ChatMessage> chatHistory = new()
	{
		new ChatMessage(ChatRole.System, """
            You are a friendly hiking enthusiast who helps people discover fun hikes in their area.
            You introduce yourself when first saying hello.
            When helping people out, you always ask them for this information
            to inform the hiking recommendation you provide:

            1. The location where they would like to hike
            2. What hiking intensity they are looking for

            You will then provide three suggestions for nearby hikes that vary in length
            after you get that information. You will also share an interesting fact about
            the local nature on the hikes when making a recommendation. At the end of your
            response, ask if there is anything else you can help with.
        """)
	};

```

```csharp
//Add the user prompt in as a User message
chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));
```

```csharp
// Sent chat history to service and stream in response, add full response to history
var response = "";
	await foreach (var item in
		chatClient.CompleteStreamingAsync(chatHistory))
	{
		Console.Write(item.Text);
		response += item.Text;
	}
	chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));

```


## Demo 2: change to ollama

You will need to follow this tutorial to setup Ollama and get the phi3 model https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/quickstart-local-ai

```csharp
IChatClient chatClient =
	new OllamaChatClient(new Uri("http://localhost:11434/"), "phi3:mini");
```

## Demo 3: Embeddings

// Create the embedding generator
IEmbeddingGenerator<string, Embedding<float>> generator = new OpenAIClient(new ApiKeyCredential(key))
			.AsEmbeddingGenerator(modelId: modelEmbedding);

// Create and populate the vector store
var vectorStore = new InMemoryVectorStore();
var cloudServicesStore = vectorStore.GetCollection<int, CloudService>("cloudServices");
var cloudServices = CloudServiceHelpers.GetDefaults();
await cloudServicesStore.CreateCollectionIfNotExistsAsync();

foreach (var service in cloudServices)
{
	service.Vector = await generator.GenerateEmbeddingVectorAsync(service.Description);
	await cloudServicesStore.UpsertAsync(service);
}


// Convert a search query to a vector and search the vector store
while (true)
{
	Console.WriteLine("Your prompt:");
	var userPrompt = Console.ReadLine();
	var queryEmbedding = await generator!.GenerateEmbeddingVectorAsync(userPrompt);

	var results = await cloudServicesStore.VectorizedSearchAsync(queryEmbedding, new VectorSearchOptions()
	{
		Top = 1,
		VectorPropertyName = "Vector"
	});

	await foreach (var result in results.Results)
	{
		Console.WriteLine($"Name: {result.Record.Name}");
		Console.WriteLine($"Description: {result.Record.Description}");
		Console.WriteLine($"Vector match score: {result.Score}");
		Console.WriteLine();
	}
}



ollama run phi3:mini