// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

namespace Mediapipe.Unity.HandTracking
{
    public class HandTrackingSolution : ImageSourceSolution<HandTrackingGraph>
    {
        [SerializeField] private DetectionListAnnotationController _palmDetectionsAnnotationController;
        [SerializeField] private NormalizedRectListAnnotationController _handRectsFromPalmDetectionsAnnotationController;
        [SerializeField] private MultiHandLandmarkListAnnotationController _handLandmarksAnnotationController;
        [SerializeField] private NormalizedRectListAnnotationController _handRectsFromLandmarksAnnotationController;

        [SerializeField] private HandPositionReader handPositionReader;

        public HandTrackingGraph.ModelComplexity modelComplexity
        {
            get => graphRunner.modelComplexity;
            set => graphRunner.modelComplexity = value;
        }

        public int maxNumHands
        {
            get => graphRunner.maxNumHands;
            set => graphRunner.maxNumHands = value;
        }

        public float minDetectionConfidence
        {
            get => graphRunner.minDetectionConfidence;
            set => graphRunner.minDetectionConfidence = value;
        }

        public float minTrackingConfidence
        {
            get => graphRunner.minTrackingConfidence;
            set => graphRunner.minTrackingConfidence = value;
        }

        protected override void OnStartRun()
        {
            if (!runningMode.IsSynchronous())
            {
                graphRunner.OnPalmDetectectionsOutput += OnPalmDetectionsOutput;
                graphRunner.OnHandRectsFromPalmDetectionsOutput += OnHandRectsFromPalmDetectionsOutput;
                graphRunner.OnHandLandmarksOutput += OnHandLandmarksOutput;
                graphRunner.OnHandRectsFromLandmarksOutput += OnHandRectsFromLandmarksOutput;
                graphRunner.OnHandednessOutput += OnHandednessOutput;
            }

            var imageSource = ImageSourceProvider.ImageSource;

            SetupAnnotationController(
              _palmDetectionsAnnotationController,
              imageSource,
              true
            );

            SetupAnnotationController(
              _handRectsFromPalmDetectionsAnnotationController,
              imageSource,
              true
            );

            SetupAnnotationController(
              _handLandmarksAnnotationController,
              imageSource,
              true
            );

            SetupAnnotationController(
              _handRectsFromLandmarksAnnotationController,
              imageSource,
              true
            );
        }

        protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
        {
            graphRunner.AddTextureFrameToInputStream(textureFrame);
        }

        protected override IEnumerator WaitForNextValue()
        {
            List<Detection> palmDetections = null;
            List<NormalizedRect> handRectsFromPalmDetections = null;
            List<NormalizedLandmarkList> handLandmarks = null;
            List<LandmarkList> handWorldLandmarks = null;
            List<NormalizedRect> handRectsFromLandmarks = null;
            List<ClassificationList> handedness = null;

            if (runningMode == RunningMode.Sync)
            {
                var _ = graphRunner.TryGetNext(
                  out palmDetections,
                  out handRectsFromPalmDetections,
                  out handLandmarks,
                  out handWorldLandmarks,
                  out handRectsFromLandmarks,
                  out handedness,
                  true
                );
            }
            else if (runningMode == RunningMode.NonBlockingSync)
            {
                yield return new WaitUntil(() =>
                  graphRunner.TryGetNext(
                    out palmDetections,
                    out handRectsFromPalmDetections,
                    out handLandmarks,
                    out handWorldLandmarks,
                    out handRectsFromLandmarks,
                    out handedness,
                    false
                  )
                );
            }

            _palmDetectionsAnnotationController.DrawNow(palmDetections);

            _handRectsFromPalmDetectionsAnnotationController.DrawNow(
              handRectsFromPalmDetections
            );

            _handLandmarksAnnotationController.DrawNow(
              handLandmarks,
              handedness
            );

            _handRectsFromLandmarksAnnotationController.DrawNow(
              handRectsFromLandmarks
            );
        }

        private void OnPalmDetectionsOutput(
          object stream,
          OutputEventArgs<List<Detection>> eventArgs)
        {
            _palmDetectionsAnnotationController.DrawLater(eventArgs.value);
        }

        private void OnHandRectsFromPalmDetectionsOutput(
          object stream,
          OutputEventArgs<List<NormalizedRect>> eventArgs)
        {
            _handRectsFromPalmDetectionsAnnotationController.DrawLater(
              eventArgs.value
            );
        }


        private void OnHandLandmarksOutput(
            object stream,
            OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
        {
            _handLandmarksAnnotationController.DrawLater(
                eventArgs.value
            );

            if (eventArgs.value == null ||
                eventArgs.value.Count == 0)
            {
                if (handPositionReader != null)
                {
                    handPositionReader.ClearHand();
                }
                return;
            }

            NormalizedLandmarkList hand = eventArgs.value[0];

            if (hand.Landmark == null ||
                hand.Landmark.Count < 21)
            {
                return;
            }

            // 手首
            NormalizedLandmark wrist = hand.Landmark[0];

            // 人差し指の付け根
            NormalizedLandmark index = hand.Landmark[5];

            // 中指の付け根
            NormalizedLandmark middle = hand.Landmark[9];

            // 小指の付け根
            NormalizedLandmark pinky = hand.Landmark[17];

            if (handPositionReader != null)
            {
                // 手の大きさ
                float dx = index.X - pinky.X;
                float dy = index.Y - pinky.Y;

                float handSize = Mathf.Sqrt(
                    dx * dx + dy * dy
                );

                // 手首の位置を送信
                handPositionReader.SetHandPosition(
                    wrist.X,
                    wrist.Y,
                    wrist.Z,
                    handSize
                );


                // 手のひらの横方向の角度
                float angleZ = Mathf.Atan2(
                    index.Y - pinky.Y,
                    index.X - pinky.X
                ) * Mathf.Rad2Deg;

                // 手のひらの前後方向の角度
                float angleX = Mathf.Atan2(
                    middle.Z - wrist.Z,
                    Mathf.Abs(middle.Y - wrist.Y)
                ) * Mathf.Rad2Deg;

                // 角度を補正して送信
                handPositionReader.SetHandTilt(
                    -angleX,
                    0f,
                    -angleZ
                );

            }
        }


        private void OnHandRectsFromLandmarksOutput(
          object stream,
          OutputEventArgs<List<NormalizedRect>> eventArgs)
        {
            _handRectsFromLandmarksAnnotationController.DrawLater(
              eventArgs.value
            );
        }

        private void OnHandednessOutput(
          object stream,
          OutputEventArgs<List<ClassificationList>> eventArgs)
        {
            _handLandmarksAnnotationController.DrawLater(
              eventArgs.value
            );
        }
    }
}