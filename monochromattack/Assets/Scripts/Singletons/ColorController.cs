using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum GameColor { White = -1, Red, Yellow, Blue, Orange, Green, Purple }

[System.Serializable]
public struct ColorPair
{
    public GameColor gameColor;
    public Color color;
}

public class ColorController : MonoBehaviour
{
    public static ColorController instance;

    public List<ColorPair> colors;
    public List<SpriteRenderer> controlledSprites;
    public List<Tilemap> controlledTilemaps;

    private GameColor currentColor = GameColor.White;
    private Dictionary<GameColor, Color> colorDictionary;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            Initialize();
        }
    }

    private void Initialize()
    {
        colorDictionary = new Dictionary<GameColor, Color>();
        foreach(var pair in colors)
        {
            colorDictionary.Add(pair.gameColor, pair.color);
        }
    }

    public void AddSprite(SpriteRenderer sprite)
    {
        controlledSprites.Add(sprite);
        ChangeColor(sprite);
    }

    public void RemoveSprite(SpriteRenderer sprite)
    {
        if(controlledSprites.Contains(sprite))
        {
            controlledSprites.Remove(sprite);
        }
    }

    public GameColor GetCurrentGameColor()
    {
        return currentColor;
    }

    public void NextColor()
    {
        var previousColor = currentColor;

        currentColor = (GameColor) (((int)currentColor + 1) % 6);
        ApplyColor();

        if(previousColor == GameColor.White)
        {
            EnemyController.instance.StartFight();
        }
    }

    private void ChangeColor(SpriteRenderer sprite)
    {
        sprite.color = colorDictionary[currentColor];
    }

    private void ApplyColor()
    {
        foreach(var sprite in controlledSprites)
        {
            sprite.color = colorDictionary[currentColor];
        }

        foreach(var tilemap in controlledTilemaps)
        {
            tilemap.color = colorDictionary[currentColor];
        }

        EnemyController.instance.ChangeColors();
    }
}
