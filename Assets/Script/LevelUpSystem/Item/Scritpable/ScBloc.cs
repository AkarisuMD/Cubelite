using UnityEngine;

public class ScBloc : ScriptableObject
{
    public Sprite Fond, FondStats, Icon;
    public string Name;
    [TextArea] public string Info;
    public BlocStats blocStats;
    public Object ScriptBloc;
}
