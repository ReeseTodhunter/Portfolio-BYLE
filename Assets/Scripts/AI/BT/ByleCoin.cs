using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ByleCoin : MonoBehaviour
{
    public float flightSpeed = 30;
    public float rotateSpeed = 90;
    public float bobSpeed = 1;
    public float bobHeight = 0.5f;
    public Transform coinModel;
    public Vector3 startPos = Vector3.up * 0.5f;
    //public Image shadowImage;
    public float shadowMaxHeight = 1;
    private static bool flyToPlayer = false;
    public static void setCoinsToFly(bool canFly)
    {
        flyToPlayer = canFly;
    }
    void Start()
    {
        Physics.IgnoreLayerCollision(10, 14);
        Physics.IgnoreLayerCollision(9, 14);
    }
    public void Update()
    {
        if (flyToPlayer)
        {
            Vector3 velocity = PlayerController.instance.transform.position - transform.position;
            //if()
            //this.GetComponent<Rigidbody>().detectCollisions = false;
            //this.GetComponent<Rigidbody>().useGravity = false;
            velocity.Normalize();
            transform.position += velocity * flightSpeed * Time.deltaTime;
        }
        //shadowImage.gameObject.SetActive(transform.position.y > shadowMaxHeight ? false : true);
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        coinModel.position = transform.position + (startPos + Vector3.up * offset);
        coinModel.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == 7)
        //Debug.Log("Entered objects");
        if (other.gameObject.layer == 7 && !other.isTrigger)
        {
            PlayerController.instance.ChangeCoinValue(1);
            Debug.Log("Entered Player");

            // heal player if they have the powerup
            if (PlayerController.instance.GetModifier(ModifierType.CoinHeal) > 0)
            {
                PlayerController.instance.Heal(PlayerController.instance.GetModifier(ModifierType.CoinHeal));
            }
            Destroy(this.gameObject);
        }
    }
}
