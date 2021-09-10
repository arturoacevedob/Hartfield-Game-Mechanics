using UnityEngine;

public class StoryTrackerEntry : MonoBehaviour {
    public StoryTracker StoryTracker;

    [Header("Global Variables")]
    public bool key;
    public bool hat;

    [Header("Lambert")]
    public bool lambertInitiated;
    public bool rugCollected;
    public bool wigSnatched;
    public bool lambertQuestComplete;

    [Header("Twins")]
    public bool twinsInitiated;

    public void Set() {
        if (key) StoryTracker.key = true;
        if (hat) StoryTracker.hat = true;
        if (lambertInitiated) StoryTracker.LambertInitaited();
        if (rugCollected) StoryTracker.rugCollected = true;
        if (wigSnatched) StoryTracker.Wig();
        if (lambertQuestComplete) StoryTracker.lambertQuestComplete = true;
    }
}
