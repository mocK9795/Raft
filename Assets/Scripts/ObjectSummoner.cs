using UnityEngine;

public class ObjectSummoner : MonoBehaviour
{
	public GameObject[] prefabs;
	public Water water;
	public float summonTime;
	public float summonVariation;
	float currentSummonTime;
	float summonTimer;

	private void Start()
	{
		currentSummonTime = summonTime + Random.Range(-summonVariation, summonVariation);
	}

	private void Update()
	{
		if (summonTimer > currentSummonTime)
		{
			currentSummonTime = summonTime + Random.Range(-summonVariation, summonVariation);
			summonTimer = 0;
			float summonX = water.transform.position.x - (water.box.size.x * water.transform.localScale.x) / 3;
			float summonZ = water.transform.position.z - (water.box.size.z * water.transform.localScale.z) / 3;
			int summonIndex = Random.Range(0, prefabs.Length - 1);
			GameObject createdObject = Instantiate(prefabs[summonIndex]);
			createdObject.transform.position = new Vector3(summonX, 1, summonZ);
		}
		summonTimer += Time.deltaTime;
	}
}
