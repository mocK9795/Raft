using UnityEngine;

public class Water : MonoBehaviour
{
	public float floatOffset;
	public float density;
	GlobalData data;

	private void Start()
	{
		data = FindAnyObjectByType<GlobalData>();
	}

	private void OnTriggerStay(Collider other)
	{
		ObjectData floatingObject = other.GetComponent<ObjectData>();
		if (floatingObject == null) return;
		float boyancyProvided = density * data.gravity * ((transform.position.y + floatOffset) - floatingObject.transform.position.y) * Time.deltaTime;
		floatingObject.velocity.y += boyancyProvided;
	}

	private void OnTriggerEnter(Collider other)
	{
		ObjectData floatingObject = other.GetComponent<ObjectData>();
		if (floatingObject == null) return;
		floatingObject.isInWater = true;
	}

	private void OnTriggerExit(Collider other)
	{
		ObjectData floatingObject = other.GetComponent<ObjectData>();
		if (floatingObject == null) return;
		floatingObject.isInWater = false;
	}
}
