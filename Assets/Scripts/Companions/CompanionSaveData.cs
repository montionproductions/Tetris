using System;
using System.Collections.Generic;

[Serializable]
public class CompanionSaveData
{
    public string id;
    public bool unlocked;
    public int affinityLevel = 1;
    public int affinityXp;
    public List<string> unlockedOutfitIds = new();
}

[Serializable]
public class CompanionSaveWrapper
{
    public List<CompanionSaveData> companions = new();
}