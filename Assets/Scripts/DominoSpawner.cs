using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.InputSystem;

public class DominoSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject dominosParent;
    [SerializeField] private PathDrawer pathDrawer;

    [Header("Shadow")]
    [SerializeField] private BoxCollider shadowCollider;
    [SerializeField] private BoxCollider longShadowCollider;

    [Header("Dominos")]
    [SerializeField] private List<GameObject> dominoPrefabs;
    [SerializeField] private List<GameObject> longDominoPrefabs;
    [SerializeField] private List<int> dominosCounts;
    [SerializeField] private List<TextMeshProUGUI> dominosCountsTexts;
    [SerializeField] private Color disabledColor;

    [Header("Parameters")]
    [SerializeField] float speed = 0.3f;
    [SerializeField] private float minTimeBetweenDominos = 0.1f;
    [SerializeField] private Vector3 spawnOffset = new (0, 0.3f, 0);
    [SerializeField] private Material shadowRedMaterial;


    private int dominoIndex = 0;
    private float lastPosition = -1;
    private float timeSinceLastDomino = Mathf.Infinity;
    private int dominosRemaining = 0;
    private bool dollyCartStarted = false;

    Controls controls;
    private List<float> distances;
    private List<int> colors;

    private CinemachineDollyCart dollyCart;
    private MeshRenderer shadowMeshRenderer;
    private MeshRenderer longShadowMeshRenderer;
    private Material shadowMaterial;

    private void Awake()
    {
        for (int i = 0; i < dominosCounts.Count; i++) dominosRemaining += dominosCounts[i];
        
        dollyCart = GetComponent<CinemachineDollyCart>();
        shadowMeshRenderer = shadowCollider.GetComponent<MeshRenderer>();
        longShadowMeshRenderer = longShadowCollider.GetComponent<MeshRenderer>();
        
        shadowMaterial = shadowMeshRenderer.material;
    }

    private void OnEnable()
    {
        distances = new List<float>();
        colors = new List<int>();

        if (dominoPrefabs.Count != dominosCounts.Count)
        {
            Debug.LogWarning("La liste des nombres de dominos n'a pas la même taille que le nombre de prefabs de dominos");
        }
        for (int i = 0; i < Mathf.Min(dominosCountsTexts.Count, dominosCounts.Count); i++)
        {
            dominosCountsTexts[i].text = dominosCounts[i].ToString();
            if (dominosCounts[i] == 0) dominosCountsTexts[i].color = disabledColor;
        }

        controls = new Controls();
        controls.Player.Enable();
        controls.Player.PlaceColor1.performed += Spawn0;
        controls.Player.PlaceColor2.performed += Spawn1;
        controls.Player.PlaceColor3.performed += Spawn2;
        controls.Player.PlaceColor4.performed += Spawn3;
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        controls.Player.PlaceColor1.performed -= Spawn0;
        controls.Player.PlaceColor2.performed -= Spawn1;
        controls.Player.PlaceColor3.performed -= Spawn2;
        controls.Player.PlaceColor4.performed -= Spawn3;
    }

    private void Update()
    {
        if (Time.timeScale < 1) return;
        if (dollyCartStarted && (lastPosition == dollyCart.m_Position)) PlacingPhaseFinished();
        
        timeSinceLastDomino += Time.deltaTime;
        lastPosition = dollyCart.m_Position;

        bool longKeyPressed = controls.Player.LongDomino.ReadValue<float>() > 0;
        
        shadowCollider.gameObject.SetActive(!longKeyPressed);
        longShadowCollider.gameObject.SetActive(longKeyPressed);
    }

    private void FixedUpdate()
    {
        bool spaceAvailable = SpaceAvailable();

        shadowMeshRenderer.material = spaceAvailable ? shadowMaterial : shadowRedMaterial;
        longShadowMeshRenderer.material = spaceAvailable ? shadowMaterial : shadowRedMaterial;
    }


    private void Spawn0(InputAction.CallbackContext ctx) => Spawn(0);
    private void Spawn1(InputAction.CallbackContext ctx) => Spawn(1);
    private void Spawn2(InputAction.CallbackContext ctx) => Spawn(2);
    private void Spawn3(InputAction.CallbackContext ctx) => Spawn(3);

    public void Spawn(int prefabIndex)
    {
        if (!SpaceAvailable() || Time.timeScale < 1) return;
        if (dominosCounts.Count <= prefabIndex || dominosCounts[prefabIndex] <= 0) return;

        if (!dollyCartStarted)
        {
            dollyCartStarted = true;
            dollyCart.m_Speed = speed;
        }

        if (timeSinceLastDomino > minTimeBetweenDominos)
        {
            bool longDomino = (controls.Player.LongDomino.ReadValue<float>() > 0 && dominosCounts[prefabIndex] >= 2);
            var dominoInstance = Instantiate(longDomino ? longDominoPrefabs[prefabIndex] : dominoPrefabs[prefabIndex], spawnPoint.position + spawnOffset, spawnPoint.rotation, dominosParent.transform);
            
            timeSinceLastDomino = 0;
            dominosCounts[prefabIndex] -= longDomino ? 2 : 1;
            dominosRemaining -= longDomino ? 2 : 1;

            distances.Add(dollyCart.m_Position);
            colors.Add(prefabIndex);
            
            if (dominosCountsTexts.Count > prefabIndex)
            {
                dominosCountsTexts[prefabIndex].text = dominosCounts[prefabIndex].ToString();
                if (dominosCounts[prefabIndex] == 0) dominosCountsTexts[prefabIndex].color = disabledColor;
            }

            dominoInstance.GetComponent<Domino>().Init(dominoIndex, dollyCart.m_Position, dollyCart.m_Position / dollyCart.m_Path.PathLength, pathDrawer);
            dominoIndex++;
        }
        
        if (dominosRemaining <= 0) PlacingPhaseFinished();
    }


    private bool SpaceAvailable()
    {
        Collider[] hitColliders;
        BoxCollider collider;

        if (controls.Player.LongDomino.ReadValue<float>() > 0) collider = longShadowCollider;
        else collider = shadowCollider;

        hitColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, transform.rotation);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject != collider.gameObject &&
                !hitColliders[i].gameObject.CompareTag("IgnoreSound"))
                return false;
        }
        
        return true;
    }
  

    private void PlacingPhaseFinished()
    {
        GameManager.Instance.SwitchToShowdownPhase();
        Destroy(gameObject);
    }
}
