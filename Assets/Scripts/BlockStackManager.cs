using System.Collections.Generic;
using UnityEngine;
using SwipeSort;
using System.Collections;
using Assets.Scripts;
using System;

// 도형 스택을 관리하는 핵심 매니저
public class BlockStackManager : MonoBehaviour
{

    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private GameObject circlePrefab;

    public int stackCount;

    public Queue<Block> currentBlocks = new Queue<Block>();

    private SwipeInputManager swipeInputManager;


    public int ScoreValue { get; set; }
    private void Awake()
    {

    }

    private void Start()
    {
        Init();
    }


    private void Init()
    {
        for (int i = 0; i < stackCount; i++)
        {
            BlockData data = GetRandomBlockData();
            GameObject go = CreateBlock(data);
            go.transform.position = new Vector3(0, 1, i * 0.5f);
            //go.transform.position = new Vector3(0, 1 + i * 0.2f, i * 0.5f);
        }
        //SwipeInputManager.instance.OnSwipeDetected += HandleSwipeDetected;

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
            Allign();
            return topBlock;
        }
        throw new IndexOutOfRangeException();
    }


    private void Allign()
    {
        foreach (Block b in currentBlocks)
        {
            Vector3 targetPos = new Vector3(0, 1, 0.5f * System.Array.IndexOf(currentBlocks.ToArray(), b));
            b.transform.position = targetPos;
        }
        var data = GetRandomBlockData();
        CreateBlock(data);
    }

    private BlockData GetRandomBlockData()
    {
        BlockColor c = (BlockColor)UnityEngine.Random.Range(0, 5);
        ShapeType s = (ShapeType)UnityEngine.Random.Range(0, 2);

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
                    go = Instantiate(circlePrefab);
                    break;
                }
            case ShapeType.Square:
                {
                    go = Instantiate(squarePrefab);
                    break;
                }
            default:
                {
                    throw new System.Exception("해당하는 모양이 없습니다.");
                }
        }
        Block block = go.GetComponent<Block>();
        block.SetupBlock(data);
        currentBlocks.Enqueue(block);
        return go;
    }

}