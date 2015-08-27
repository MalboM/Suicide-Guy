//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;


public enum GroundType
{
    DEFAULT,
    ROCK,
    SAND,
	GRASS,
	ASPHALT,
	CONCRETE
}

/// <summary>
/// Checks whether we need to play different sound effects when stepping on different grounds
/// </summary>
public class GroundStepsController : MonoBehaviour {

    public RigidbodyFirstPersonController playerController;
    public AudioSource audioSource;
    float checkDistance = 10f;

    public float minimumDelay = 0.5f;

    // Clips
    private int MAX_CLIPS = 4; // Make sure all the clips have 4 versions at max!

    public AudioClip[] defaultClips;
    public AudioClip[] rockClips;
    public AudioClip[] sandClips;
	public AudioClip[] grassClips;
	public AudioClip[] asphaltClips;
	public AudioClip[] concreteClips;

    private AudioClip[] chosenClips;
    private int currentClipIndex = 0;

    void Start()
    {
        SetGroundType(GroundType.DEFAULT);
        StartCoroutine(CheckPlaySoundsCO());
    }

    void FixedUpdate()
    {
        if (playerController.Grounded)
        {
            RaycastHit hitInfo;
            LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
            if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, checkDistance, layerMask))
            {
                var groundState = hitInfo.collider.gameObject.GetComponent<GroundState>();
                if (groundState != null)
                {
                    SetGroundType(groundState.groundType);
                }
                else
                {
                    // Default: ROCKY
                    SetGroundType(GroundType.DEFAULT);
                }
            }
        }
	}

    /// <summary>
    /// Set the ground we are walking on.
    /// </summary>
    /// <param name="groundType">Type of the ground</param>
    void SetGroundType(GroundType groundType)
    {
        //Debug.Log("SET TYPE " + groundType);
        switch (groundType)
        {
            case GroundType.DEFAULT:
                chosenClips = defaultClips;
                break;

            case GroundType.ROCK:
                chosenClips = rockClips;
                break;

            case GroundType.SAND:
                chosenClips = sandClips;
                break;
			
			case GroundType.GRASS:
				chosenClips = grassClips;
				break;

			case GroundType.ASPHALT:
				chosenClips = asphaltClips;
				break;

			case GroundType.CONCRETE:
				chosenClips = concreteClips;
				break;
        }

    }

    /// <summary>
    /// Coroutine that checks what step sound to play at runtime
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckPlaySoundsCO()
    {
        while (true)
        {
            if (!playerController.Grounded || playerController.Jumping){
                yield return null;
                continue;
            }
            Vector3 velocity = playerController.rigidbody.velocity;
            float velocitySqMagnitude = velocity.sqrMagnitude;

            if (velocitySqMagnitude > 1f)
            {
                audioSource.clip = chosenClips[currentClipIndex];
                currentClipIndex++;
                if (currentClipIndex >= MAX_CLIPS) currentClipIndex = 0;

                audioSource.Play();
                float delay = Mathf.Max(minimumDelay, 1f / velocitySqMagnitude);
                yield return new WaitForSeconds(delay);
            }
            else 
            {
                yield return null;
            }
        }
    }
}
