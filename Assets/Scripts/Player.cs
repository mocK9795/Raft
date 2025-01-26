using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
	float currentSpeed;
	Vector2 moveDirection;
	PlayerCamera playerCamera;
	ObjectData lastSelectedObject;
	bool isJumping;

	public void OnMove(InputAction.CallbackContext value)
	{
		moveDirection = value.ReadValue<Vector2>();
	}

	private new void Update()
	{
		if (isInWater)
		{
			currentSpeed = swimSpeed;
			if (isJumping) velocity.y += swimJumpHeight * Time.deltaTime;
		}
		else if (controller.isGrounded)
		{
			if (isJumping)
			{
				velocity.y += jumpHeight;
			}
			currentSpeed = speed;
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
		if (lastSelectedObject != null) lastSelectedObject.UnOutline();
		lastSelectedObject = null;

		if (!Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hitInfo, range, interactionMask)) return;
		ObjectData selectedObject = hitInfo.collider.GetComponent<ObjectData>();
		if (selectedObject == null) return;

		selectedObject.Outline();
		lastSelectedObject = selectedObject;
	}

	public void OnInteract(InputAction.CallbackContext value) { if (value.canceled) OnInteract(); }

	private new void Start()
	{
		base.Start();
		playerCamera = FindAnyObjectByType<PlayerCamera>();
	}
}
