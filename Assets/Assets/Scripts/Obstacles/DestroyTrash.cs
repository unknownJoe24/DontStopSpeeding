using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTrash : MonoBehaviour
{
    [SerializeField] float delay;

    //These variables were added in the ToDoListSarah Branch
    [SerializeField] AudioClip destroySound;
    [SerializeField] float volume;
    [SerializeField] GameObject explosionParticles;

    private void Start()
    {
        Destroy(this.gameObject, 30);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            if(destroySound != null)
                SoundManager.Instance.Play(destroySound, volume); //this was added in the ToDoListSarah Brancch
            if(explosionParticles != null)
                explosionParticles.gameObject.SetActive(true); //this was added in the ToDoListSarah Brancch
            Destroy(this.gameObject, delay);
        }
    }
}
