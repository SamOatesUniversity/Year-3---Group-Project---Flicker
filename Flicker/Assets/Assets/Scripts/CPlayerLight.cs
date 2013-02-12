using UnityEngine;
using System.Collections;

public class CPlayerLight : MonoBehaviour {
	
	public GameObject LightPrefab = null;
	public float MaxRange = 3.0f;
	public float MinRange = 2.5f;
	public Color LightColor = new Color(1,1,1);
	public float MaxIntensity = 0.95f;
	public float MinIntensity = 0.1f;
	public float FlickerSpeed = 0.5f;
	public float LightCharge = 100.0f;
	public float LightDecharge = 10.0f;

	// 2 = 2% chance
	public int FlickerFrequency = 2;
	
	
	private UnityEngine.Light m_light = null;
	private float m_stepSize = 0.0f;
	private int m_reverse = 1;
	private float m_currentRange = 0;
	private float m_currentIntensity = 0;
	private float m_currentLightCharge = 0;
	
	
	
	
	public void ChargeLight(float ammount)
	{
		if (m_currentLightCharge < LightCharge)
		{
			m_currentLightCharge += ammount;
		}	
	}
	
	
	// Use this for initialization
	void Start () {
		m_currentRange = MaxRange;
		m_currentIntensity = MaxIntensity;
		m_stepSize = FlickerSpeed;
		m_currentLightCharge = LightCharge;
		
		if (LightPrefab)
		{
			SkinnedMeshRenderer mesh = GameObject.Find("Body_Lowpoly").GetComponent<SkinnedMeshRenderer>();
			Transform parentTransform = mesh.bones[9].transform;
			parentTransform = parentTransform.transform.FindChild("Bip001 Spine2");
			parentTransform = parentTransform.transform.FindChild("Bip001 Spine3");
			parentTransform = parentTransform.transform.FindChild("Bone017");
			parentTransform = parentTransform.transform.FindChild("Bone018");
			parentTransform = parentTransform.transform.FindChild("connectBone001");
			parentTransform = parentTransform.transform.FindChild("Bone030");
			parentTransform = parentTransform.transform.FindChild("LightAttatchBone");
			
			GameObject obj = (GameObject)Instantiate(LightPrefab, Vector3.zero, Quaternion.identity);
			m_light = obj.GetComponent<Light>().light;
			m_light.transform.parent = parentTransform;
			m_light.transform.localPosition = Vector3.zero;
			m_light.color = LightColor;
			m_light.range = MaxRange;
			m_light.intensity = MaxIntensity;
		}				
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_light == null)
		{
			Debug.LogWarning("Failed to ceate player light");
			return;	
		}
		
		if (m_currentRange > MaxRange)
		{
			m_reverse = -1;
		}
		if (m_currentRange < MinRange)
		{
			m_reverse = 1;
		}
		
		m_currentRange +=  (m_stepSize * Time.deltaTime) * m_reverse;
		
		m_light.range = m_currentRange;
		
		
		m_light.intensity = MaxIntensity * ( m_currentLightCharge / LightCharge );
		
		if (Random.Range(0,200) < FlickerFrequency && m_currentLightCharge > 0)	
		{
			float rand = Random.Range(MinIntensity, MaxIntensity);
			m_light.intensity = rand;
		}
		
		if ( m_currentLightCharge > -1)
		{
			m_currentLightCharge -= LightDecharge * Time.deltaTime;
		}
			
	}
}
