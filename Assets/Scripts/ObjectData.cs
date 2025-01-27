using UnityEngine;

public class ObjectData : MonoBehaviour
{
	[Header("General Object Data")]
    public float mass;
	public enum MoveMode {Transform, RigidBody, CharacterController}
	public MoveMode moveMode;
	public bool interact = true;
	public bool createConvexMesh = true;
    public Vector3 velocity;
	Vector3 deltaVelocity;
	[HideInInspector] public float forceOfGravity;
	[HideInInspector] public bool isInWater;

	[HideInInspector] public GlobalData data;
	[HideInInspector] Rigidbody body;
	[HideInInspector] public CharacterController controller;

	MeshRenderer[] renderers;



	public void Start()
	{
		body = GetComponent<Rigidbody>();
		if (body != null)
		{
			body.useGravity = false;
			body.isKinematic = true;
		}
		controller = GetComponent<CharacterController>();

		data = FindAnyObjectByType<GlobalData>();

		renderers = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < renderers.Length; i++)
		{
			Material[] renderMaterials = renderers[i].materials;
			Material[] addedMaterials = new Material[renderMaterials.Length + 1];
			for (int materialIndex = 0; materialIndex < renderMaterials.Length; materialIndex++)
			{
				addedMaterials[materialIndex] = renderMaterials[materialIndex];
			}
			addedMaterials[renderMaterials.Length] = data.nullMaterial;
			renderers[i].materials = addedMaterials;
		}

		if (createConvexMesh)
		{
			MeshCollider trigerCollider = gameObject.AddComponent<MeshCollider>();
			trigerCollider.convex = true;
			trigerCollider.isTrigger = true;
		}
	}

	public void Outline()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			Material[] materials = renderer.materials;
			materials[renderer.materials.Length - 1] = data.outlineMaterial;
			renderer.materials = materials;
		}
	}

	public void UnOutline()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			Material[] materials = renderer.materials;
			materials[renderer.materials.Length - 1] = data.nullMaterial;
			renderer.materials = materials;
		}
	}

	public void Update()
	{
		velocity -= velocity * data.friction * Time.deltaTime;

		Vector3 velocityLastFrame = velocity;
		forceOfGravity = data.gravity * mass * Time.deltaTime;
		velocity += Vector3.down * forceOfGravity;

		if (moveMode == MoveMode.Transform)
		{
			transform.Translate(velocity * Time.deltaTime, Space.World);
		}
		else if (moveMode == MoveMode.RigidBody)
		{
			body.MovePosition(transform.position + velocity * Time.deltaTime);
		}
		else if (moveMode == MoveMode.CharacterController)
		{
			controller.Move(velocity * Time.deltaTime);
		}
		deltaVelocity = velocity - velocityLastFrame;
	}

	private void OnTriggerEnter(Collider other)
	{
		ObjectData obj = other.gameObject.GetComponent<ObjectData>();
		if (obj == null) return;
		if (!obj.interact) return;
		print(name + " velocity " + velocity.ToString());
		velocity += obj.velocity * 0.5f;
		obj.velocity = -obj.velocity * 0.5f;
		print(name + " velocity " + velocity.ToString());
	}
}
