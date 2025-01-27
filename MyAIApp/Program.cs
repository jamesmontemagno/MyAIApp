using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using MyAIApp;
using Microsoft.Extensions.VectorData;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var model = config["ModelName"]!; 
var modelEmbedding = config["ModelEmbeddingName"]!;
var key = config["OpenAIKey"]!;





while (true)
{
	// Get user prompt and add to chat history
	Console.WriteLine("Your prompt:");
	var userPrompt = Console.ReadLine();


	// Stream the AI response and add to chat history
	Console.WriteLine("AI Response:");



	Console.WriteLine();
}


