using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGhost : MonoBehaviour
{
    [SerializeField] private GameObject visual;
    private GridBuilding grid;  
    // Start is called before the first frame update
    private void Start()
    {
        grid = FindObjectOfType<GridBuilding>();
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 target = grid.GetSnappedMousePos();
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 15f) ;
    }

    public void ChangeGhost(PlaceableLevels type)
    {
        if(type!=null)
        {
            //Overriding pre-existing visual with a new one
            this.visual = Instantiate(type.prefab, transform);

            //Making the visual transparent
            foreach(SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
            }
            //Removing colliders
            foreach (BoxCollider2D collider in GetComponentsInChildren<BoxCollider2D>())
            {
                collider.enabled = false;
            }
        }
    }

    public void RemoveGhost()
    {
        if(visual != null)
        {
            Destroy(visual);
            visual = null;
        }
    }

}
