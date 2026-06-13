// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace FourteenNumbers {

    public class StatsGraph : MonoBehaviour {
        [SerializeField] private RectTransform graphContainer;
        [SerializeField] private Sprite dotSprite;
        public Font labelFont;

        private const uint GRAPH_GENESIS_GAME_DAY = 540;

        // Make sure (max - min) / increment is integer divisible.
        private const uint MIN_SCORE = 110;
        private const uint MAX_SCORE = 190;
        private const uint PERFECT_SCORE = 210;
        private const uint PERFECT_SCORE_SPACE = 2;
        private const uint SCORE_INCREMENT = 5;
        private const int MAX_SCALED = (int)(PERFECT_SCORE_SPACE + (MAX_SCORE - MIN_SCORE) / SCORE_INCREMENT);
        private const int DATA_SIZE = 1 + (int)((MAX_SCORE - MIN_SCORE) / SCORE_INCREMENT);


        private const uint MAX_NUM_Y_TICKS = 5;

        private const uint X_AXIS_HORIZONTAL_LABEL_Y_OFFS = 50;
        private const uint X_AXIS_VERTICAL_LABEL_Y_OFFS = 35;
        private const uint Y_AXIS_HORIZONTAL_LABEL_X_OFFS = 12;

        private const uint X_LEFT_OFFSET = 80;
        private const uint X_RIGHT_OFFSET = 50;
        private const uint Y_TOP_OFFSET = 70;
        private const uint Y_BOTTOM_OFFSET = 40;

        private const float BAR_LINE_WIDTH = 25f;

        private uint[] scoreDistribution;
        private uint scoreDistributionPerfectScore;


        public void Start() {
            AuditLog.Log("Stats graph screen");
            createScoreDistribution();
            //createDummyScoreDistribution();


            showGraph();
        }


        private void createScoreDistribution() {
            scoreDistribution = new uint[DATA_SIZE];
            uint todaysGameDay = Timeline.GameDay();

            for (uint i = GRAPH_GENESIS_GAME_DAY; i <= todaysGameDay; i++)
            {
                uint score = Stats.GetBestScoreForDay((uint)i);
                if (score == PERFECT_SCORE) 
                {
                    scoreDistributionPerfectScore++;
                }
                else 
                {
                    // Round score up and normalise to the data set.
                    // Discard scores below roundup(MIN_SCORE)
                    uint index = score + (SCORE_INCREMENT - 1);
                    if (index >= MIN_SCORE) 
                    {
                        index -= MIN_SCORE;
                        index /= SCORE_INCREMENT;
                        scoreDistribution[index]++;
                    }
                }
            }
        }

        private void createDummyScoreDistribution() {
            uint[] scoreDist = new uint[DATA_SIZE];
            scoreDist[0] = 10;
            scoreDist[1] = 20;
            scoreDist[2] = 130;
            scoreDist[3] = 10;
            scoreDist[5] = 5;
            scoreDist[10] = 1;
            scoreDist[DATA_SIZE - 1] = 65;
            scoreDistribution = scoreDist;
            scoreDistributionPerfectScore = 33;
        }


        private void showGraph()
        {
            float graphHeight = graphContainer.sizeDelta.y - Y_TOP_OFFSET - Y_BOTTOM_OFFSET;
            float graphWidth = graphContainer.sizeDelta.x - X_LEFT_OFFSET - X_RIGHT_OFFSET;

            //AuditLog.Log($"Stats: h: {graphHeight}, w:{graphWidth}");


            // Auto-scale Y: Find the max value in your list
            int yMax = 0;
            foreach (int val in scoreDistribution) {
                if (val > yMax) 
                {
                    yMax = val;
                }
            }
            if (scoreDistributionPerfectScore > yMax)
            {
                yMax = (int) scoreDistributionPerfectScore;   
            }
            yMax = Mathf.Max(yMax, 1); // Avoid division by zero
            //AuditLog.Log($"Stats: ymax: {yMax}");

            float xSize = graphWidth / MAX_SCALED;
            //AuditLog.Log($"Stats: xSize: {xSize}");

            // 1. Draw Y-Axis Labels
            // Separator count is the y axis lines
            uint separatorCount = MAX_NUM_Y_TICKS;
            if (yMax < MAX_NUM_Y_TICKS)
            {
                separatorCount = (uint) yMax;
            }
            for (int i = 0; i <= separatorCount; i++) {
                float normalizedValue = i / (float)separatorCount;
                float yPos = normalizedValue * graphHeight + Y_TOP_OFFSET;
                string labelText = Mathf.RoundToInt(normalizedValue * yMax).ToString();
                CreateLabel(new Vector2(Y_AXIS_HORIZONTAL_LABEL_X_OFFS, yPos), labelText, TextAlignmentOptions.Right, false);
            }

            // 2. Draw X-Axis Labels
            for (int i = 0; i < DATA_SIZE; i++) {
                float xPos = i * xSize + X_LEFT_OFFSET;
                string xLabel = (MIN_SCORE + i * SCORE_INCREMENT).ToString();
                CreateLabel(new Vector2(xPos, X_AXIS_VERTICAL_LABEL_Y_OFFS), xLabel, TextAlignmentOptions.Center, true);

                // Scale Y relative to the container height and max value
                float yPosition = (scoreDistribution[i] / (float)yMax) * graphHeight + Y_TOP_OFFSET;
                CreateDot(new Vector2(xPos, yPosition));
            }

            // Put in perfect score.
            float xPos1 = graphWidth + X_LEFT_OFFSET;
            CreateLabel(new Vector2(xPos1, X_AXIS_HORIZONTAL_LABEL_Y_OFFS), PERFECT_SCORE.ToString(), TextAlignmentOptions.Top, false);
            float yPosition1 = (scoreDistributionPerfectScore / (float)yMax) * graphHeight + Y_TOP_OFFSET;
            CreateDot(new Vector2(xPos1, yPosition1));
        }

        private void CreateLabel(Vector2 anchoredPosition, string text, TextAlignmentOptions anchor, bool rotate)
        {
            //AuditLog.Log($"Stats: create label: {text}");
            // 1. Create a new GameObject for the text
            GameObject textObj = new GameObject("DynamicText");

            // 2. Make the Image the parent of this text
            textObj.transform.SetParent(graphContainer.transform, false);

            // 3. Add the TextMeshPro component
            TextMeshProUGUI myText = textObj.AddComponent<TextMeshProUGUI>();
            
            // 4. Set the text and styling
            myText.text = text;
            myText.fontSize = 30;
            myText.alignment = anchor; //TextAlignmentOptions.Center;
            myText.color = Color.black;

            // 5. Make the text fill the parent image area
            RectTransform rt = textObj.GetComponent<RectTransform>();
            // rt.anchorMin = new Vector2(0, 0);
            // rt.anchorMax = new Vector2(1, 1);
            // rt.offsetMin = Vector2.zero;
            // rt.offsetMax = Vector2.zero;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = new Vector2(100, 20); // Large enough for the number
            rt.anchorMin = rt.anchorMax = new Vector2(0, 0);

            if (rotate) 
            {
                rt.eulerAngles = new Vector3(0, 0, 90f);
            }
        }

        private void CreateDot(Vector2 anchoredPosition)
        {
            float xPosition = anchoredPosition.x;
            float yPosition = anchoredPosition.y;
            float lineHeight = yPosition - Y_TOP_OFFSET;
            if (lineHeight > 0f)
            {
                GameObject lineObj = new GameObject("barLine", typeof(Image));
                lineObj.transform.SetParent(graphContainer, false);
                Image lineImage = lineObj.GetComponent<Image>();
                lineImage.sprite = dotSprite;
                lineImage.color = new Color(0.6f, 0.6f, 0.6f, 0.85f);
                RectTransform lineRect = lineObj.GetComponent<RectTransform>();
                lineRect.anchorMin = lineRect.anchorMax = new Vector2(0, 0);
                lineRect.pivot = new Vector2(0.5f, 0f);
                lineRect.anchoredPosition = new Vector2(xPosition, Y_TOP_OFFSET);
                lineRect.sizeDelta = new Vector2(BAR_LINE_WIDTH, lineHeight);
            }

            GameObject dot = new GameObject("dot", typeof(Image));
            dot.transform.SetParent(graphContainer, false);
            dot.GetComponent<Image>().sprite = dotSprite;
            RectTransform rect = dot.GetComponent<RectTransform>();
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(8, 8);
            rect.anchorMin = rect.anchorMax = new Vector2(0, 0);
        }
    }
}