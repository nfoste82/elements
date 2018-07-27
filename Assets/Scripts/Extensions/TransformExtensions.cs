using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class TransformExtensions
{
    //Source: https://answers.unity.com/questions/285133/find-child-of-a-game-object-using-tag.html
    
    /// <summary>
    /// Find all children of the Transform by tag (includes self)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public static List<Transform> FindChildrenByTag(this Transform transform, params string[] tags)
    {
        List<Transform> list = new List<Transform>();
        
        foreach (var tran in transform.Cast<Transform>().ToList())
        {
            list.AddRange(tran.FindChildrenByTag(tags)); // recursively check children
        }

        if (tags.Any(tag => tag == transform.tag))
        {
            list.Add(transform); // we matched, add this transform
        }

        return list;
    }
    
    /// <summary>
    /// Find all children of the GameObject by tag (includes self)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public static List<GameObject> FindChildrenByTag(this GameObject gameObject, params string[] tags)
    {
        return FindChildrenByTag(gameObject.transform, tags)
            //.Cast<GameObject>() // Can't use Cast here :(
            .Select(tran => tran.gameObject)
            .Where(gameOb => gameOb != null)
            .ToList();
    }
    
    //https://answers.unity.com/questions/799429/transformfindstring-no-longer-finds-grandchild.html
 
    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        
        if (result != null)
        {
            return result;
        }

        foreach(Transform child in aParent)
        {
            result = child.FindDeepChild(aName);
            if (result != null)
            {
                return result;
            }
        }
        
        return null;
    } 
}