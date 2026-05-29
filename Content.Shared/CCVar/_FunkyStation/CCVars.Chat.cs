using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     Whether the /suicide command is enabled for players
    /// </summary>
    public static readonly CVarDef<bool> SuicideCommandEnabled =
        CVarDef.Create("funky.suicide_command_enabled", true, CVar.SERVERONLY);
}
