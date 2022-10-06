using Assets.Scripts.System;
using System.Collections.Generic;
using UnityEngine;

public class HelperController : MonoBehaviour
{
	[SerializeField] private PlayerController _player;
	[SerializeField] private int              _positionBufferSize;
	[SerializeField] private float            _minDistance;
	[SerializeField] private Animator         _anim;
	[SerializeField] private SpriteRenderer   _sprite;

	private Queue<BufferedPosition> _positionBuffer;

	private BufferedPosition _lastPosition;
	private float            _originalXScale;
	private float            _currentFlipVelocity;
	private bool _moving;

	// Start is called before the first frame update
	private void Awake()
	{
		_originalXScale = transform.localScale.x;

		_positionBuffer = new Queue<BufferedPosition>(_positionBufferSize);
		_lastPosition =
			new BufferedPosition(_player.transform.position, _player.transform.localScale.x > 0);

		_player.OnMove += OnPlayerMove;
	}

	private void Update()
	{
		if (!PotatoDialogueManager.Instance.Talking && !Notepad.instance.opened) {
			_anim.SetBool("Walking", _moving);

			_sprite.transform.position = new Vector3(_sprite.transform.position.x,
				_sprite.transform.position.y, transform.position.y / 50);
		}else
			_anim.SetBool("Walking", false);

		int direction = _lastPosition.FacingRight ? 1 : -1;

		float scaleX = Mathf.SmoothDamp(transform.localScale.x, direction * _originalXScale,
			ref _currentFlipVelocity, .1f);
		transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
	}

	private void OnPlayerMove(Vector2 a_newPosition, Vector2 a_velocity)
	{
		_moving = false;

		if (a_velocity.sqrMagnitude < 0.1f) return;

		if (_positionBuffer.Count >= _positionBufferSize)
		{
			_lastPosition      = _positionBuffer.Dequeue();
			transform.position = _lastPosition.Position;
			_moving = true;
		}

		_positionBuffer.Enqueue(new BufferedPosition(a_newPosition, a_velocity.x > 0));
	}

	public struct BufferedPosition
	{
		public BufferedPosition(Vector2 a_position, bool a_facingRight)
		{
			Position    = a_position;
			FacingRight = a_facingRight;
		}

		public Vector2 Position;
		public bool    FacingRight;
	}
}