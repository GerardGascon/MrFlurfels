using Assets.Scripts.System;
using Assets.Scripts.TestScenes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleTools.AudioManager;

public class PlayerController : MonoBehaviour
{
	public Action<Vector2, Vector2> OnMove;

	[SerializeField] private float          _speed;
	[SerializeField] private Rigidbody2D    _rb;
	[SerializeField] private Animator       _anim;
	[SerializeField] private SpriteRenderer _sprite;
	[SerializeField] private DialoguePrompterManager _dialoguePrompterManager;

	private readonly List<DialoguePrompter> _prompters = new List<DialoguePrompter>();

	private Vector2 _velocity,            _currentVelocity;
	private float   _currentFlipVelocity, _originalXScale;
	private int     _direction = 1;

	[HideInInspector] public bool talkingBlocked;

	public List<object> moveBlockers = new List<object>();

	RiverTravel riverTravel;

	private void Awake()
	{
		_originalXScale = transform.localScale.x;
	}

	private void Start() {
		AudioManager.instance.Play("musica_juego");
	}

	private void Update()
	{
		//Movement
		if (!PotatoDialogueManager.Instance.Talking && !Notepad.instance.opened && moveBlockers.Count == 0)
		{
			_anim.SetBool("Walking", _velocity.sqrMagnitude > 0.1f);

			_sprite.transform.position = new Vector3(_sprite.transform.position.x,
				_sprite.transform.position.y, transform.position.y / 50);

			float horizontal = Input.GetAxisRaw("Horizontal");
			if (horizontal > 0.2) _direction = 1;
			else if (horizontal < -0.2) _direction = -1;
		} else
			_anim.SetBool("Walking", false);

		float scaleX = Mathf.SmoothDamp(transform.localScale.x, _direction * _originalXScale,
			ref _currentFlipVelocity, .1f);
		transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

		//Interaction
		if (Input.GetButtonDown("Submit") && moveBlockers.Count == 0)
		{
			if (_prompters.Count > 0 && !_dialoguePrompterManager.PlayingDialogue && !Notepad.instance.opened) {
				if (!talkingBlocked) _dialoguePrompterManager.PlayDialogue(_prompters.Last().GetDialogue());
				else talkingBlocked = false;
			} else {
				DialoguePrompterManager d = FindObjectOfType<DialoguePrompterManager>();
				if (TimeBar.instance.currentTime == 100 && riverTravel /*&& tienes_remos*/ && d._gameVariables.ContainsKey("tienes_remos") && d._gameVariables["tienes_remos"]) riverTravel.ChangeSide(); 
			}
		}
	}

	const float stepInterval = .4f;
	float currentStepInterval = 0;
	private void FixedUpdate()
	{
		if (PotatoDialogueManager.Instance.Talking || Notepad.instance.opened || moveBlockers.Count > 0)
			return;
		
		float   x              = Input.GetAxisRaw("Horizontal");
		float   y              = Input.GetAxisRaw("Vertical");
		Vector2 targetVelocity = new Vector3(x, y).normalized;
		_velocity = Vector2.SmoothDamp(_velocity, targetVelocity, ref _currentVelocity, .1f);

		if(_velocity.magnitude > .5f){
			currentStepInterval += Time.deltaTime;
			if (currentStepInterval >= stepInterval) {
				AudioManager.instance.PlayRandomSound("paso_duro_1", "paso_duro_2", "paso_duro_3", "paso_duro_4");
				currentStepInterval = 0;
			}
		}

		if (_velocity.normalized == Vector2.zero) return;

		_rb.MovePosition(_rb.position + _velocity * _speed * Time.fixedDeltaTime);
		OnMove?.Invoke(_rb.position, _velocity);
	}

	public void AddPrompter(DialoguePrompter a_dialoguePrompter)
	{
		if (!_prompters.Contains(a_dialoguePrompter)) {
			_prompters.Add(a_dialoguePrompter);
		}
	}

	public void RemovePrompter(DialoguePrompter a_dialoguePrompter)
	{
		if (_prompters.Contains(a_dialoguePrompter))
			_prompters.Remove(a_dialoguePrompter);
	}

	private void OnTriggerEnter2D(Collider2D col) {
		DialoguePrompter d = col.GetComponent<DialoguePrompter>();
		if (d)
			AddPrompter(d);

		riverTravel = col.GetComponent<RiverTravel>() ?? riverTravel;
	}

	private void OnTriggerStay2D(Collider2D col) {
		riverTravel = col.GetComponent<RiverTravel>() ?? riverTravel;
	}

	void OnTriggerExit2D(Collider2D col) {
		DialoguePrompter d = col.GetComponent<DialoguePrompter>();
		if (d)
			RemovePrompter(d);

		if (col.GetComponent<RiverTravel>()){
			riverTravel = null;
		}
	}
}