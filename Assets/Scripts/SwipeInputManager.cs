using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SwipeSort
{
    // 스와이프 방향 열거형
    public enum SwipeDirection
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    // 스와이프 이벤트 데이터
    [System.Serializable]
    public class SwipeData
    {
        public SwipeDirection direction;
        public Vector2 startPosition;
        public Vector2 endPosition;
        public float distance;
        public float duration;
        public float speed;

        public SwipeData(SwipeDirection dir, Vector2 start, Vector2 end, float time)
        {
            direction = dir;
            startPosition = start;
            endPosition = end;
            distance = Vector2.Distance(start, end);
            duration = time;
            speed = distance / time;
        }
    }

    // 스와이프 입력을 감지하는 매니저
    public class SwipeInputManager : MonoBehaviour
    {
        [Header("Swipe Settings")]
        [Tooltip("최소 스와이프 거리 (픽셀)")]
        public float minSwipeDistance = 50f;

        [Tooltip("최대 스와이프 시간 (초)")]
        public float maxSwipeTime = 1f;

        [Tooltip("스와이프 각도 허용 범위 (도)")]
        public float angleThreshold = 30f;

        [Header("Debug")]
        public bool showDebugInfo = false;
        public bool drawSwipeLine = false;

        // 이벤트 시스템
        public event Action<SwipeData> OnSwipeDetected;
        public event Action<SwipeDirection> OnSwipeUp;
        public event Action<SwipeDirection> OnSwipeDown;
        public event Action<SwipeDirection> OnSwipeLeft;
        public event Action<SwipeDirection> OnSwipeRight;

        // 내부 변수들
        private Vector2 startTouchPosition;
        private Vector2 endTouchPosition;
        private float startTime;
        private bool isTouching = false;

        public static SwipeInputManager instance { get; set; }
        private void Awake()
        {
            instance = this;

        }
        void Update()
        {
            // 모바일 터치 입력
            if (Input.touchCount > 0)
            {
                HandleTouchInput();
            }
            // PC에서 마우스로 테스트
            else if (Application.isEditor || !Application.isMobilePlatform)
            {
                HandleMouseInput();
            }
        }

        #region Touch Input (모바일)
        void HandleTouchInput()
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchStart(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnTouchEnd(touch.position);
                    break;
            }
        }
        #endregion

        #region Mouse Input (PC 테스트용)
        void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTouchStart(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnTouchEnd(Input.mousePosition);
            }
        }
        #endregion

        #region Touch Events
        void OnTouchStart(Vector2 position)
        {
            startTouchPosition = position;
            startTime = Time.time;
            isTouching = true;

            if (showDebugInfo)
            {
                Debug.Log($"Touch Start: {position}");
            }
        }

        void OnTouchEnd(Vector2 position)
        {
            if (!isTouching) return;

            endTouchPosition = position;
            float endTime = Time.time;
            float swipeTime = endTime - startTime;

            isTouching = false;

            // 스와이프 감지 및 처리
            SwipeDirection direction = DetectSwipeDirection(startTouchPosition, endTouchPosition, swipeTime);

            if (direction != SwipeDirection.None)
            {
                SwipeData swipeData = new SwipeData(direction, startTouchPosition, endTouchPosition, swipeTime);
                ProcessSwipe(swipeData);
            }

            if (showDebugInfo)
            {
                Debug.Log($"Touch End: {position}, Direction: {direction}, Time: {swipeTime:F2}s");
            }
        }
        #endregion

        #region Swipe Detection
        SwipeDirection DetectSwipeDirection(Vector2 start, Vector2 end, float time)
        {
            // 시간이 너무 오래 걸렸으면 스와이프 아님
            if (time > maxSwipeTime)
            {
                if (showDebugInfo) Debug.Log("Swipe too slow");
                return SwipeDirection.None;
            }

            Vector2 swipeVector = end - start;
            float distance = swipeVector.magnitude;

            // 거리가 너무 짧으면 스와이프 아님
            if (distance < minSwipeDistance)
            {
                if (showDebugInfo) Debug.Log($"Swipe too short: {distance}px < {minSwipeDistance}px");
                return SwipeDirection.None;
            }

            // 각도 계산
            float angle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f; // 0-360 범위로 변환

            // 방향 결정
            SwipeDirection direction = GetDirectionFromAngle(angle);

            if (showDebugInfo)
            {
                Debug.Log($"Swipe Vector: {swipeVector}, Distance: {distance:F1}px, Angle: {angle:F1}°, Direction: {direction}");
            }

            return direction;
        }

        SwipeDirection GetDirectionFromAngle(float angle)
        {
            // 각도 범위로 방향 결정
            if (IsAngleInRange(angle, 90f, angleThreshold))
                return SwipeDirection.Up;
            else if (IsAngleInRange(angle, 270f, angleThreshold))
                return SwipeDirection.Down;
            else if (IsAngleInRange(angle, 0f, angleThreshold) || IsAngleInRange(angle, 360f, angleThreshold))
                return SwipeDirection.Right;
            else if (IsAngleInRange(angle, 180f, angleThreshold))
                return SwipeDirection.Left;

            return SwipeDirection.None;
        }

        bool IsAngleInRange(float angle, float targetAngle, float threshold)
        {
            float diff = Mathf.Abs(Mathf.DeltaAngle(angle, targetAngle));
            return diff <= threshold;
        }
        #endregion

        #region Swipe Processing
        void ProcessSwipe(SwipeData swipeData)
        {
            // 전체 스와이프 이벤트 발생
            OnSwipeDetected?.Invoke(swipeData);

            // 방향별 이벤트 발생
            switch (swipeData.direction)
            {
                case SwipeDirection.Up:
                    OnSwipeUp?.Invoke(swipeData.direction);
                    break;
                case SwipeDirection.Down:
                    OnSwipeDown?.Invoke(swipeData.direction);
                    break;
                case SwipeDirection.Left:
                    OnSwipeLeft?.Invoke(swipeData.direction);
                    break;
                case SwipeDirection.Right:
                    OnSwipeRight?.Invoke(swipeData.direction);
                    break;
            }

            if (showDebugInfo)
            {
                Debug.Log($"✅ Swipe Processed: {swipeData.direction} | Distance: {swipeData.distance:F1}px | Speed: {swipeData.speed:F1}px/s");
            }
        }
        #endregion

        #region Gizmos (디버그용)
        void OnDrawGizmos()
        {
            if (drawSwipeLine && isTouching)
            {
                // 화면 좌표를 월드 좌표로 변환
                Vector3 startWorld = Camera.main.ScreenToWorldPoint(new Vector3(startTouchPosition.x, startTouchPosition.y, 10f));
                Vector3 currentWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(startWorld, currentWorld);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(startWorld, 0.1f);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(currentWorld, 0.1f);
            }
        }
        #endregion

        #region Public Methods
        // 외부에서 민감도 조정 가능
        public void SetSwipeSettings(float minDistance, float maxTime, float angleThresh)
        {
            minSwipeDistance = minDistance;
            maxSwipeTime = maxTime;
            angleThreshold = angleThresh;
        }

        // 현재 터치 중인지 확인
        public bool IsTouching()
        {
            return isTouching;
        }

        // 현재 터치 위치 반환 (터치 중일 때만)
        public Vector2? GetCurrentTouchPosition()
        {
            if (!isTouching) return null;

            if (Input.touchCount > 0)
                return Input.GetTouch(0).position;
            else if (Input.GetMouseButton(0))
                return Input.mousePosition;

            return null;
        }
        #endregion
    }

    // 사용 예시: 스와이프 이벤트를 받는 다른 스크립트
    /*    public class SwipeTestReceiver : MonoBehaviour
        {
            void OnEnable()
            {
                // 이벤트 구독
                SwipeInputManager.OnSwipeUp += OnSwipeUpReceived;
                SwipeInputManager.OnSwipeDown += OnSwipeDownReceived;
                SwipeInputManager.OnSwipeDetected += OnAnySwipeReceived;
            }

            void OnDisable()
            {
                // 이벤트 구독 해제
                SwipeInputManager.OnSwipeUp -= OnSwipeUpReceived;
                SwipeInputManager.OnSwipeDown -= OnSwipeDownReceived;
                SwipeInputManager.OnSwipeDetected -= OnAnySwipeReceived;
            }

            void OnSwipeUpReceived(SwipeDirection direction)
            {
                Debug.Log("🔼 위로 스와이프! - 블록 통과시키기");
                // 여기서 블록을 통과시키는 로직 호출
            }

            void OnSwipeDownReceived(SwipeDirection direction)
            {
                Debug.Log("🔽 아래로 스와이프! - 블록 버리기");
                // 여기서 블록을 버리는 로직 호출
            }

            void OnAnySwipeReceived(SwipeData swipeData)
            {
                Debug.Log($"스와이프 감지: {swipeData.direction} | 속도: {swipeData.speed:F1}px/s");
            }
        }*/
}