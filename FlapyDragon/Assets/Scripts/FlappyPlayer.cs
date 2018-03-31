using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

[RequireComponent(typeof(Rigidbody))]
public class FlappyPlayer : MonoBehaviour
{
    public bool godMode;
    public float movingSpeed = 5;
    public float jumpForce = 10;
    public float additionalGravityForce = 5;
    public float rotationPercentage = 0.5f;
    public float assumeDeadBelowY = -7;
    public float assumeDeadAboveY = 6;

    public float waitDurationBeforeRestart = 3f;

    public AudioClip deadClip;
    AudioSource _audioSource;
    public AudioMixerGroup dieOutput;

    // prevent killing player many times
    bool dead = false;

    public Rigidbody body
    {
        get;
        private set;
    }

    public bool IsDead()
    {
        return state == State.DEAD;
    }

    bool inputAllowed;

    enum State
    {
        INTRO, MOVING, DEAD
    }

    State state;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        state = State.INTRO;
        inputAllowed = true;
    }

    void Update()
    {
        if (inputAllowed && state == State.MOVING)
        {
            // Input update has to execute during Update, while logic can run in FixedUpdate
            UpdateInput();
        }
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.INTRO:
                UpdateIntro();
                break;
            case State.MOVING:
                UpdateMoving();
                ApplyAdditionalGravity();
                break;
            case State.DEAD:
                ApplyAdditionalGravity();
                break;
        }
    }

    bool Tapped()
    {
        return Input.GetMouseButtonDown(0);
    }

    void UpdateIntro()
    {
        // Constant velocity until first tap
        body.velocity = Vector3.zero;
        if (Tapped())
        {
            state = State.MOVING;
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            StartCoroutine(FlappyObstaclesSpawner.instance.StartSpawningObstacles());
            GameObject.Find("IntroText").SetActive(false);
        }
    }

    void UpdateMoving()
    {
        // Physics controls only Y axis of velocity
        body.velocity = new Vector3(movingSpeed, body.velocity.y, 0);
        // Additional effect inspired by Flappy Bird - rotation based on velocity
        transform.forward = Vector3.Lerp(Vector3.right, body.velocity, rotationPercentage);

        if (transform.position.y < assumeDeadBelowY || transform.position.y > assumeDeadAboveY)
        {
            Die();
        }
    }

    void UpdateInput()
    {
        if (Tapped())
        {
            Jump();
        }
    }

    void Jump()
    {
        body.velocity = new Vector3(movingSpeed, 0, 0);
        body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        _audioSource.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (godMode) return;
        Die();
    }


    void Die()
    {
        if (dead) return;
        dead = true;
        inputAllowed = false;
        state = State.DEAD;
        body.constraints = RigidbodyConstraints.None;
        FlappyObstaclesSpawner.instance.StopSpawningObstacles();
        StartCoroutine(RestartAfterSeconds(waitDurationBeforeRestart));

        _audioSource.clip = deadClip;
        _audioSource.outputAudioMixerGroup = dieOutput;
        _audioSource.Play();

        GetComponent<Animator>().SetBool("Die", true);
    }

    void ApplyAdditionalGravity()
    {
        body.AddForce(Vector3.down * additionalGravityForce);
    }

    IEnumerator RestartAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(0); // load first scene
    }
}