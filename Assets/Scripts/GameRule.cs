using SwipeSort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GameRule 
{
    public string ruleName;
    public string ruleDescription;
    public SwipeDirection correctDirection;

    public GameRule(string name, string desc, SwipeDirection correctDir)
    {
        ruleName = name;
        ruleDescription = desc;
        correctDirection = correctDir;
    }

    // 블록이 이 규칙에 맞는지 체크
    public abstract bool IsBlockMatch(BlockData block);
}
public class ColorRule : GameRule
{
    public BlockColor targetColor;

    public ColorRule(BlockColor color, SwipeDirection correctDir)
        : base($"{color} Only", $"Pass {color} blocks", correctDir)
    {
        targetColor = color;
    }

    public override bool IsBlockMatch(BlockData block)
    {
        return block.color == targetColor;
    }
}

// 도형 기반 규칙
public class ShapeRule : GameRule
{
    public ShapeType targetShape;

    public ShapeRule(ShapeType shape, SwipeDirection correctDir)
        : base($"{shape} Only", $"Pass {shape} shapes", correctDir)
    {
        targetShape = shape;
    }

    public override bool IsBlockMatch(BlockData block)
    {
        return block.shape == targetShape;
    }
}

// 숫자 기반 규칙
public class NumberRule : GameRule
{
    public int targetNumber;
    public bool isEven; // 짝수/홀수 규칙용

    public NumberRule(int number, SwipeDirection correctDir)
        : base($"Number {number}", $"Pass blocks with {number}", correctDir)
    {
        targetNumber = number;
    }

    // 짝수/홀수 규칙 생성자
    public NumberRule(bool evenRule, SwipeDirection correctDir)
        : base(evenRule ? "Even Numbers" : "Odd Numbers",
               evenRule ? "Pass even numbers" : "Pass odd numbers", correctDir)
    {
        isEven = evenRule;
        targetNumber = -1; // 특정 숫자가 아님을 표시
    }

    public override bool IsBlockMatch(BlockData block)
    {
        if (targetNumber >= 0) // 특정 숫자 규칙
        {
            return block.number == targetNumber;
        }
        else // 짝수/홀수 규칙
        {
            return isEven ? (block.number % 2 == 0) : (block.number % 2 == 1);
        }
    }
}