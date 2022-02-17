using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    [SerializeField] PortalScript _otherPortal = default;
    [SerializeField] Renderer _outlineRenderer = default;
    [SerializeField] Color _portalColor = default;
    [SerializeField] LayerMask _placementMask = default;
    [SerializeField] Transform _judgmentPos = default;
    [SerializeField] float _delay = 0.001f;
    [SerializeField] ParticleSystem _particle = default;
    [SerializeField] Animator _setAnim = default;

    bool isPlaced;
    Collider _wallCollider = default;
    Renderer _renderer = default;
    Material _material = default;
    Collider _collider = default;
    List<PortalableObject> _portalObjects = new List<PortalableObject>();

    public PortalScript OtherPortal { get => _otherPortal; }
    public Color PortalColor { get => _portalColor;}
    public Renderer Renderer { get => _renderer;}

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
        for(int i = 0; i < _portalObjects.Count; i++)
        {
            //ポータル内に入っているオブジェクトの座標をポータルのローカル座標に変更
            Vector3 obj = transform.InverseTransformPoint(_portalObjects[i].transform.position);

            if(obj.z > 0.0f)
            {
                _portalObjects[i].Warp();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (OtherPortal.IsPlaced())
        {
            var obj = other.GetComponent<PortalableObject>();

            if (obj != null)
            {
                _portalObjects.Add(obj);
                obj.EnterPortal(this, OtherPortal, _wallCollider);
            }
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

    /// <summary>カメラに写っているかをboolで返す</summary>
    /// <returns></returns>
    public bool IsRendererVisible()
    {
        return _renderer.isVisible;
    }
    /// <summary>ポータルを配置する</summary>
    /// <param name="wallCollider"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
    public bool PlacePortal(Collider wallCollider, Vector3 pos, Quaternion rot)
    {
        _judgmentPos.position = pos;
        _judgmentPos.rotation = rot;
        _judgmentPos.position -= _judgmentPos.forward * _delay;

        if(CheckOverlap())
        {
            _wallCollider = wallCollider;
            transform.position = _judgmentPos.position;
            transform.rotation = _judgmentPos.rotation;

            gameObject.SetActive(true);
            _setAnim.Play("SetPortalAnim");
            isPlaced = true;
            return true;
        }

        //Portalを置けない時にパーティクルを出す
        _particle.transform.position = _judgmentPos.position;
        _particle.Play();
        return false;
    }
    /// <summary>ポータルが重なっているかを判断</summary>
    /// <returns></returns>
    bool CheckOverlap()
    {
        //ここの辺りの数値はポータル用のメッシュの大きさを考えている
        //チェックする範囲
        var checkExtents = new Vector3(0.9f, 1.9f, 0.05f);

        var checkPositions = new Vector3[]
        {
            //transform.TransformVector()
            //()内のベクトルをローカルからワールド系に変換させる
            //中心奥
            _judgmentPos.position + _judgmentPos.TransformVector(new Vector3( 0.0f,  0.0f, -0.1f)),
            //左下
            _judgmentPos.position + _judgmentPos.TransformVector(new Vector3(-1.0f, -2.0f, -0.1f)),
            //左上
            _judgmentPos.position + _judgmentPos.TransformVector(new Vector3(-1.0f,  2.0f, -0.1f)),
            //右下
            _judgmentPos.position + _judgmentPos.TransformVector(new Vector3( 1.0f, -2.0f, -0.1f)),
            //右上
            _judgmentPos.position + _judgmentPos.TransformVector(new Vector3( 1.0f,  2.0f, -0.1f)),
            //中央手前
            _judgmentPos.TransformVector(new Vector3(0.0f, 0.0f, 0.2f))
        };

        // ポータルが壁と交差しないようする
        var intersections = Physics.OverlapBox(checkPositions[0], checkExtents, _judgmentPos.rotation, _placementMask);

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

        for (int i = 1; i < checkPositions.Length - 1; i++)
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
