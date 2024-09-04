public enum DebugEnvironment
{
    // CORE: Fundamental environment types
    // CONJUGATE:  Combinations of COREs representing broader environment categories
    RELEASE = 1 << 0, //...............................0001.........................................{ CORE }
    EDITOR = 1 << 1, //................................0010.........................................{ CORE }
    DEVELOPMENT = 1 << 2, //...........................0100.........................................{ CORE }
    BUILD = DEVELOPMENT | RELEASE, //..................0101 (Debug or Release build, not Editor)....{ CONJUGATE }
    EDITOR_OR_DEVELOPMENT = EDITOR | DEVELOPMENT, //...0110 (Editor or Development, not Release)....{ CONJUGATE }
    ALL = RELEASE | EDITOR_OR_DEVELOPMENT, //..........0111 (All environments)......................{ CONJUGATE }
}

/// <summary>
/// Defines the current environment the application is running in,
/// thus defining the current debug environment
/// </summary>
public static class Environment
{
    private static DebugEnvironment _debugEnvironment;

    /// <summary>
    /// Determines what environment (out of the 3 core ones) is currently active
    /// </summary>
    static Environment()
    {
        #if UNITY_EDITOR && !DEVELOPMENT_BUILD
            _debugEnvironment = DebugEnvironment.EDITOR;
        #elif !UNITY_EDITOR && DEVELOPMENT_BUILD
            _debugEnvironment = DebugEnvironment.DEVELOPMENT;
        #else
            _debugEnvironment = DebugEnvironment.RELEASE;
        #endif
    }

    /// <summary>
    /// Determines if the debug environment is in the given environment
    /// </summary>
    /// <param name="environment">The environment to compare the debug environment to</param>
    /// <returns>Whether or not the debug environment is in the given environment</returns>
    public static bool Is(DebugEnvironment environment)
    {
        return (environment & _debugEnvironment) == _debugEnvironment; 
    }
}