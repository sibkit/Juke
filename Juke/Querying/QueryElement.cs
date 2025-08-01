using Juke.Common;

namespace Juke.Querying;

public abstract class QueryElement: IChild<QueryElement> {
    public QueryElement? Parent { get; set; }
    
    protected T? ValidateParent<T>(T? oldElement, T? newElement) where T : QueryElement {
        if (oldElement != null) 
            oldElement.Parent = null;
        if (newElement != null) 
            newElement.Parent = this;
        return newElement;
    }
}