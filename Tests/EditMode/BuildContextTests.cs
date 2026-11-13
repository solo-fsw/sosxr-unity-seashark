using NUnit.Framework;


namespace SOSXR.SeaShark.Tests
{
    /// <summary>
    ///     Build-context filters that used the DEVELOPMENT_BUILD symbol, which is deprecated in Unity 6.6 (UAC0009).
    ///     They now decide at runtime from Application.isEditor and Debug.isDebugBuild.
    ///     Player-only branches can't run in EditMode, so the pure filter is tested with every context.
    /// </summary>
    [TestFixture]
    public class BuildContextTests
    {
        // Arguments: type, isEditor, isPlaying, isDebugBuild, expected
        [TestCase(BuildTriggerType.Always, true, true, true, true)]
        [TestCase(BuildTriggerType.Always, false, true, false, true)]
        [TestCase(BuildTriggerType.OnlyInEditor, true, true, true, true)]
        [TestCase(BuildTriggerType.OnlyInEditor, true, false, true, false)]
        [TestCase(BuildTriggerType.OnlyInEditor, false, true, true, false)]
        [TestCase(BuildTriggerType.OnlyInBuilds, true, true, true, false)]
        [TestCase(BuildTriggerType.OnlyInBuilds, false, true, true, true)]
        [TestCase(BuildTriggerType.OnlyInBuilds, false, true, false, true)]
        [TestCase(BuildTriggerType.OnlyInDevelopmentBuilds, true, true, true, false)]
        [TestCase(BuildTriggerType.OnlyInDevelopmentBuilds, false, true, true, true)]
        [TestCase(BuildTriggerType.OnlyInDevelopmentBuilds, false, true, false, false)]
        [TestCase(BuildTriggerType.OnlyInProductionBuilds, true, true, true, false)]
        [TestCase(BuildTriggerType.OnlyInProductionBuilds, false, true, true, false)]
        [TestCase(BuildTriggerType.OnlyInProductionBuilds, false, true, false, true)]
        public void MatchesBuildContext_MatchesPreviousSymbolBehaviour(BuildTriggerType type, bool isEditor, bool isPlaying, bool isDebugBuild, bool expected)
        {
            Assert.AreEqual(expected, AdditionalUnityEvent.MatchesBuildContext(type, isEditor, isPlaying, isDebugBuild));
        }
    }
}
