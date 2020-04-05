# Avalonia-TreeViewBug
Code to recreate a bug in Avalonia TreeView v0.9.7


When I try to re-add an item to a Parent that was previously removed create an exception in `TreeContainerIndex.Add` in the line `this._containerToItem.Add(container, item);`

To Reproduce:
- Add Parent1 instance
- Add Child1 instance (child of Parent 1)
- Expand Parent1 instance
- Remove Child1 instance
- RemoveParent1 instance
- Add again the same Parent1 instance
- Add again Child1 instance (child of Parent 1)
- Expand Parent1 instance ==> Crash

I would expect that the children containers don't get properly cleaned up when the Parent is removed.

![Alt Text](https://github.com/Voulz/Avalonia-TreeViewBug/blob/master/TreeViewBug.gif)
