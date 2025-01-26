using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ObjectData : MonoBehaviour
{
	[Header("General Object Data")]
    public float mass;
	public enum FrictionMode {Instant, Gradual}
	public FrictionMode frictionType;
	public bool keepControllerSize;

    [HideInInspector] public Vector3 velocity;
	public Vector3 deltaVelocity;
	[HideInInspector] public float forceOfGravity;
	[HideInInspector] public bool isInWater;

	[HideInInspector] public CharacterController controller;
	[HideInInspector] public GlobalData data;

	MeshRenderer[] renderers;

	public void Start()
	{
		data = FindAnyObjectByType<GlobalData>();
		controller = GetComponent<CharacterController>();
		if (!keepControllerSize)
		{
			controller.radius = 0;
			controller.height = 0.01f;
		}

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
		if (frictionType == FrictionMode.Gradual)
		{
			velocity -= velocity * data.friction * Time.deltaTime;
		}

		Vector3 velocityLastFrame = velocity;
		if (!controller.isGrounded)
		{
			forceOfGravity = data.gravity * mass * Time.deltaTime;
			velocity += Vector3.down * forceOfGravity;
		}
		
		controller.Move(velocity * Time.deltaTime);
		
		deltaVelocity = velocity - velocityLastFrame;
	}


}
