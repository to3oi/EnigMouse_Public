﻿using UnityEngine;

public class StageObjectRock : BaseStageObject
{
    public float stageObjectMagma;

    private void Start()
    {
    }

    public StageObjectRock(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    public override bool isValidMove()
    {
        //ネズミが移動可能か判定する
        return false;
    }
}