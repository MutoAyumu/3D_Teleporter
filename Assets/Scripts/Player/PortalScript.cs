using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    [SerializeField] PortalScript _otherPortal = default;
    [SerializeField] Renderer _outlineRenderer = default;
    [SerializeField] Color _portalColor = default;
    [SerializeField] LayerMask _placementMask = default;
    [SerializeField] Transform _testTransform = default;
    [SerializeField] float _delay = 0.001f;

    bool isPlaced;
    Collider _wallCollider = default;
    Renderer _renderer = default;
    Material _material = default;
    Collider _collider = default;
    List<PortalableObject> _portalObjects = new List<PortalableObject>();

    public PortalScript OtherPortal { get => _otherPortal; set => _otherPortal = value; }
    public Color PortalColor { get => _portalColor; set => _portalColor = value; }
    public Renderer Renderer { get => _renderer; set => _renderer = value; }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
        _outlineRenderer.materials[0].color = PortalColor;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        for(int i = 0; i < _portalObjects.Count; ++i)
        {
            Vector3 obj = transform.InverseTransformPoint(_portalObjects[i].transform.position);

            if(obj.z > 0.0f)
            {
                _portalObjects[i].Warp();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();

        if(obj != null)
        {
            _portalObjects.Add(obj);
            obj.SetInPortal(this, OtherPortal, _wallCollider);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();

        if(_portalObjects.Contains(obj))
        {
            _portalObjects.Remove(obj);
            obj.ExitPortal(_wallCollider);
        }
    }
    public void SetTexture(RenderTexture tex)
    {
        _material.mainTexture = tex;
    }

    public bool IsRendererVisible()
    {
        return _renderer.isVisible;
    }
    public bool PlacePortal(Collider wallCollider, Vector3 pos, Quaternion rot)
    {
        _testTransform.position = pos;
        _testTransform.rotation = rot;
        _testTransform.position -= _testTransform.forward * _delay;

        FixOverhangs();
        FixIntersects();

        if(CheckOverlap())
        {
            _wallCollider = wallCollider;
            transform.position = _testTransform.position;
            transform.rotation = _testTransform.rotation;

            gameObject.SetActive(true);
            isPlaced = true;
            return true;
        }

        return false;
    }
    void FixOverhangs()
    {
        var testPoints = new List<Vector3>
        {
            new Vector3(-1.1f,  0.0f, 0.1f),
            new Vector3( 1.1f,  0.0f, 0.1f),
            new Vector3( 0.0f, -2.1f, 0.1f),
            new Vector3( 0.0f,  2.1f, 0.1f)
        };

        var testDirs = new List<Vector3>
        {
             Vector3.right,
            -Vector3.right,
             Vector3.up,
            -Vector3.up
        };

        for(int i = 0; i < 4; ++i)
        {
            RaycastHit hit;
            Vector3 rayPos = _testTransform.TransformPoint(testPoints[i]);
            Vector3 rayDir = _testTransform.TransformDirection(testDirs[i]);

            if(Physics.CheckSphere(rayPos, 0.05f, _placementMask))
            {
                break;
            }
            else if(Physics.Raycast(rayPos, rayDir, out hit, 2.1f, _placementMask))
            {
                var offset = hit.point - rayPos;
                _testTransform.Translate(offset, Space.World);
            }
        }
    }
    void FixIntersects()
    {
        var testDirs = new List<Vector3>
        {
             Vector3.right,
            -Vector3.right,
             Vector3.up,
            -Vector3.up
        };

        var testDists = new List<float> { 1.1f, 1.1f, 2.1f, 2.1f };

        for (int i = 0; i < 4; ++i)
        {
            RaycastHit hit;
            Vector3 rayPos = _testTransform.TransformPoint(0.0f, 0.0f, -0.1f);
            Vector3 rayDir = _testTransform.TransformDirection(testDirs[i]);

            if (Physics.Raycast(rayPos, rayDir, out hit, testDists[i], _placementMask))
            {
                var offset = (hit.point - rayPos);
                var newOffset = -rayDir * (testDists[i] - offset.magnitude);
                _testTransform.Translate(newOffset, Space.World);
            }
        }
    }
    bool CheckOverlap()
    {
        var checkExtents = new Vector3(0.9f, 1.9f, 0.05f);

        var checkPositions = new Vector3[]
        {
            _testTransform.position + _testTransform.TransformVector(new Vector3( 0.0f,  0.0f, -0.1f)),

            _testTransform.position + _testTransform.TransformVector(new Vector3(-1.0f, -2.0f, -0.1f)),
            _testTransform.position + _testTransform.TransformVector(new Vector3(-1.0f,  2.0f, -0.1f)),
            _testTransform.position + _testTransform.TransformVector(new Vector3( 1.0f, -2.0f, -0.1f)),
            _testTransform.position + _testTransform.TransformVector(new Vector3( 1.0f,  2.0f, -0.1f)),

            _testTransform.TransformVector(new Vector3(0.0f, 0.0f, 0.2f))
        };

        // ポータルが壁と交差しないようする
        var intersections = Physics.OverlapBox(checkPositions[0], checkExtents, _testTransform.rotation, _placementMask);

        if (intersections.Length > 1)
        {
            return false;
        }
        else if (intersections.Length == 1)
        {
            // 古いPortalと交差している時
            if (intersections[0] != _collider)
            {
                return false;
            }
        }

        // Portalの端が表面に重なるようにする
        bool isOverlapping = true;

        for (int i = 1; i < checkPositions.Length - 1; ++i)
        {
            isOverlapping &= Physics.Linecast(checkPositions[i],
                checkPositions[i] + checkPositions[checkPositions.Length - 1], _placementMask);
        }

        return isOverlapping;
    }
    public bool IsPlaced()
    {
        return isPlaced;
    }
}
