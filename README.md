# LinQTreeExtensions
Tree traversal extensions for LinQ

##ZDV.LinQ.Hierarchy
This can be used for traversing hierarchies where each node knows its parent and/or children.

```
Grab it from nuget: Install-Package ZDV.LinQ.Hierarchy
```

Currently supported _static_ methods:

- HierarchyWalker.Ancestors(startNode, parentSelector, [includeSelf])
- HierarchyWalker.AncestorsAndSelf(startNode, parentSelector)
- HierarchyWalker.Descendants(startNode, childSelector, [includeSelf])
- HierarchyWalker.DescendantsAndSelf(startNode, childSelector)
- HierarchyWalker.Siblings(startNode, parentSelector, childSelector, [includeSelf])
- HierarchyWalker.SiblingsAndSelf(startNode, parentSelector, childSelector)
 
Addionally the methods are implemented as extensions methods for IEnumerable.
Keep in mind, that the extension methods are **not** guaranteed to return distinct results!
