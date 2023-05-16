using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AddressableLoader
{
    
    public static async Task InitAssets<T>(string label, List<T> createdObjs) where T : Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;
        foreach (var location in locations)
        {
            var tasks = await Addressables.LoadAssetAsync<T>(location).Task as T;
            if (!createdObjs.Contains(tasks))
                createdObjs.Add(tasks);
        }
    }        

    public static async Task InitAssets<T>(AssetLabelReference reference, List<T> createdObjs) where T : Object
        => await InitAssets(reference.labelString, createdObjs);
    
}
