using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public RoomTemplateSO roomTemplate;
    private List<SpawnableObjectsByLevel<EnemyDetailsSO>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private GameObject instantiatedEnemy;

    private void Start()
    {
        testLevelSpawnList = roomTemplate.enemiesByLevelList;

        randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)){

            if (instantiatedEnemy!= null)
            {
                Destroy(instantiatedEnemy);
            }

            EnemyDetailsSO enemyDetails = randomEnemyHelperClass.GetItem();
        
            if(enemyDetails != null)
            {
                instantiatedEnemy = Instantiate(enemyDetails.enemyPrefab, HelperUtilitie.GetSpawnPositionNearestToPlayer
                    (HelperUtilitie.GetMouseWorldPosition()), Quaternion.identity);
            }
        }
    }
}
