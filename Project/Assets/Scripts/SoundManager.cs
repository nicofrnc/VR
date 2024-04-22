using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource fallSound;

    private void OnCollisionEnter(Collision collision)
    {
        fallSound.Play();
    }
}
