public interface IItem
{
    string Name { get; }
    string Description { get; }
    ICommand Command { get; } // The command to execute when using this item
}
