using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SwipeSort
{
    // 도형의 기본 속성들 (단순화)
    public enum ShapeType
    {
        Circle,
        Square,
        Triangle,
        /*    Diamond,
           Star,
           Heart*/
    }

    public enum BlockColor
    {
        Red,
        Blue,
        Yellow,
        /*       Green,
               Purple,
               Orange*/
    }

    public enum BlockSize
    {
        Small,
        Medium,
        Large
    }

    // 블록 데이터 클래스
    [System.Serializable]
    public class BlockData
    {
        public ShapeType shape;
        public BlockColor color;
        public BlockSize size;
        public int number; // 1-6

        public BlockData(ShapeType shape, BlockColor color, BlockSize size, int number)
        {
            this.shape = shape;
            this.color = color;
            this.size = size;
            this.number = number;
        }

        public override string ToString()
        {
            return $"{size} {color} {shape} ({number})";
        }
    }

    // 개별 블록 컴포넌트 (단순화)
    public class Block : MonoBehaviour
    {
        [Header("Block Data")]
        public BlockData data;

        [Header("Visual Components")]
        [HideInInspector] public MeshRenderer meshRenderer;
        public TextMesh numberText;
        public UnityEvent onBlockMoveEnd;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            //numberText = GetComponentInChildren<TextMesh>();
        }
        public void SetupBlock(BlockData blockData)
        {
            this.data = blockData;
            ApplyVisuals();
        }

        void ApplyVisuals()
        {
            // 색상 적용
            if (meshRenderer != null)
            {
                meshRenderer.material.color = GetColorFromEnum(data.color);
            }

            // 크기 적용
            Vector3 scale = Vector3.one;
            switch (data.size)
            {
                case BlockSize.Small: scale = Vector3.one * 0.7f; break;
                case BlockSize.Medium: scale = Vector3.one * 1.0f; break;
                case BlockSize.Large: scale = Vector3.one * 1.3f; break;
            }
            //transform.localScale = scale;

            // 숫자 적용
            if (numberText != null)
            {
                numberText.text = data.number.ToString();
            }
        }

        Color GetColorFromEnum(BlockColor blockColor)
        {
            switch (blockColor)
            {
                case BlockColor.Red: return Color.red;
                case BlockColor.Blue: return Color.blue;
                case BlockColor.Yellow: return Color.yellow;
                /*                case BlockColor.Green: return Color.green;
                                case BlockColor.Purple: return new Color(0.5f, 0f, 0.5f);
                                case BlockColor.Orange: return new Color(1f, 0.5f, 0f);*/
                default: return Color.white;
            }
        }

        public void Move(SwipeDirection dir)
        {
            switch (dir)
            {
                case SwipeDirection.Left:
                    StartCoroutine(CoMove(Vector3.left));
                    break;
                case SwipeDirection.Right:
                    StartCoroutine(CoMove(Vector3.right));
                    break;
                case SwipeDirection.Up:
                    StartCoroutine(CoMove(Vector3.up));
                    break;
                case SwipeDirection.Down:
                    StartCoroutine(CoMove(Vector3.down));
                    break;
                default:
                    break;
            }
        }

        private IEnumerator CoMove(Vector3 dir)
        {
            float time = 0f;
            float duration = 3f;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                transform.position = Vector3.Lerp(transform.position, dir * 5f, t);
                yield return null;
            }
            onBlockMoveEnd?.Invoke();
        }
    }


}