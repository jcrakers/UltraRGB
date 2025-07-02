using UnityEngine;

namespace UltrakillArtemisMod.Components;

public class PlayerCheck : MonoBehaviour
{
    public int health = 100;

    public int hardDamage = 0;

    public bool dead = false;


    private NewMovement nm;
    private GameObject player;

    private void Start()
    {
        nm = MonoSingleton<NewMovement>.Instance;
        player = nm.gameObject;
    }
}