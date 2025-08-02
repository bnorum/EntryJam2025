using UnityEngine;

public interface ITechnique : ICommand
{
    string Name { get; }
    string Description { get; }
    int Cost { get; }
}
