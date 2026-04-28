using UnityEngine;

public class Fruit : MonoBehaviour
{

    public int fruitType;

    public bool hasMerged = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(hasMerged)
            return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();
        
        if (otherFruit != null && !otherFruit.hasMerged && otherFruit.fruitType == fruitType)
        {
            hasMerged = true;
            otherFruit.hasMerged = true;

            Vector3 mergePosition = (transform.position + otherFruit.transform.position) / 2f;

            FruitGame gameManager = FindAnyObjectByType<FruitGame>();
            if (gameManager != null)
            {
                gameManager.MergeFruits(fruitType,mergePosition);
            }

            Destroy(otherFruit.gameObject);
            Destroy(gameObject);
        }
    }
}
