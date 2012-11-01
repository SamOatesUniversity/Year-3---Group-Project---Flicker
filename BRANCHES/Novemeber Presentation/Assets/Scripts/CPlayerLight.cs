using UnityEngine;
using System.Collections;

public class CPlayerLight : MonoBehaviour {
	
	public GameObject LightPrefab = null;
	public float MaxRange = 2.0f;
	public float MinRange = 0.0f;
	public Color LightColor = new Color(1,1,1);
	public float MaxIntensity = 0.1f;
	public float MinIntensity = 0.0f;
	public float FlickerSpeed = 0.5f;

	// 2 = 2% chance
	public int FlickerFrequency = 2;
	
	
	private UnityEngine.Light m_light = null;
	private float m_stepSize = 0.0f;
	private int m_reverse = 1;
	private float m_currentRange = 0;
	private float m_currentIntensity = 0;

	// Use this for initialization
	void Start () {
		m_currentRange = MaxRange;
		m_currentIntensity = MaxIntensity;
		m_stepSize = FlickerSpeed;
		
		if (LightPrefab)
		{
			Transform parentTransform = this.GetComponent<CEntityPlayer>().transform;
			GameObject obj =  (GameObject)Instantiate(LightPrefab,transform.position,Quaternion.identity);
			m_light = obj.GetComponent<Light>().light;
			m_light.transform.parent = parentTransform;
			m_light.color = LightColor;
			m_light.range = MaxRange;
			m_light.intensity = MaxIntensity;
		}
				
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_currentRange > MaxRange)
		{
			m_reverse = -1;
		}
		if (m_currentRange < MinRange)
		{
			m_reverse = 1;
		}
		
		m_currentRange += (m_stepSize * Time.deltaTime) * m_reverse;
		m_light.range = m_currentRange;
		
		
		m_light.intensity = MaxIntensity;
		if (Random.Range(0,200) < FlickerFrequency)	
		{
			float rand = Random.Range(MinIntensity, MaxIntensity);
			m_light.intensity = rand;
		}
		
		
	}
}
