using UnityEngine;

/// <summary>
/// Describes the quality of a roll's outcome based on its Effect.
/// </summary>
public enum EffectOutcome
{
    /// <summary>The check failed by a large margin (-6 or less).</summary>
    ExceptionalFailure,
    /// <summary>The check failed (-1 to -5).</summary>
    Failure,
    /// <summary>The check succeeded (0 to +5).</summary>
    Success,
    /// <summary>The check succeeded by a large margin (+6 or more).</summary>
    ExceptionalSuccess
}

/// <summary>
/// Defines the difficulty level of an action, which translates to a roll modifier.
/// </summary>
public enum RollDifficulty
{
    Simple = 6,
    Easy = 4,
    Routine = 2,
    Average = 0,
    Difficult = -2,
    VeryDifficult = -4,
    Formidable = -6
}

/// <summary>
/// A circumstance modifier that can be applied to a roll, giving a +1 bonus or -1 penalty.
/// </summary>
public enum Circumstance
{
    /// <summary>A -1 penalty is applied to the roll.</summary>
    Disadvantage = -1,
    /// <summary>No circumstantial modifier is applied.</summary>
    Neutral = 0,
    /// <summary>A +1 bonus is applied to the roll.</summary>
    Advantage = 1
}

/// <summary>
/// A struct to hold the detailed results of an action roll.
/// </summary>
public struct RollResult
{
    public int Die1;
    public int Die2;
    public int Modifier;
    public int Total;
    public bool Succeeded;
    /// <summary>The difference between the total roll and the success threshold (8).</summary>
    public int Effect;
    /// <summary>The qualitative outcome of the roll (e.g., Success, ExceptionalFailure).</summary>
    public EffectOutcome Outcome;
}

/// <summary>
/// A static utility class to handle the game's core dice rolling mechanic.
/// </summary>
public static class ActionRoller
{
    /// <summary>
    /// The target number to meet or exceed for an action to succeed.
    /// </summary>
    private const int SuccessThreshold = 8;

    /// <summary>
    /// Performs an action roll with a given difficulty, an additional modifier, and a circumstance.
    /// </summary>
    /// <param name="difficulty">The inherent difficulty of the task.</param>
    /// <param name="additionalModifier">Any other modifiers to add (e.g., from character stats).</param>
    /// <param name="circumstance">A circumstantial modifier (+1 for Advantage, -1 for Disadvantage).</param>
    /// <returns>A RollResult struct containing the detailed outcome of the roll.</returns>
    public static RollResult Check(RollDifficulty difficulty, int additionalModifier = 0, Circumstance circumstance = Circumstance.Neutral)
    {
        int totalModifier = (int)difficulty + additionalModifier + (int)circumstance;
        return Check(totalModifier);
    }

    /// <summary>
    /// Performs a standard action roll (2d6 + modifier) and returns the result.
    /// </summary>
    /// <param name="modifier">The total modifier to add to the roll. Can be positive or negative.</param>
    /// <returns>A RollResult struct containing the detailed outcome of the roll.</returns>
    public static RollResult Check(int modifier)
    {
        // 1. Roll two 6-sided dice.
        // In Unity, Random.Range for integers has an exclusive maximum value, so we use 7.
        int die1 = Random.Range(1, 7);
        int die2 = Random.Range(1, 7);

        // 2. Calculate the total sum.
        int total = die1 + die2 + modifier;

        // 3. Check against the success threshold.
        bool success = total >= SuccessThreshold;

        // 4. Calculate the Effect and determine the qualitative outcome.
        int effect = total - SuccessThreshold;
        EffectOutcome outcome;

        if (effect >= 6)
        {
            outcome = EffectOutcome.ExceptionalSuccess;
        }
        else if (effect >= 0) // Covers 0 to 5
        {
            outcome = EffectOutcome.Success;
        }
        else if (effect <= -6) // Covers -6 and lower
        {
            outcome = EffectOutcome.ExceptionalFailure;
        }
        else // Covers -1 to -5
        {
            outcome = EffectOutcome.Failure;
        }

        // For debugging, it's helpful to see the roll details in the console.
        Debug.Log($"Roll: {die1} + {die2} + ({modifier}) = {total}. Target: {SuccessThreshold}. Effect: {effect} ({outcome}). Success: {success}");

        // 5. Return the detailed result.
        return new RollResult
        {
            Die1 = die1,
            Die2 = die2,
            Modifier = modifier,
            Total = total,
            Succeeded = success,
            Effect = effect,
            Outcome = outcome
        };
    }
}