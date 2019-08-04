using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteAfterImage : MonoBehaviour {
    [Tooltip("The color each after-image will fade to over its lifetime. Alpha of 0 is recommended")]
    public Color finalColor = Color.clear;
    [Tooltip("The amount of time an after-image will take to fade away.")]
    public float trailLifetime = .25f;
    [Tooltip("The distance this object must move to spawn one after-image.")]
    public float distancePerSpawn = .1f;
    [Tooltip("Optimization - number of after-images to create before the effect starts, to reduce the start-up load.")]
    public int spawnOnStart = 0;
    private SpriteRenderer mainSpriteRenderer;    // the sprite renderer to trail after
    private List<SpriteRenderer> readyObjects;    // the list of objects ready to be shown
    private float distanceTraveledSinceLastSpawn; // the distance this object has moved since the last object was shown
    private Vector3 lastSpawnPosition;            // the position the last object was spawned
    private Color initialColor;
    private List<GameObject> allObjects = new List<GameObject>();        

    private void Awake() {
        // get the sprite renderer on this object
        mainSpriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = mainSpriteRenderer.color;
        // initialize the empty list
        readyObjects = new List<SpriteRenderer>();
        // optionally populate list beforehand with objects to use
        for (int i = 0; i < spawnOnStart; i++) {
            readyObjects.Add(makeSpriteObject());
        }
        allObjects = new List<GameObject>();
    }

    private void OnEnable() {
        StartCoroutine(trailCoroutine());
    }

    // function to create a sprite gameobject ready for use
    private SpriteRenderer makeSpriteObject() {
        // create a gameobject named "TrailSprite" with a SpriteRenderer component
        GameObject spriteObject = new GameObject("TrailSprite", typeof(SpriteRenderer));
        // parent the object to this object so that it follows it
        spriteObject.transform.SetParent(transform);
        // center it on this object
        spriteObject.transform.localPosition = Vector3.zero;
        // hide it
        spriteObject.SetActive(false);
        allObjects.Add(spriteObject);
        return spriteObject.GetComponent<SpriteRenderer>();
    }

    private IEnumerator trailCoroutine() {
        // keep running while this component is enabled
        while (enabled) {
            // get the distance between the current position and the last position
            // a trail object was spawned
            distanceTraveledSinceLastSpawn = Vector2.Distance(lastSpawnPosition, transform.position);
            // if that distance is greater than the specified distance per spawn
            if (distanceTraveledSinceLastSpawn > distancePerSpawn) {
                // if there aren't any objects ready to show, spawn a new one
                if (readyObjects.Count == 0) {
                    // add that object's sprite renderer to the trail list
                    readyObjects.Add(makeSpriteObject());
                }
                // get the next object in the ready list
                SpriteRenderer nextObject = readyObjects[0];
                // set this trailSprite to reflect the current player sprite
                nextObject.sprite = mainSpriteRenderer.sprite;
                //flip sprite if necessary
                nextObject.flipX = mainSpriteRenderer.flipX;
                // this makes it so that the trail will render behind the main sprite
                nextObject.sortingLayerID = mainSpriteRenderer.sortingLayerID;
                nextObject.sortingOrder = mainSpriteRenderer.sortingOrder - 1;
                // set it loose in the world
                nextObject.transform.SetParent(null, true);
                // match the copy's scale to the sprite's world-space scale
                nextObject.transform.localScale = mainSpriteRenderer.transform.lossyScale;
                // show it
                nextObject.gameObject.SetActive(true);
                // start it fading out over time
                StartCoroutine(fadeOut(nextObject));
                // remove it from the list of ready objects
                readyObjects.Remove(nextObject);
                // save this position as the last spawned position
                lastSpawnPosition = transform.position;
                // reset the distance traveled
                distanceTraveledSinceLastSpawn = 0;
            }
            // wait until next frame to continue the loop
            yield return null;
        }
        // reduce number of sprites back to original pool size
        /*foreach (SpriteRenderer sprite in this.readyObjects) {
            if (this.readyObjects.Count > spawnOnStart) {
                Destroy(sprite.gameObject);
            } else {
                resetObject(sprite);
            }
        }*/
    }

    private IEnumerator fadeOut(SpriteRenderer sprite) {
        float timeElapsed = 0;
        // while the elapsed time is less than the specified trailLifetime
        while (timeElapsed < trailLifetime) {
            // get a number between 0 and 1 that represents how much time has passed
            // 0 = no time has passed, 1 = trailLifetime seconds has passed
            float progress = Mathf.Clamp01(timeElapsed / trailLifetime);
            // linearly interpolates between the initial color and the final color
            // based on the value of progress (0 to 1)
            sprite.color = Color.Lerp(initialColor, finalColor, progress);
            // track the time passed
            timeElapsed += Time.deltaTime;
            // wait until next frame to continue the loop
            yield return null;
        }
        // reset the object so that it can be reused
        resetObject(sprite);
    }

    // resets the object so that it is ready to use again
    private void resetObject(SpriteRenderer sprite) {
        // hide the sprite
        sprite.gameObject.SetActive(false);
        // reset the tint to default
        sprite.color = initialColor;
        // parent it to this object
        sprite.transform.SetParent(transform);
        // center it on this object
        sprite.transform.localPosition = Vector3.zero;
        // add it to the ready list
        readyObjects.Add(sprite);
    }

    private void OnDestroy()
    {
        foreach (var obj in allObjects)
        {
            Destroy(obj);
        }
    }
}