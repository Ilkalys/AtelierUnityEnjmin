using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZombieSpawn : MonoBehaviour
{
    public float timeBetweenWave;
    public GameObject Zombie;
    public Player player;

    public int firstWaveQuantity;
    public int waveIncrement;
    public int currentWave;

    public TextMeshProUGUI waveText;
    private float countDown;
    private GameObject[] grounds;

    // Start is called before the first frame update
    void Start()
    {
        currentWave = 0;
        countDown = timeBetweenWave;
        if(waveText)
        waveText.text = "Wave : " + currentWave;
    }

    // Update is called once per frame
    void Update()
    {
        if( countDown <= 0)
        {
            SpawnHorde();
            countDown = timeBetweenWave;
        }
        else
        {
            countDown -= Time.deltaTime;
        }

    }
    

    private void SpawnHorde()
    {
        for (int i = 0; i < firstWaveQuantity + (currentWave ) * waveIncrement; i++)
        {
            if (grounds == null || grounds.Length == 0)
            {
                grounds = GameObject.FindGameObjectsWithTag("Ground");
            }
            if (grounds.Length > 0)
            {
                int rand = Random.Range(0, grounds.Length);
                GameObject target = grounds[rand];
                GameObject zombie = GameObject.Instantiate(Zombie, target.transform.position + Vector3.up, target.transform.rotation);
                zombie.GetComponent<WorldObject>().player = player;
            }
        }
        currentWave++;
        if (waveText)
            waveText.text = "Wave : " + currentWave;
    }
}
