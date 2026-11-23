namespace SOSXR.SeaShark
{
    /// <summary>
    ///     The Factory pattern encapsulates object creation logic, letting you swap HOW objects are created without changing the code that USES them.
    ///     In Unity, factories are essential for generating trial definitions, creating stimulus configurations, producing condition variants,
    ///     or instantiating any GameObject that needs configuration beyond simple Instantiate().
    ///     Factories pair naturally with trial sequencers — the factory handles trial creation, the sequencer handles presentation order.
    /// </summary>
    /// <typeparam name="T">The product type created by this factory.</typeparam>
    public interface IFactory<T>
    {
        /// <summary>
        ///     Creates and returns a product instance.
        /// </summary>
        /// <returns>A newly created product instance.</returns>
        T Create();
    }
}
