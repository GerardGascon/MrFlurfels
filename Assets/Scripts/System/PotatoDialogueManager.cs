using System;
using System.Collections.Generic;
using DG.Tweening;
using SimpleTools;
using SimpleTools.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.System
{
	public class PotatoDialogueManager : MonoBehaviour
	{
		private DialogueVertexAnimator _dialogueVertexAnimator;

		private Queue<string>       _sentences;
		private Queue<bool>         _displayNames;
		private Queue<string>       _characterNames;
		private Queue<Sprite>       _characterImages;
		private Queue<SentenceSide> _sides;
		public bool Talking { get; private set; }

		public  PotatoDialogueItems DialogueItems;
		private Coroutine           _typeRoutine;

		public static PotatoDialogueManager Instance;


		private void Awake()
		{
			Instance         = this;
			_sentences       = new Queue<string>();
			_displayNames    = new Queue<bool>();
			_characterNames  = new Queue<string>();
			_characterImages = new Queue<Sprite>();
			_sides           = new Queue<SentenceSide>();

			_dialogueVertexAnimator = new DialogueVertexAnimator(DialogueItems.TextBox);

			SetCharacterImage(SentenceSide.Left, null);
			SetCharacterImage(SentenceSide.Right, null);
		}

		public void SetCharacterImage(SentenceSide a_side, Sprite a_sprite)
		{
			Image image = a_side == SentenceSide.Left
				? DialogueItems.LeftCharacterImage
				: DialogueItems.RightCharacterImage;

			if (a_sprite == null)
			{
				image.enabled = false;
			}
			else
			{
				image.enabled = true;
				image.sprite  = a_sprite;
			}
		}

		public bool Dialogue(PotatoDialogue a_dialogue)
		{
			return Dialogue(a_dialogue, string.Empty);
		}

		public bool Dialogue(PotatoDialogue a_dialogue, params string[] a_sounds)
		{
			_dialogueVertexAnimator.SetAudioSourceGroup(a_sounds);

			if (!Talking)
			{
				SetCharacterImage(SentenceSide.Left, null);
				SetCharacterImage(SentenceSide.Right, null);

				_sentences.Clear();
				if (a_dialogue.Sentences.Length != 0)
					foreach (PotatoSentence sentence in a_dialogue.Sentences)
					{
						_sentences.Enqueue(sentence.Sentence);
						_displayNames.Enqueue(sentence.DisplayName);
						_characterNames.Enqueue(sentence.CharacterName);
						_characterImages.Enqueue(sentence.CharacterImage);
						_sides.Enqueue(sentence.Side);
					}
				else
					_sentences.Enqueue("I am error. No text has been added");

				Talking = true;

				if (_sentences.Count == 0) {
					if (_dialogueVertexAnimator.IsMessageAnimating())
						return true;
					Talking = false;
					return false;
				}

				string       sentenceToShow = _sentences.Peek();
				bool         displayName    = _displayNames.Peek();
				string       characterName  = _characterNames.Peek();
				Sprite       characterImage = _characterImages.Peek();
				SentenceSide side           = _sides.Peek();
				if (PlayDialogue(sentenceToShow, displayName, characterName, characterImage, side)) {
					_sentences.Dequeue();
					_displayNames.Dequeue();
					_characterNames.Dequeue();
					_characterImages.Dequeue();
					_sides.Dequeue();
				}

				return true;
			}
			else
			{
				if (_sentences.Count == 0)
				{
					if (_dialogueVertexAnimator.IsMessageAnimating())
						return true;
					Talking = false;
					return false;
				}

				string       sentenceToShow = _sentences.Peek();
				bool         displayName    = _displayNames.Peek();
				string       characterName  = _characterNames.Peek();
				Sprite       characterImage = _characterImages.Peek();
				SentenceSide side           = _sides.Peek();
				if (PlayDialogue(sentenceToShow, displayName, characterName, characterImage, side))
				{
					_sentences.Dequeue();
					_displayNames.Dequeue();
					_characterNames.Dequeue();
					_characterImages.Dequeue();
					_sides.Dequeue();
				}

				return true;
			}
		}

		private bool PlayDialogue(
			string       a_message,
			bool         a_displayName    = false,
			string       a_characterName  = "",
			Sprite       a_characterImage = null,
			SentenceSide a_side           = SentenceSide.Right)
		{
			if (_dialogueVertexAnimator.IsMessageAnimating())
			{
				_dialogueVertexAnimator.SkipToEndOfCurrentMessage();
				return
					false; //Next message hasn't been shown because the current one is still animating.
			}

			this.EnsureCoroutineStopped(ref _typeRoutine);
			_dialogueVertexAnimator.textAnimating = false;
			List<DialogueCommand> commands =
				DialogueUtility.ProcessInputString(a_message, out string totalTextMessage);
			_typeRoutine =
				StartCoroutine(
					_dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, null));


			Color       fadedColor    = new Color(0.61f, 0.61f, 0.61f);
			float       scaleUnfaded  = 0.8f;
			const float animationTime = 0.2f;

			Image left  = DialogueItems.LeftCharacterImage;
			Image right = DialogueItems.RightCharacterImage;

			switch (a_side)
			{
				case SentenceSide.Left:
					left.rectTransform.DOScale(Vector3.one, animationTime);
					left.DOColor(Color.white, animationTime);

					right.rectTransform.DOScale(new Vector3(-1, 1, 1) * scaleUnfaded, animationTime);
					right.DOColor(fadedColor, animationTime);

					SetCharacterImage(a_side, a_characterImage);
					break;
				case SentenceSide.Right:
					left.rectTransform.DOScale(Vector3.one * scaleUnfaded, animationTime);
					left.DOColor(fadedColor, animationTime);

					right.rectTransform.DOScale(new Vector3(-1, 1, 1), animationTime);
					right.DOColor(Color.white, animationTime);

					SetCharacterImage(a_side, a_characterImage);
					break;
				default:
					left.rectTransform.DOScale(Vector3.one * scaleUnfaded, animationTime);
					left.DOColor(fadedColor, animationTime);

					right.rectTransform.DOScale(new Vector3(-1, 1, 1) * scaleUnfaded, animationTime);
					right.DOColor(fadedColor, animationTime);
					break;
			}

			if (a_displayName)
			{
				DialogueItems.CharacterNameTag.SetActive(true);
				DialogueItems.CharacterName.text = a_characterName;
			}
			else
				DialogueItems.CharacterNameTag.SetActive(false);

			DialogueItems.TextContinueImage.enabled = _sentences.Count > 1;

			return true; //Next message shown successfully
		}
	}

	[Serializable]
	public struct PotatoDialogueItems
	{
		public Image    LeftCharacterImage;
		public Image    RightCharacterImage;
		public GameObject CharacterNameTag;
		public TMP_Text CharacterName;
		public TMP_Text TextBox;
		public Image    TextContinueImage;
	}

	public class PotatoDialogue : ScriptableObject
	{
		public PotatoSentence[] Sentences;
	}

	public class PotatoSentence
	{
		public                   bool         DisplayName;
		public                   string       CharacterName;
		public                   Sprite       CharacterImage;
		public                   SentenceSide Side;
		[TextArea(5, 10)] public string       Sentence;
	}

	public enum SentenceSide
	{
		None,
		Left,
		Right
	}
}