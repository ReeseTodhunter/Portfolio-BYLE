using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D defaultSprite;
    public static CursorController instance;
    private Texture2D currTexture;
    public void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;
        SetCursor(defaultSprite);
    }
    public void SetCursor(Texture2D tx)
    {
        if(tx == currTexture){return;}
        if(tx == null){tx = defaultSprite;}
        currTexture = tx;
        Cursor.SetCursor(tx,Vector2.one * 64f, CursorMode.ForceSoftware);
    }
}
