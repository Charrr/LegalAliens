using UnityEngine;

namespace LegalAliens
{
	[CreateAssetMenu(fileName = "ApiKeyManager", menuName = "LegalAliens/ApiKeyManager", order = 1)]
    public class ApiKeyManager : ScriptableObject
	{
		[SerializeField] private string _openAIKey;
		public string OpenAIKey => _openAIKey;
    } 
}
