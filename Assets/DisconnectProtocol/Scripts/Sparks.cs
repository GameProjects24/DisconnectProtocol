using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparks : MonoBehaviour
{
    public class SparkParticleObject
    {
        public GameObject gameObject;
        public ParticleSystem particleSystem;
    }

    public class SparkParticlePool
    {
        private List<SparkParticleObject> pool;
        private GameObject sparkPrefab; // Префаб системы частиц

        public SparkParticlePool(GameObject prefab, int poolSize)
        {
            pool = new List<SparkParticleObject>();
            sparkPrefab = prefab;

            for (int i = 0; i < poolSize; i++)
            {
                CreateSparkParticle();
            }
        }

        private SparkParticleObject CreateSparkParticle()
        {
            GameObject sparkObject = Instantiate(sparkPrefab);
            sparkObject.SetActive(false);

            SparkParticleObject sparkParticle = new SparkParticleObject
            {
                gameObject = sparkObject,
                particleSystem = sparkObject.GetComponent<ParticleSystem>(),
            };

            pool.Add(sparkParticle);
            return sparkParticle;
        }

        public SparkParticleObject RentParticle()
        {
            SparkParticleObject sparkParticle = pool.Find(p => !p.gameObject.activeSelf);

            if (sparkParticle == null)
            {
                sparkParticle = CreateSparkParticle();
            }

            sparkParticle.gameObject.SetActive(true);
            return sparkParticle;
        }

        public void ReturnParticle(SparkParticleObject sparkParticle)
        {
            sparkParticle.gameObject.SetActive(false);
        }
    }
}
