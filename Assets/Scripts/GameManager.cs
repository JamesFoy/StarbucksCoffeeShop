using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    #region Singleton Setup
    private static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    public List<GameObject> npcPrefabs;
    public Transform spawnPoint;
    public Transform orderPoint;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetDestination());
    }

    public void SpawnNewNpc()
    {
        GameObject npc = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Count)], spawnPoint.transform);
        npc.GetComponent<NavMeshAgent>().SetDestination(orderPoint.position);
    }

    IEnumerator SetDestination()
    {
        yield return new WaitForSeconds(3f);
        SpawnNewNpc();
    }

}
