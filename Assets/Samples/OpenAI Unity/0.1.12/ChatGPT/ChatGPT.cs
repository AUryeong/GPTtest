using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ScrtipableObjects;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private AudioSource audioSource;

        public NpcSetting npc;
        public TextAsset asset;

        public bool isAssetLoading = false;

        private float height;
        private OpenAIApi openai = new OpenAIApi("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");

        private List<ChatMessage> messages = new();

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (isAssetLoading)
            {
                string[] lines = asset.text.Split('\n');
                npc.prompt.Clear();
                npc.examples.Clear();
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] colummn = lines[i].Split('\t');
                    npc.prompt.Add(colummn[0]);
                    npc.examples.Add(new NpcExample()
                    {
                        question = colummn[1],
                        answer = colummn[2]
                    });
                }
            }
            foreach (var prom in npc.prompt)
            {
                messages.Add(new ChatMessage()
                {
                    Role = "system",
                    Content = prom.Trim()
                });
            }

            foreach (var example in npc.examples)
            {
                messages.Add(new ChatMessage()
                {
                    Role = "user",
                    Content = example.question
                });
                messages.Add(new ChatMessage()
                {
                    Role = "assistant",
                    Content = example.answer
                });
            }
            
            button.onClick.AddListener(SendReply);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return))
                SendReply();
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        IEnumerator DownloadTheAudio(string text)
        {
            string url = $"http://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q={text}&tl=ko-kr";
            WWW www = new WWW(url);
            yield return www;

            audioSource.clip = www.GetAudioClip(false,true, AudioType.MPEG);
            audioSource.Play();
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);
            
            messages.Add(newMessage);
            
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-16k",
                Messages = messages,
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                
                messages.Add(message);
                StartCoroutine(DownloadTheAudio(message.Content));
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
            
            button.enabled = true;
            inputField.enabled = true;
        }
    }
}
