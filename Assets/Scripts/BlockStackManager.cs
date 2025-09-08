using System.Collections.Generic;
using UnityEngine;
using SwipeSort;
using System.Collections;
using Assets.Scripts;
using System;
using UnityEngine.Pool;

// 도형 스택을 관리하는 핵심 매니저
public class BlockStackManager : MonoBehaviour
{

    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private GameObject trianglePrefab;
    [SerializeField] private GameObject poolGroup;
    private GameObject squareParent;
    private GameObject triangleParent;
    private GameObject circleParent;

    ObjectPool<Block> blockPool;
    public int stackCount;

    public Queue<Block> currentBlocks = new Queue<Block>();

    private SwipeInputManager swipeInputManager;

    [SerializeField] Transform blockSpawnPos;
    public int ScoreValue { get; set; }

    private int blockColorCount = Enum.GetValues(typeof(BlockColor)).Length;
    private int shapeTypeCount = Enum.GetValues(typeof(ShapeType)).Length;
    private void Awake()
    {

    }

    private void Start()
    {
        squareParent = new GameObject("SquareParent");
        triangleParent = new GameObject("TriangleParent");
        circleParent = new GameObject("CircleParent");
        squareParent.transform.parent = poolGroup.transform;
        triangleParent.transform.parent = poolGroup.transform;
        circleParent.transform.parent = poolGroup.transform;
        Init();

    }


    private void Init()
    {
        blockPool = new ObjectPool<Block>(() =>
        {
            GameObject go = CreateBlock(GetRandomBlockData());
            return go.GetComponent<Block>();
        }, actionOnGet: OnGetFromPool,
        actionOnRelease: OnReleaseToPool,
        actionOnDestroy: OnDestroyPoolObject,
        collectionCheck: false,
        defaultCapacity: 10,
        maxSize: 20);
        blockPool.Get();
    }

    #region Object Pool Callbacks
    private void OnDestroyPoolObject(Block block)
    {
        Destroy(block.gameObject);
    }

    private void OnReleaseToPool(Block block)
    {
        block.gameObject.SetActive(false);
    }

    /// <summary>
    /// <see cref="BlockData"/>를 재생성하여 블록을 초기화하고 스폰 위치에 배치
    /// </summary>
    /// <remarks>Enqueue / Postion / SetActive</remarks>
    /// <param name="block"></param>
    private void OnGetFromPool(Block block)
    {
        var data = GetRandomBlockData(); // 랜덤 데이터 생성
        block.SetupBlock(data);
        block.transform.position = blockSpawnPos.position;
        currentBlocks.Enqueue(block);
        block.gameObject.SetActive(true);
    }
    #endregion

    float timer = 0f;
    public void Update()
    {
        foreach (Block b in currentBlocks)
        {
            b.transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime);
        }

        //
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer = 0f;
            var data = GetRandomBlockData();
            //CreateBlock(data);
            blockPool.Get();
        }

    }
    /// <summary>
    /// 맨앞 빼내기
    /// </summary>
    /// <param name="data"></param>
    public Block HandleSwipeDetected(SwipeData data)
    {
        if (currentBlocks.Count > 0)
        {
            Block topBlock = currentBlocks.Dequeue();

            topBlock.Move(data.direction);
            //blockPool.Release(topBlock);
            Allign();
            return topBlock;
        }
        throw new IndexOutOfRangeException();
    }


    private void Allign()
    {


    }

    private BlockData GetRandomBlockData()
    {
        BlockColor c = (BlockColor)UnityEngine.Random.Range(0, blockColorCount);
        ShapeType s = (ShapeType)UnityEngine.Random.Range(0, shapeTypeCount);

        BlockData result = new BlockData(s, c, BlockSize.Small, 0);

        return result;
    }
    private GameObject CreateBlock(BlockData data)
    {
        GameObject go;

        switch (data.shape)
        {
            case ShapeType.Circle:
                {
                    go = Instantiate(circlePrefab, circleParent.transform);
                    break;
                }
            case ShapeType.Square:
                {
                    go = Instantiate(squarePrefab, squareParent.transform);
                    break;
                }
            case ShapeType.Triangle:
                {
                    go = Instantiate(trianglePrefab, triangleParent.transform);
                    break;
                }
            default:
                {
                    throw new System.Exception($"해당하는 모양이 없습니다. {data.shape.ToString()}");
                }
        }
        Block block = go.GetComponent<Block>();
        block.onBlockMoveEnd.AddListener(() =>
        {
            blockPool.Release(block);
        });
        go.transform.position = blockSpawnPos.position;
        block.SetupBlock(data);
        return go;
    }

}