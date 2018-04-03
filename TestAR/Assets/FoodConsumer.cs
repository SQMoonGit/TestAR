using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class FoodConsumer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "food")
        {
            collision.gameObject.SetActive(false);
            Slithering s = GetComponentInParent<Slithering>();

            if (s != null)
                s.AddBodyPart();
        }
    }
}
