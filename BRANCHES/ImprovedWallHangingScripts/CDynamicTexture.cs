using UnityEngine;
using System.Collections;
 
class CDynamicTexture : MonoBehaviour
{
    public int columns = 2;
    public int rows = 2;
    public float framesPerSecond = 10f;
	public bool loop = true;
	
	public bool fadeIn = false;
	public float fadeInTime = 3.0f; 
	
	public float pauseAtStartTime = 3.0f;
	
    //the current frame to display
    private int index = 0;
	
	private bool isPlaying = true;
	private bool isOnFadeIn = false;
	private bool isOnPauseStart = false;
	
	private float fadeAlpha = 0.0f;
	
	private float startTime = 0.0f;
		
	private Vector2 size = Vector2.one;

    void Start()
    {
		renderer.sharedMaterial.SetTextureOffset("_MainTex", Vector2.one);
		size = new Vector2(1f / columns, 1f / rows);
		
		if (pauseAtStartTime != 0.0f)
		{
			isOnPauseStart = true;
		}
		
		if (fadeIn)
		{
			isOnFadeIn = true;	
			isOnPauseStart = false;
			startTime = Time.time;
		}
		
		isPlaying = true;
		
        StartCoroutine(updateTiling());
    }
 
    private IEnumerator updateTiling()
    {
        while (true)
        {
			if (isOnFadeIn)
			{	
				float percentage = ( Time.time - startTime ) / ( (startTime + fadeInTime) - startTime );
				fadeAlpha = Mathf.Lerp(0.0f, 1.0f, percentage);
				
				float timeSinceStart = Time.time - startTime;
				if (timeSinceStart > fadeInTime)
				{
					startTime = Time.time;
					isOnFadeIn = false;
					isOnPauseStart = true;	
				}
			}
			
			if (isOnPauseStart)
			{
				float timeSinceStart = Time.time - startTime;
				if (timeSinceStart > pauseAtStartTime)	
					isOnPauseStart = false;
			}
			
            //move to the next index
            if (isPlaying && !isOnFadeIn && !isOnPauseStart)
				index++;
			
            if (index >= rows * columns && isPlaying)
			{
				if (loop)
                	index = 0;
				else 
				{
					isPlaying = false;
					index--;
				}
			}
			
			float yOffset = (index % rows) * size.y;
			
			float xOffset = 0;
			int counter = 0;
			while (counter <= (index - rows))
			{
				counter += rows;
				xOffset++;
			}
			
			xOffset *= size.x;
 			
            //split into x and y indexes
            Vector2 offset = new Vector2(xOffset, 1 - yOffset);
			
            renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
			
			Color fadeColor = new Color(fadeAlpha, fadeAlpha, fadeAlpha, fadeAlpha);
			renderer.sharedMaterial.SetColor("_Color", fadeColor);
			
            yield return new WaitForSeconds(1f / framesPerSecond);
        }
 
    }
}