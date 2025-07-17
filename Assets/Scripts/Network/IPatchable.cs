using System.Collections.Generic;

public interface IPatchable
{
    void ApplyPatch(IDictionary<string, object> jsonObject);
}