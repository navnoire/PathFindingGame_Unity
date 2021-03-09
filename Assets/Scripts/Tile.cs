using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public static event Action<Vector2Int> OnTileRotated;

    public int prefabIndex;
    public TileType tileType;
    public bool[] values;
    public Vector2Int Coords;
    public float rotationSpeed;

    public bool isEmissive;
    public bool isOpen;
    public bool isUntouchable;
    public bool isRotatable;
    public float realRotation;

    public Vector2Int textureSize = new Vector2Int(2048, 512);
    private List<Vector2> textureCoords = new List<Vector2>();
    private MeshRenderer rend;
    private Material emissiveMat;
    private Material regularMat;

    [HideInInspector] public Mesh _mesh;


    [HideInInspector] public HashSet<Vector2Int> connectedWith = new HashSet<Vector2Int>();
    public bool isRotating;


    public void AssignMaterials(Material ordinary, Material emissive)
    {
        regularMat = ordinary;
        emissiveMat = emissive;
    }

    public void SetEmission(bool value)
    {
        isEmissive = value;

        if (rend == null)
        {
            rend = GetComponent<MeshRenderer>();
        }

        if (isEmissive)
        {
            rend.sharedMaterial = emissiveMat;
        }
        else
        {
            rend.sharedMaterial = regularMat;
        }
    }

    public void InitializeSingleUVRect(TileCoordsPack.TextureRect rect)
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _mesh.GetUVs(0, textureCoords);

        textureCoords[0] = ConvertTexturePixelsToUVCoordinates(new Vector2(rect.ld.x, rect.ld.y));
        textureCoords[1] = ConvertTexturePixelsToUVCoordinates(new Vector2(rect.rd.x, rect.rd.y));
        textureCoords[2] = ConvertTexturePixelsToUVCoordinates(new Vector2(rect.lt.x, rect.lt.y));
        textureCoords[3] = ConvertTexturePixelsToUVCoordinates(new Vector2(rect.rt.x, rect.rt.y));

        _mesh.SetUVs(0, textureCoords);
    }

    IEnumerator Rotate()
    {
        yield return new WaitWhile(() => isRotating);
        isRotating = true;
        transform.localPosition += new Vector3(0, .01f, 0);

        while (Math.Abs(gameObject.transform.rotation.eulerAngles.y - realRotation) > .05f)
        {
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(90, realRotation, 0), rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.localPosition -= new Vector3(0, .01f, 0);
        transform.localRotation = Quaternion.Euler(90, Mathf.Round(transform.localRotation.eulerAngles.y), 0);
        isRotating = false;

    }

    public void RotateTile()
    {
        realRotation += 90;

        if (realRotation >= 360 || realRotation <= -360)
        {
            realRotation = 0;
        }

        StartCoroutine(Rotate());
        rotateValues();

        OnTileRotated?.Invoke(Coords);

    }

    void rotateValues()
    {
        bool aux = values[0];

        for (int i = 0; i < values.Length - 1; i++)
        {
            values[i] = values[i + 1];
        }
        values[3] = aux;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isRotatable)
        {
            RotateTile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isRotatable = false;
            isOpen = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isUntouchable && other.tag == "Player")
        {
            isRotatable = true;
            isOpen = true;
        }
    }

    private void OnDisable()
    {
        if (Rotate() != null)
        {
            isRotating = false;

        }
    }

    private Vector2 ConvertTexturePixelsToUVCoordinates(Vector2 _textureCoords)
    {
        return new Vector2((float)_textureCoords.x / textureSize.x, (float)_textureCoords.y / textureSize.y);

    }
}


