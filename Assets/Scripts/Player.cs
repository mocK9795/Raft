using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : ObjectData
{
	[Header("Player Statistics")]
	public float range;
	public float jumpHeight;
	public float swimJumpHeight;
	public float speed;
	public float swimSpeed;
	[Header("Interactable Object Mask")]
	public LayerMask interactionMask;
	[Header("Interaction")]
	public float boatLeaveVelocity = 2f;
	float currentSpeed;
	Vector2 moveDirection;
	PlayerCamera playerCamera;
	ObjectData lastSelectedObject;
	Interactable lastInteractable;
	bool isJumping;

	bool boatInteracter;

	public void OnMove(InputAction.CallbackContext value)
	{
		moveDirection = value.ReadValue<Vector2>();
	}

	private new void Update()
	{
		if (isInWater)
		{
			if (isJumping) velocity.y += swimJumpHeight * Time.deltaTime;
			currentSpeed = swimSpeed;
		}
		else if (controller.isGrounded)
		{
			if (isJumping)
			{
				velocity.y += jumpHeight;
			}
			currentSpeed = speed;
		}
		if (boatInteracter)
		{
			currentSpeed = lastInteractable.speedBonus;
		}
		velocity += (transform.right * moveDirection.x + transform.forward * moveDirection.y) * currentSpeed * Time.deltaTime;
			
		base.Update();
	}

	public void OnJump(InputAction.CallbackContext value)
	{
		isJumping = !value.canceled;
	}

	public void OnInteract()
	{
		if (lastInteractable != null)
		{
			if (boatInteracter)
			{
				lastInteractable.transform.parent = transform.parent;
				velocity += Vector3.up * jumpHeight * boatLeaveVelocity;
			}
		}
		boatInteracter = false;
		lastInteractable = null;

		int lastSelectedId = -1;
		if (lastSelectedObject != null)
		{
			lastSelectedObject.UnOutline();
			lastSelectedId = lastInteractable.GetInstanceID();
		}
		lastSelectedObject = null;

		if (!Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hitInfo, range, interactionMask)) return;
		ObjectData selectedObject = hitInfo.collider.GetComponent<ObjectData>();
		if (selectedObject == null) return;
		if (selectedObject.GetInstanceID() == lastSelectedId) return;

		selectedObject.Outline();
		lastSelectedObject = selectedObject;
		
		Interactable interacter = selectedObject.GetComponent<Interactable>();
		if (interacter == null) return;

		lastInteractable = interacter;

		if (interacter.interacterType == Interactable.InteractionType.Boat)
		{
			boatInteracter = true;
			transform.position = interacter.transform.position;
			transform.position += interacter.offset;
			interacter.transform.parent = transform;
		}
		
	}

	public void OnInteract(InputAction.CallbackContext value) { if (value.canceled) OnInteract(); }

	private new void Start()
	{
		base.Start();
		playerCamera = FindAnyObjectByType<PlayerCamera>();
		controller = GetComponent<CharacterController>();
		interact = true;
	}
}
